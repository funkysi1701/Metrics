using Pulumi;
using Pulumi.Azure.AppInsights;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using System.Collections.Generic;
using System.Linq;
using Atlas = Pulumi.Mongodbatlas;
using Cloudflare = Pulumi.Cloudflare;
using Config = Pulumi.Config;
using Kind = Pulumi.AzureNative.Storage.Kind;
using res = Pulumi.AzureNative.Resources;

namespace Metrics.Pulumi
{
    public class MetricsStack : Stack
    {
        public MetricsStack()
        {
            var config = new Config();
            var name = $"metrics-pulumi-{config.Require("env")}";

            var resourceGroup = new res.ResourceGroup(name, new res.ResourceGroupArgs
            {
                ResourceGroupName = name,
                Location = "westeurope"
            });

            var storageAccount = new StorageAccount($"metricssa{config.Require("env")}", new StorageAccountArgs
            {
                ResourceGroupName = resourceGroup.Name,
                Sku = new SkuArgs
                {
                    Name = SkuName.Standard_LRS,
                },
                Kind = Kind.StorageV2,
            });

            var container = new BlobContainer("deploymentzips", new BlobContainerArgs
            {
                AccountName = storageAccount.Name,
                PublicAccess = PublicAccess.None,
                ResourceGroupName = resourceGroup.Name,
            });

            var blobtimer = new Blob($"Metrics.TimerFunction.zip", new BlobArgs
            {
                AccountName = storageAccount.Name,
                ContainerName = container.Name,
                ResourceGroupName = resourceGroup.Name,
                Type = BlobType.Block,
                Source = new FileArchive($"..\\Metrics.TimerFunction\\bin\\Release\\net6.0\\publish")
            });

            var blobhttp = new Blob($"Metrics.Function.zip", new BlobArgs
            {
                AccountName = storageAccount.Name,
                ContainerName = container.Name,
                ResourceGroupName = resourceGroup.Name,
                Type = BlobType.Block,
                Source = new FileArchive($"..\\Metrics.Function\\bin\\Release\\net6.0\\publish")
            });

            var deploymentZipBlobtimerSasUrl = StackFunctions.SignedBlobReadUrl(blobtimer, container, storageAccount, resourceGroup);
            var deploymentZipBlobhttpSasUrl = StackFunctions.SignedBlobReadUrl(blobhttp, container, storageAccount, resourceGroup);

            var appServicePlan = new AppServicePlan($"metrics-pulumi-functions-asp-{config.Require("env")}", new AppServicePlanArgs
            {
                ResourceGroupName = resourceGroup.Name,
                Kind = "FunctionApp",
                Sku = new SkuDescriptionArgs
                {
                    Tier = "Dynamic",
                    Name = "Y1"
                }
            });

            var appInsights = new Insights("appInsights", new InsightsArgs
            {
                Location = resourceGroup.Location,
                ResourceGroupName = resourceGroup.Name,
                ApplicationType = "web",
                Name = $"metrics-pulumi-appInsights-{config.Require("env")}",
            });

            var writeAnnotations = new ApiKey($"writeAnnotations", new ApiKeyArgs
            {
                ApplicationInsightsId = appInsights.Id,
                WritePermissions =
                {
                    "annotations",
                },
            });

            var project = new Atlas.Project($"pulumi-project-{config.Require("env")}", new Atlas.ProjectArgs
            {
                OrgId = config.RequireSecret("AtlasOrg"),
                Name = $"pulumi-project-{config.Require("env")}",
                IsDataExplorerEnabled = true
            });

            var cluster = new Atlas.Cluster($"pulumi-cluster-{config.Require("env")}", new Atlas.ClusterArgs
            {
                ProjectId = project.Id,
                ProviderInstanceSizeName = "M0",
                BackingProviderName = "AZURE",
                ProviderName = "TENANT",
                ProviderRegionName = "EUROPE_NORTH",
            });

            var password = Output.CreateSecret(StackFunctions.RandomString(10));

            _ = new Atlas.DatabaseUser($"{config.Require("env")}-user", new Atlas.DatabaseUserArgs
            {
                AuthDatabaseName = "admin",
                Password = password,
                ProjectId = project.Id,
                Username = $"{config.Require("env")}-user",
                Roles =
                {
                    new Atlas.Inputs.DatabaseUserRoleArgs
                    {
                        DatabaseName = $"Metrics-{config.Require("env")}",
                        RoleName = "readWrite",
                    },
                    new Atlas.Inputs.DatabaseUserRoleArgs
                    {
                        DatabaseName = "admin",
                        RoleName = "readAnyDatabase",
                    },
                }
            });

            var Con = cluster.SrvAddress.Apply(x => StackFunctions.InsertLoginDetails(x, $"{config.Require("env")}-user", password, $"Metrics-{config.Require("env")}"));

            var timerfunction = new WebApp("timerfunction", new WebAppArgs
            {
                Name = $"metrics-pulumi-timerfunction-{config.Require("env")}",
                Kind = "FunctionApp",
                ResourceGroupName = resourceGroup.Name,
                ServerFarmId = appServicePlan.Id,
                SiteConfig = new SiteConfigArgs
                {
                    AppSettings = StackFunctions.AppSettings(deploymentZipBlobtimerSasUrl, resourceGroup, storageAccount, config, Con, appInsights)
                },
            });

            var function = new WebApp("function", new WebAppArgs
            {
                Name = $"metrics-pulumi-function-{config.Require("env")}",
                Kind = "FunctionApp",
                ResourceGroupName = resourceGroup.Name,
                ServerFarmId = appServicePlan.Id,
                SiteConfig = new SiteConfigArgs
                {
                    AppSettings = StackFunctions.AppSettings(deploymentZipBlobhttpSasUrl, resourceGroup, storageAccount, config, Con, appInsights)
                },
            });

            var Ips = Output.Tuple(timerfunction.PossibleOutboundIpAddresses, function.PossibleOutboundIpAddresses).Apply(t =>
            {
                var (timer, http) = t;
                return $"{timer},{http}";
            });

            var listOfIps = Ips.Apply(x => x.Split(",").Distinct().ToList());

            listOfIps.Apply(x =>
            {
                x.ForEach(y => StackFunctions.AddFWRule(y, project.Id));
                return "ok";
            });

            this.Readme = Output.Create(System.IO.File.ReadAllText("../README.md"));
            this.WriteAnnotationsApiKey = writeAnnotations.Key;
            this.WriteAnnotationsApplicationKey = appInsights.AppId;

            var staticSite = new StaticSite("staticSite", new StaticSiteArgs
            {
                Branch = config.Require("branch"),
                BuildProperties = new StaticSiteBuildPropertiesArgs
                {
                    ApiLocation = "Metrics.StaticFunction",
                    AppArtifactLocation = "wwwroot",
                    AppLocation = "Metrics.Static",
                    SkipGithubActionWorkflowGeneration = false
                },
                Location = "westeurope",
                Name = $"metrics-pulumi-static-{config.Require("env")}",
                RepositoryToken = config.RequireSecret("GitHubToken"),
                RepositoryUrl = "https://github.com/funkysi1701/Metrics",
                ResourceGroupName = resourceGroup.Name,
                Sku = new SkuDescriptionArgs
                {
                    Name = "Free",
                    Tier = "Free",
                },
            });

            var zone = Cloudflare.GetZone.Invoke(new Cloudflare.GetZoneInvokeArgs()
            {
                AccountId = config.RequireSecret("accountId"),
                ZoneId = config.RequireSecret("zoneId"),
            });

            _ = new Cloudflare.Record("cloudflare-cname", new()
            {
                ZoneId = zone.Apply(x => x.ZoneId),
                Name = $"metrics-{config.Require("env")}",
                Value = staticSite.DefaultHostname,
                Type = "CNAME",
                Ttl = 3600,
            });

            _ = new StaticSiteCustomDomain("staticSiteCustomDomain", new()
            {
                DomainName = $"metrics-{config.Require("env")}.funkysi1701.com",
                Name = staticSite.Name,
                ResourceGroupName = resourceGroup.Name,
            });

            _ = new res.Deployment("static-webapp-configuration",
                    new res.DeploymentArgs
                    {
                        ResourceGroupName = resourceGroup.Name,
                        Properties = new res.Inputs.DeploymentPropertiesArgs
                        {
                            Mode = res.DeploymentMode.Incremental,
                            Template = new Dictionary<string, object>
                            {
                                { "$schema", "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#" },
                                { "contentVersion", "1.0.0.0" },
                                {
                                    "resources", new List<Dictionary<string, object>>()
                                    {
                                        new ()
                                        {
                                            { "type", "Microsoft.Web/staticSites/config" },
                                            { "apiVersion", "2020-10-01" },
                                            { "name", staticSite.Name.Apply(c => $"{c}/appsettings") },
                                            { "kind", "string" },
                                            {
                                                "properties", new Dictionary<string, object>()
                                                {
                                                    { "ConnectionString", Con.Apply(x => x) },
                                                    { "CollectionName", $"Metrics-{config.Require("env")}" },
                                                    { "APPLICATIONINSIGHTS_CONNECTION_STRING", appInsights.ConnectionString },
                                                    { "APPINSIGHTS_INSTRUMENTATIONKEY", appInsights.InstrumentationKey },
                                                    { "DatabaseName", $"Metrics-{config.Require("env")}" },
                                                    { "FunctionAPI", $"https://metrics-pulumi-function-{config.Require("env")}.azurewebsites.net" }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    });
        }

        [Output]
        public Output<string> Readme { get; set; }

        [Output("writeAnnotationsApiKey")]
        public Output<string> WriteAnnotationsApiKey { get; set; }

        [Output("writeAnnotationsApplicationKey")]
        public Output<string> WriteAnnotationsApplicationKey { get; set; }
    }
}

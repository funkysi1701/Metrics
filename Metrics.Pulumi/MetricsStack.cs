﻿using Pulumi;
using Pulumi.Azure.AppInsights;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using System;
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

            var deploymentZipBlobtimerSasUrl = SignedBlobReadUrl(blobtimer, container, storageAccount, resourceGroup);
            var deploymentZipBlobhttpSasUrl = SignedBlobReadUrl(blobhttp, container, storageAccount, resourceGroup);

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

            var password = Output.CreateSecret(RandomString(10));

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

            var Con = cluster.SrvAddress.Apply(x => InsertLoginDetails(x, $"{config.Require("env")}-user", password, $"Metrics-{config.Require("env")}"));

            var timerfunction = new WebApp("timerfunction", new WebAppArgs
            {
                Name = $"metrics-pulumi-timerfunction-{config.Require("env")}",
                Kind = "FunctionApp",
                ResourceGroupName = resourceGroup.Name,
                ServerFarmId = appServicePlan.Id,
                SiteConfig = new SiteConfigArgs
                {
                    AppSettings = AppSettings(deploymentZipBlobtimerSasUrl, resourceGroup, storageAccount, config, Con, appInsights)
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
                    AppSettings = AppSettings(deploymentZipBlobhttpSasUrl, resourceGroup, storageAccount, config, Con, appInsights)
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
                x.ForEach(y => AddFWRule(y, project.Id));
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
                                                    { "FunctionAPI", $"https://metrics-pulumi-function-{config.Require("env")}.azurewebsites.net" },
                                                    { "MaxRecords", 80000 },
                                                    { "MaxPages", 16 }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    });
        }

        private static readonly Random random = new();

        private static NameValuePairArgs[] AppSettings(Output<string> url, res.ResourceGroup resourceGroup, StorageAccount storageAccount, Config config, Output<string> Con, Insights appInsights)
        {
            return new[]
                    {
                        new NameValuePairArgs{
                            Name = "WEBSITE_RUN_FROM_PACKAGE",
                            Value = url,
                        },
                        new NameValuePairArgs{
                            Name = "AzureWebJobsStorage",
                            Value = GetConnectionString(resourceGroup.Name, storageAccount.Name),
                        },
                        new NameValuePairArgs{
                            Name = "WEBSITE_CONTENTAZUREFILECONNECTIONSTRING",
                            Value = GetConnectionString(resourceGroup.Name, storageAccount.Name),
                        },
                        new NameValuePairArgs{
                            Name = "WEBSITE_CONTENTSHARE",
                            Value = $"metrics-pulumi-timerfunction-{config.Require("env")}-091999e1",
                        },
                        new NameValuePairArgs{
                            Name = "FUNCTIONS_WORKER_RUNTIME",
                            Value = "dotnet",
                        },
                        new NameValuePairArgs
                        {
                            Name = "MastodonServer",
                            Value = config.RequireSecret("MastodonServer"),
                        },
                        new NameValuePairArgs
                        {
                            Name = "MastodonUser",
                            Value = config.RequireSecret("MastodonUser"),
                        },
                        new NameValuePairArgs
                        {
                            Name = "MastodonPass",
                            Value = config.RequireSecret("MastodonPass"),
                        },
                        new NameValuePairArgs{
                            Name = "GitHubToken",
                            Value = config.RequireSecret("GitHubToken"),
                        },
                        new NameValuePairArgs{
                            Name = "Username1",
                            Value = "funkysi1701",
                        },
                        new NameValuePairArgs{
                            Name = "DEVTOAPI",
                            Value = config.RequireSecret("DEVTOAPI"),
                        },
                        new NameValuePairArgs{
                            Name = "Env",
                            Value = config.Require("env"),
                        },
                        new NameValuePairArgs{
                            Name = "EnableToot",
                            Value = "false",
                        },
                        new NameValuePairArgs{
                            Name = "EnableTweet",
                            Value = "false",
                        },
                        new NameValuePairArgs{
                            Name = "DEVTOURL",
                            Value = "https://dev.to/api/",
                        },
                        new NameValuePairArgs{
                            Name = "RSSFeed",
                            Value = "https://www.funkysi1701.com/index.xml",
                        },
                        new NameValuePairArgs{
                            Name = "OPSAPI",
                            Value = config.RequireSecret("OPSAPI"),
                        },
                        new NameValuePairArgs{
                            Name = "OPSURL",
                            Value = "https://community.ops.io/api/",
                        },
                        new NameValuePairArgs{
                            Name = "OctopusKey",
                            Value = config.RequireSecret("OctopusKey"),
                        },
                        new NameValuePairArgs{
                            Name = "OctopusElecMPAN",
                            Value = config.RequireSecret("OctopusElecMPAN"),
                        },
                        new NameValuePairArgs{
                            Name = "OctopusElecSerial",
                            Value = config.RequireSecret("OctopusElecSerial"),
                        },
                        new NameValuePairArgs{
                            Name = "OctopusGasMPAN",
                            Value = config.RequireSecret("OctopusGasMPAN"),
                        },
                        new NameValuePairArgs{
                            Name = "OctopusGasSerial",
                            Value = config.RequireSecret("OctopusGasSerial"),
                        },
                        new NameValuePairArgs{
                            Name = "DatabaseName",
                            Value = $"Metrics-{config.Require("env")}",
                        },
                        new NameValuePairArgs{
                            Name = "OldRSSFeed",
                            Value = "",
                        },
                        new NameValuePairArgs{
                            Name = "ConnectionString",
                            Value = Con.Apply(x => x),
                        },
                        new NameValuePairArgs{
                            Name = "CollectionName",
                            Value = $"Metrics-{config.Require("env")}",
                        },
                        new NameValuePairArgs{
                            Name = "runtime",
                            Value = "dotnet",
                        },
                        new NameValuePairArgs{
                            Name = "APPINSIGHTS_INSTRUMENTATIONKEY",
                            Value = appInsights.InstrumentationKey,
                        },
                        new NameValuePairArgs{
                            Name = "APPLICATIONINSIGHTS_CONNECTION_STRING",
                            Value = appInsights.ConnectionString,
                        },
                        new NameValuePairArgs{
                            Name = "FUNCTIONS_EXTENSION_VERSION",
                            Value = "~4",
                        },
                    };
        }

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static Output<string> InsertLoginDetails(string input, string user, Output<string> pass, string DatabaseName)
        {
            var connectionString = input.Split("//");
            var combinedString = Output.Format($"{connectionString[0]}//{user}:{pass}@{connectionString[1]}/{DatabaseName}?retryWrites=true&w=majority");
            return combinedString;
        }

        private static void AddFWRule(string ip, Output<string> projectid)
        {
            _ = new Atlas.ProjectIpAccessList(ip, new Atlas.ProjectIpAccessListArgs
            {
                Comment = "ip address",
                IpAddress = ip,
                ProjectId = projectid,
            });
        }

        [Output]
        public Output<string> Readme { get; set; }

        [Output("writeAnnotationsApiKey")]
        public Output<string> WriteAnnotationsApiKey { get; set; }

        [Output("writeAnnotationsApplicationKey")]
        public Output<string> WriteAnnotationsApplicationKey { get; set; }

        private static Output<string> GetConnectionString(Input<string> resourceGroupName, Input<string> accountName)
        {
            // Retrieve the primary storage account key.
            var storageAccountKeys = ListStorageAccountKeys.Invoke(new ListStorageAccountKeysInvokeArgs
            {
                ResourceGroupName = resourceGroupName,
                AccountName = accountName
            });

            return storageAccountKeys.Apply(keys =>
            {
                var primaryStorageKey = keys.Keys[0].Value;

                // Build the connection string to the storage account.
                return Output.Format($"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={primaryStorageKey}");
            });
        }

        public static Output<string> SignedBlobReadUrl(Blob blob, BlobContainer container, StorageAccount account, res.ResourceGroup resourceGroup)
        {
            return Output.Tuple(blob.Name, container.Name, account.Name, resourceGroup.Name)
                .Apply(t =>
                {
                    (string blobName, string containerName, string accountName, string resourceGroupName) = t;

                    var blobSAS = ListStorageAccountServiceSAS.InvokeAsync(new ListStorageAccountServiceSASArgs
                    {
                        AccountName = accountName,
                        Protocols = HttpProtocol.Https,
                        SharedAccessStartTime = DateTime.Now.Subtract(new TimeSpan(365, 0, 0, 0)).ToString("yyyy-MM-dd"),
                        SharedAccessExpiryTime = DateTime.Now.AddDays(3650).ToString("yyyy-MM-dd"),
                        Resource = SignedResource.C,
                        ResourceGroupName = resourceGroupName,
                        Permissions = Permissions.R,
                        CanonicalizedResource = "/blob/" + accountName + "/" + containerName,
                        ContentType = "application/json",
                        CacheControl = "max-age=5",
                        ContentDisposition = "inline",
                        ContentEncoding = "deflate",
                    });
                    return Output.Format($"https://{accountName}.blob.core.windows.net/{containerName}/{blobName}?{blobSAS.Result.ServiceSasToken}");
                });
        }
    }
}

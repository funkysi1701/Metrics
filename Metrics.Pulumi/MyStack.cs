using Pulumi;
using Pulumi.AzureNative.Insights;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using System;
using Atlas = Pulumi.Mongodbatlas;
using Kind = Pulumi.AzureNative.Storage.Kind;
using Azure = Pulumi.Azure;
using Config = Pulumi.Config;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;

namespace Metrics.Pulumi
{
    public class MyStack : Stack
    {
        public MyStack()
        {
            var config = new Config();
            var name = $"metrics-pulumi-{config.Require("env")}";

            var resourceGroup = new ResourceGroup(name, new ResourceGroupArgs
            {
                ResourceGroupName = name,
                Location = "westeurope"
            });

            var storageAccount = new StorageAccount("sa", new StorageAccountArgs
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

            var appServicePlan = new AppServicePlan("functions-win-asp", new AppServicePlanArgs
            {
                ResourceGroupName = resourceGroup.Name,

                Kind = "FunctionApp",

                Sku = new SkuDescriptionArgs
                {
                    Tier = "Dynamic",
                    Name = "Y1"
                }
            });

            var appInsights = new Azure.AppInsights.Insights("appInsights", new Azure.AppInsights.InsightsArgs
            {
                Location = resourceGroup.Location,
                ResourceGroupName = resourceGroup.Name,
                ApplicationType = "web",
                Name = $"metrics-pulumi-appInsights-{config.Require("env")}",
            });

            var writeAnnotations = new Azure.AppInsights.ApiKey($"writeAnnotations", new Azure.AppInsights.ApiKeyArgs
            {
                ApplicationInsightsId = appInsights.Id,
                WritePermissions =
                {
                    "annotations",
                },
            });

            var timerfunction = new WebApp("timerfunction", new WebAppArgs
            {
                Name = $"metrics-pulumi-timerfunction-{config.Require("env")}",
                Kind = "FunctionApp",
                ResourceGroupName = resourceGroup.Name,
                ServerFarmId = appServicePlan.Id,
                SiteConfig = new SiteConfigArgs
                {
                    AppSettings = new[]
                    {
                        new NameValuePairArgs{
                            Name = "WEBSITE_RUN_FROM_PACKAGE",
                            Value = deploymentZipBlobtimerSasUrl,
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
                            Value = $"metrics-pulumi-timerfunction-{config.Require("env")}-091999e2",
                        },
                        new NameValuePairArgs{
                            Name = "FUNCTIONS_WORKER_RUNTIME",
                            Value = "dotnet",
                        },
                        new NameValuePairArgs{
                            Name = "TWConsumerKey",
                            Value = config.RequireSecret("TWConsumerKey"),
                        },
                        new NameValuePairArgs{
                            Name = "TWConsumerSecret",
                            Value = config.RequireSecret("TWConsumerSecret"),
                        },
                        new NameValuePairArgs{
                            Name = "TWAccessToken",
                            Value = config.RequireSecret("TWAccessToken"),
                        },
                        new NameValuePairArgs{
                            Name = "TWAccessSecret",
                            Value = config.RequireSecret("TWAccessSecret"),
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
                            Value = $"Metrics{config.Require("env")}",
                        },
                        new NameValuePairArgs{
                            Name = "OldRSSFeed",
                            Value = "https://www.pwnedpass.com/feed/",
                        },
                        new NameValuePairArgs{
                            Name = "ConnectionString",
                            Value = config.RequireSecret("ConnectionString"),
                        },
                        new NameValuePairArgs{
                            Name = "CollectionName",
                            Value = $"Metrics{config.Require("env")}",
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
                    },
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
                    AppSettings = new[]
                    {
                        new NameValuePairArgs{
                            Name = "WEBSITE_RUN_FROM_PACKAGE",
                            Value = deploymentZipBlobhttpSasUrl,
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
                        new NameValuePairArgs{
                            Name = "TWConsumerKey",
                            Value = config.RequireSecret("TWConsumerKey"),
                        },
                        new NameValuePairArgs{
                            Name = "TWConsumerSecret",
                            Value = config.RequireSecret("TWConsumerSecret"),
                        },
                        new NameValuePairArgs{
                            Name = "TWAccessToken",
                            Value = config.RequireSecret("TWAccessToken"),
                        },
                        new NameValuePairArgs{
                            Name = "TWAccessSecret",
                            Value = config.RequireSecret("TWAccessSecret"),
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
                            Value = $"Metrics{config.Require("env")}",
                        },
                        new NameValuePairArgs{
                            Name = "OldRSSFeed",
                            Value = "https://www.pwnedpass.com/feed/",
                        },
                        new NameValuePairArgs{
                            Name = "ConnectionString",
                            Value = config.RequireSecret("ConnectionString"),
                        },
                        new NameValuePairArgs{
                            Name = "CollectionName",
                            Value = $"Metrics{config.Require("env")}",
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
                    },
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

            this.Con = cluster.ConnectionStrings;

            var test = new Atlas.DatabaseUser($"{config.Require("env")}-user", new Atlas.DatabaseUserArgs
            {
                AuthDatabaseName = "admin",
                Password = $"{config.Require("env")}-user",
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

            this.Readme = Output.Create(System.IO.File.ReadAllText("./Pulumi.README.md"));
            this.WriteAnnotationsApiKey = writeAnnotations.Key;
            this.WriteAnnotationsApplicationKey = appInsights.AppId;

            //var staticSite = new StaticSite("staticSite", new StaticSiteArgs
            //{
            //    Branch = config.Require("branch"),
            //    BuildProperties = new StaticSiteBuildPropertiesArgs
            //    {
            //        ApiLocation = "Metrics.Function",
            //        AppArtifactLocation = "wwwroot",
            //        AppLocation = "Metrics.Static",
            //    },
            //    Location = "westeurope",
            //    Name = $"metrics-pulumi-static-{config.Require("env")}",
            //    RepositoryToken = config.RequireSecret("GitHubToken"),
            //    RepositoryUrl = "https://github.com/funkysi1701/Metrics",
            //    ResourceGroupName = resourceGroup.Name,
            //    Sku = new SkuDescriptionArgs
            //    {
            //        Name = "Free",
            //        Tier = "Free",
            //    },
            //});
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
        public Output<ImmutableArray<Atlas.Outputs.ClusterConnectionString>> Con { get; set; }

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

        public static Output<string> SignedBlobReadUrl(Blob blob, BlobContainer container, StorageAccount account, ResourceGroup resourceGroup)
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

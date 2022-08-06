using Pulumi;
using Pulumi.AzureNative.Insights;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using Kind = Pulumi.AzureNative.Storage.Kind;

namespace Metrics.Pulumi
{
    public class MyStack : Stack
    {
        public MyStack()
        {
            this.Readme = Output.Create(System.IO.File.ReadAllText("./Pulumi.README.md"));
            var config = new Config();
            var name = $"metrics-pulumi-{config.Require("env")}";

            var resourceGroup = new ResourceGroup(name, new ResourceGroupArgs
            {
                ResourceGroupName = name,
                Location = "UKSouth"
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

            var container = new BlobContainer("zips-container", new BlobContainerArgs
            {
                AccountName = storageAccount.Name,
                PublicAccess = PublicAccess.None,
                ResourceGroupName = resourceGroup.Name,
            });

            var blob = new Blob("zip", new BlobArgs
            {
                AccountName = storageAccount.Name,
                ContainerName = container.Name,
                ResourceGroupName = resourceGroup.Name,
                Type = BlobType.Block
            });

            var appInsights = new Component("appInsights", new ComponentArgs
            {
                ResourceName = $"metrics-pulumi-appInsights-{config.Require("env")}",
                ApplicationType = ApplicationType.Web,
                Kind = "web",
                ResourceGroupName = resourceGroup.Name,
            });

            var app = new WebApp("app", new WebAppArgs
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
                            Name = "AzureWebJobsStorage",
                            Value = GetConnectionString(resourceGroup.Name, storageAccount.Name),
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
                            Value = Output.Format($"InstrumentationKey={appInsights.InstrumentationKey};IngestionEndpoint=https://uksouth-0.in.applicationinsights.azure.com/;LiveEndpoint=https://uksouth.livediagnostics.monitor.azure.com/"),
                        },
                        new NameValuePairArgs{
                            Name = "FUNCTIONS_EXTENSION_VERSION",
                            Value = "~4",
                        },
                    },
                },
            });
        }

        [Output]
        public Output<string> Readme { get; set; }

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
    }
}

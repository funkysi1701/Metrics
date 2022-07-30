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

            var appServicePlan = new AppServicePlan("functions-linux-asp", new AppServicePlanArgs
            {
                ResourceGroupName = resourceGroup.Name,

                Kind = "Windows",

                Sku = new SkuDescriptionArgs
                {
                    Tier = "Dynamic",
                    Name = "Y1"
                },

                Reserved = true
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

            var codeBlobUrl = SignedBlobReadUrl(blob, container, storageAccount, resourceGroup);

            var appInsights = new Component("appInsights", new ComponentArgs
            {
                ApplicationType = ApplicationType.Web,
                Kind = "web",
                ResourceGroupName = resourceGroup.Name,
            });

            var app = new WebApp("app", new WebAppArgs
            {
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
                            Name = "runtime",
                            Value = "dotnet",
                        },
                        new NameValuePairArgs{
                            Name = "FUNCTIONS_WORKER_RUNTIME",
                            Value = "dotnet",
                        },
                        new NameValuePairArgs{
                            Name = "WEBSITE_RUN_FROM_PACKAGE",
                            Value = codeBlobUrl,
                        },
                        new NameValuePairArgs{
                            Name = "APPLICATIONINSIGHTS_CONNECTION_STRING",
                            Value = Output.Format($"InstrumentationKey={appInsights.InstrumentationKey}"),
                        },
                    },
                },
            });
        }

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

        private static Output<string> SignedBlobReadUrl(Blob blob, BlobContainer container, StorageAccount account, ResourceGroup resourceGroup)
        {
            var serviceSasToken = ListStorageAccountServiceSAS.Invoke(new ListStorageAccountServiceSASInvokeArgs
            {
                AccountName = account.Name,
                Protocols = HttpProtocol.Https,
                SharedAccessStartTime = "2021-01-01",
                SharedAccessExpiryTime = "2030-01-01",
                Resource = SignedResource.C,
                ResourceGroupName = resourceGroup.Name,
                Permissions = Permissions.R,
                CanonicalizedResource = Output.Format($"/blob/{account.Name}/{container.Name}"),
                ContentType = "application/json",
                CacheControl = "max-age=5",
                ContentDisposition = "inline",
                ContentEncoding = "deflate",
            }).Apply(blobSAS => blobSAS.ServiceSasToken);

            return Output.Format($"https://{account.Name}.blob.core.windows.net/{container.Name}/{blob.Name}?{serviceSasToken}");
        }
    }
}

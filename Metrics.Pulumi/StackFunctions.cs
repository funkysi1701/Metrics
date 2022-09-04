using Pulumi;
using Pulumi.Azure.AppInsights;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Web.Inputs;
using System;
using System.Linq;
using Atlas = Pulumi.Mongodbatlas;
using Config = Pulumi.Config;
using res = Pulumi.AzureNative.Resources;

namespace Metrics.Pulumi
{
    public static class StackFunctions
    {
        public static Output<string> InsertLoginDetails(string input, string user, Output<string> pass, string DatabaseName)
        {
            var connectionString = input.Split("//");
            var combinedString = Output.Format($"{connectionString[0]}//{user}:{pass}@{connectionString[1]}/{DatabaseName}?retryWrites=true&w=majority");
            return combinedString;
        }

        public static void AddFWRule(string ip, Output<string> projectid)
        {
            _ = new Atlas.ProjectIpAccessList(ip, new Atlas.ProjectIpAccessListArgs
            {
                Comment = "ip address",
                IpAddress = ip,
                ProjectId = projectid,
            });
        }

        private static readonly Random random = new();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
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

        public static NameValuePairArgs[] AppSettings(Output<string> url, res.ResourceGroup resourceGroup, StorageAccount storageAccount, Config config, Output<string> Con, Insights appInsights)
        {
            return new[]
                    {
                        new NameValuePairArgs{
                            Name = "WEBSITE_RUN_FROM_PACKAGE",
                            Value = url,
                        },
                        new NameValuePairArgs{
                            Name = "AzureWebJobsStorage",
                            Value = StackFunctions.GetConnectionString(resourceGroup.Name, storageAccount.Name),
                        },
                        new NameValuePairArgs{
                            Name = "WEBSITE_CONTENTAZUREFILECONNECTIONSTRING",
                            Value = StackFunctions.GetConnectionString(resourceGroup.Name, storageAccount.Name),
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
                            Value = $"Metrics-{config.Require("env")}",
                        },
                        new NameValuePairArgs{
                            Name = "OldRSSFeed",
                            Value = "https://www.pwnedpass.com/feed/",
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
    }
}

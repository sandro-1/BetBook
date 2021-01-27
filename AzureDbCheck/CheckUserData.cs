using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Xamarin.Forms;

namespace AzureDbCheck
{
    public class CheckUserData
    {
        //<ItemGroup>
        //    <ProjectReference Include = "..\BetBook\BetBook\BetBook.csproj" />
        //</ItemGroup >
        
        [FunctionName("CheckUserData")]
        public static void Run([CosmosDBTrigger(
            databaseName: "BetBook",
            collectionName: "UserData",
            ConnectionStringSetting = "connectionString",
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input, ILogger log)
        {
            if (input != null && input.Count > 0)
            {   
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using processCreateDeleteAlerts.Models;
using processCreateDeleteAlerts.Util;

namespace processCreateDeleteAlerts
{
    public static class createDeleteAlerts
    {
        [FunctionName("createDeleteAlerts")]
        public static void Run([QueueTrigger("activity-alert-queue", Connection = "AzureWebJobsStorage")]string myQueueItem, ILogger log)
        {
            var resourceHealthAlert = JsonConvert.DeserializeObject<ResourceAlert>(myQueueItem);
           
            var validResourceTypes = new List<string> { "microsoft.compute/virtualmachines", "microsoft.storage/storageaccounts", 
                "microsoft.network/connections","microsoft.network/expressroutecircuits", "microsoft.network/loadbalancers", "microsoft.network/virtualnetworkgateways"};
            string operationName = resourceHealthAlert.data.operationName.ToLower();
            bool contains = validResourceTypes.Contains(operationName.Replace("/write", "").Replace("/delete", ""), StringComparer.OrdinalIgnoreCase);
            if (contains)
            {
                //log.LogInformation($"Processing operation: " + operationName);
                var collectionId = GetEnvironmentVariable("CosmosDb_Collection");
                var databaseId = GetEnvironmentVariable("CosmosDb_Database");
                CosmosClient client = new CosmosClient(GetEnvironmentVariable("CosmosDb_Uri"), GetEnvironmentVariable("CosmosDb_Key"));

                var resourceAlertObj = AlertHelper.ConvertToDTO(resourceHealthAlert);

                if (operationName.Contains("write"))
                {
                    //log.LogInformation($"Creating record in cosmos db....");
                    //log.LogInformation($"PartitionKey: " + resourceAlertObj.resourceId);

                    resourceHealthAlert.data.location = ""; //TODO - ADD RESOURCE LOCATION HERE

                    ItemResponse<ResourceAlertDTO> response = client.GetContainer(databaseId, collectionId).CreateItemAsync(resourceAlertObj, new PartitionKey(resourceAlertObj.resourceId)).Result;
                }
                if (operationName.Contains("delete"))
                {
                    //log.LogInformation($"Deleting record from cosmos db....");
                    QueryDefinition sqlQuery = new QueryDefinition("select * from c where c.resourceId = @resourceId").WithParameter("@resourceId", resourceAlertObj.resourceId);
                    ResourceAlertDTO item = client.GetContainer(databaseId, collectionId).GetItemLinqQueryable<ResourceAlertDTO>(true).Where(b=>b.resourceId == resourceAlertObj.resourceId).AsEnumerable().FirstOrDefault();

                    //log.LogInformation($"Item Id: " + item.id);
                    //log.LogInformation($"PartitionKey: " + resourceAlertObj.resourceId);

                    ItemResponse<ResourceAlertDTO> deletedItem = client.GetContainer(databaseId, collectionId).DeleteItemAsync<ResourceAlertDTO>(item.id, new PartitionKey(item.resourceId)).Result;
                }
            }

            //log.LogInformation($"Resource Health Alert Processed");
        }
        public static string GetEnvironmentVariable(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName);
        }
    }
}
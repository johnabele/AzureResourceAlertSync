using processCreateDeleteAlerts.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace processCreateDeleteAlerts.Util
{
    public static class AlertHelper
    {
        public static ResourceAlert PopulateMetaData(ResourceAlert resource)
        {
            resource.data.resourceId = resource.data.resourceUri;
            resource.data.resourceName = resource.data.resourceUri.Split('/')[8];
            resource.data.resourceGroup = resource.data.resourceUri.Split('/')[4];
            resource.id = resource.data.subscriptionId + "_" + resource.data.resourceGroup + "_" + resource.data.resourceName;

            return resource;
        }

        public static ResourceAlertDTO ConvertToDTO(ResourceAlert resource)
        {
            var resourceAlertDTO = new ResourceAlertDTO()
            {
                alertId = Guid.NewGuid().ToString(),
                alertStatus = "Resolved",
                currentHealthStatus = "Available",
                previousHealthStatus = "null",
                eventTimestamp = resource.eventTime,
                resourceId = resource.data.resourceUri,
                id = Guid.NewGuid().ToString(),
                resourceType = resource.data.resourceType,
                subscriptionId = resource.data.subscriptionId
            };

            return resourceAlertDTO;
        }
    }
}

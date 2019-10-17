using System;
using System.Collections.Generic;
using System.Text;

namespace processCreateDeleteAlerts.Models
{
    public class ResourceAlert
    {
        public string id { get; set; }
        public DateTime eventTime { get; set; } 
        public data data { get; set; }
    }

    public class data
    {
        public string resourceId { get; set; }
        public string resourceName { get; set; }
        public string resourceGroup { get; set; }
        public string resourceUri { get; set; }
        public string subscriptionId { get; set; }
        public string status { get; set; }
        public string resourceType { get; set; }
        public string currentHealthStatus { get; set; }
        public string previousHealthStatus { get; set; }
        public string operationName { get; set; }
        public string location { get; set; }
    }
}
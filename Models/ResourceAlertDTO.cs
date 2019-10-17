using System;
using System.Collections.Generic;
using System.Text;

namespace processCreateDeleteAlerts.Models
{
    public class ResourceAlertDTO
    {
            public string id { get; set; }
            public string alertId { get; set; }
            public string resourceId { get; set; }
            public string subscriptionId { get; set; }
            public string alertStatus { get; set; }
            public string resourceType { get; set; }
            public string currentHealthStatus { get; set; }
            public string previousHealthStatus { get; set; }
            public DateTime eventTimestamp { get; set; }
        }
}

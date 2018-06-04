using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saturnin.Models
{
    public class DpmbRisObject
    {
        public int vehicleId { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public int route { get; set; }
        public string course { get; set; }
        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string headsign { get; set; }
        public int bearing { get; set; }
    }

}

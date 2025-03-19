using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app.Models
{
    
    public class Country
    {
        [JsonProperty("name")]
        public Name Name { get; set; }
        [JsonProperty("flags")]
        public Flags Flags { get; set; }
    }
    public class Name
    {
        [JsonProperty("common")]
        public string Common { get; set; }  
    }
    public class Flags
    {
        [JsonProperty("png")]
        public string Png { get; set; }
    }
}

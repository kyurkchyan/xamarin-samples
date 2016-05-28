using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FontSample.Model
{
    public class CustomFont
    {
        public string Name { get; set; }
        public string File { get; set; }
        [JsonIgnore]
        public byte[] Data { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LogWire.Controller.Data.Model
{
    
    public class ConfigurationEntry
    {
        
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }

        public ConfigurationEntry(string key, string value)
        {
            Key = key;
            Value = value;
        }

    }
}

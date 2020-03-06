using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LogWire.Controller.Data.Model.SIEM
{
    public class SIEMUserEntry
    {

        public Guid Id { get; set; }

        [Key]
        public string Username { get; set; }

    }
}

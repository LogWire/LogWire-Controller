using System;
using System.ComponentModel.DataAnnotations;

namespace LogWire.Controller.Data.Model.Application
{
    public class ApplicationEntry
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}

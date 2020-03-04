using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LogWire.Controller.Data.Model.Application
{
    public class ApplicationEntry
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}

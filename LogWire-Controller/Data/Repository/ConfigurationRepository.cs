using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogWire.Controller.Data.Context;
using LogWire.Controller.Data.Model;

namespace LogWire.Controller.Data.Repository
{
    public class ConfigurationRepository : IDataRepository<ConfigurationEntry>
    {

        readonly ConfigurationContext _context;

        public ConfigurationRepository(ConfigurationContext context)
        {
            _context = context;
        }

        public IEnumerable<ConfigurationEntry> GetAll()
        {
            return _context.Configuration.ToList();
        }

        public ConfigurationEntry Get(string key)
        {
            return _context.Configuration
                .FirstOrDefault(e => e.Key == key);
        }

        public void Add(ConfigurationEntry entity)
        {
            _context.Configuration.Add(entity);
            _context.SaveChanges();
        }

        public void Update(ConfigurationEntry dbEntity, ConfigurationEntry entity)
        {
            dbEntity.Value = entity.Value;
            _context.SaveChanges();
        }

        public void Delete(ConfigurationEntry entity)
        {
            _context.Configuration.Remove(entity);
            _context.SaveChanges();
        }
    }
}

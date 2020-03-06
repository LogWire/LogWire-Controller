using System.Collections.Generic;
using System.Linq;
using LogWire.Controller.Data.Context;
using LogWire.Controller.Data.Model;

namespace LogWire.Controller.Data.Repository
{
    public class ConfigurationRepository : IDataRepository<ConfigurationEntry>
    {

        readonly DataContext _context;

        public ConfigurationRepository(DataContext context)
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

        public int Count()
        {
            return _context.Configuration.Count();
        }

        public IEnumerable<ConfigurationEntry> GetByPrefix(string requestPrefix)
        {
            return _context.Configuration.Where(c => c.Key.StartsWith(requestPrefix)).ToList();
        }
    }
}

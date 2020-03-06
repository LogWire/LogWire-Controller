using System;
using System.Collections.Generic;
using System.Linq;
using LogWire.Controller.Data.Context;
using LogWire.Controller.Data.Model.Application;

namespace LogWire.Controller.Data.Repository.Application
{
    public class ApplicationRepository : IDataRepository<ApplicationEntry>
    {

        readonly ApplicationDataContext _context;

        public ApplicationRepository(ApplicationDataContext context)
        {
            _context = context;
        }

        public IEnumerable<ApplicationEntry> GetAll()
        {
            return _context.Applications.ToList();
        }

        public ApplicationEntry Get(string key)
        {
            return _context.Applications
                .FirstOrDefault(e => e.Id == Guid.Parse(key));
        }

        public void Add(ApplicationEntry entity)
        {
            _context.Applications.Add(entity);
            _context.SaveChanges();
        }

        public void Update(ApplicationEntry dbEntity, ApplicationEntry entity)
        {
            dbEntity.Name = entity.Name;
            _context.SaveChanges();
        }

        public void Delete(ApplicationEntry entity)
        {
            _context.Applications.Remove(entity);
            _context.SaveChanges();
        }

        public int Count()
        {
            return _context.Applications.Count();
        }
    }
}

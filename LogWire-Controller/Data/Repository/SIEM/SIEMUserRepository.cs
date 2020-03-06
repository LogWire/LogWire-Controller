using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogWire.Controller.Data.Context;
using LogWire.Controller.Data.Model.SIEM;
using LogWire.Controller.Data.Utils;
using LogWire.Controller.Data.Utils.PagedResults;

namespace LogWire.Controller.Data.Repository.SIEM
{
    public class SIEMUserRepository : IDataRepository<SIEMUserEntry>
    {
        readonly SIEMDataContext _context;

        public SIEMUserRepository(SIEMDataContext context)
        {
            _context = context;
        }

        public IEnumerable<SIEMUserEntry> GetAll()
        {
            return _context.Users.ToList();
        }

        public SIEMUserEntry Get(string key)
        {
            return _context.Users
                .FirstOrDefault(e => e.Username.Equals(key));
        }

        public void Add(SIEMUserEntry entity)
        {
            _context.Users.Add(entity);
            _context.SaveChanges();
        }

        public void Update(SIEMUserEntry dbEntity, SIEMUserEntry entity)
        {
            dbEntity.Username = entity.Username;

            _context.SaveChanges();
        }

        public void Delete(SIEMUserEntry entity)
        {
            _context.Users.Remove(entity);
            _context.SaveChanges();
        }

        public int Count()
        {
            return _context.Users.Count();
        }

        public PagedResult<SIEMUserEntry> GetPagedList(int page, int pageSize)
        {
            return _context.Users.GetPaged(page, pageSize);
        }

    }
}

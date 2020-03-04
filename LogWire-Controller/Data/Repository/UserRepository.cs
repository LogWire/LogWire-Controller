using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogWire.Controller.Data.Context;
using LogWire.Controller.Data.Model;

namespace LogWire.Controller.Data.Repository
{
    public class UserRepository : IDataRepository<UserEntry>
    {

        readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<UserEntry> GetAll()
        {
            return _context.Users.ToList();
        }

        public UserEntry Get(string key)
        {
            return _context.Users
                .FirstOrDefault(e => e.Username.Equals(key));
        }

        public void Add(UserEntry entity)
        {
            _context.Users.Add(entity);
            _context.SaveChanges();
        }

        public void Update(UserEntry dbEntity, UserEntry entity)
        {
            dbEntity.Email = entity.Email;
            dbEntity.FirstName = entity.FirstName;
            dbEntity.LastName = entity.LastName;
            dbEntity.Username = entity.Username;
            dbEntity.Password = entity.Password;

            _context.SaveChanges();
        }

        public void Delete(UserEntry entity)
        {
            _context.Users.Remove(entity);
            _context.SaveChanges();
        }

        public int Count()
        {
            return _context.Users.Count();
        }
    }
}

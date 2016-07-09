using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace CACSLibrary.Data
{
    public class EfTransaction : ITransaction
    {
        Database _db;
        DbContextTransaction _transaction;
        IDbContext _context;
        bool _isBegined;

        public EfTransaction(Database db, IDbContext context)
        {
            _db = db;
            _context = context;
        }

        public bool IsBegined
        {
            get { return _isBegined; }
        }

        public ITransaction BeginTransaction()
        {
            _transaction = _db.BeginTransaction();
            _isBegined = true;
            return this;
        }

        public void Commit()
        {
            if (!IsBegined) return;
            _context.SaveChanges();
            _transaction.Commit();
            _isBegined = false;
        }

        public void Dispose()
        {
            if (!IsBegined) return;
            _transaction.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Rollback()
        {
            if (!IsBegined) return;
            _transaction.Rollback();
            _isBegined = false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CACSLibrary.Data
{
    public interface ITransaction : IDisposable
    {
        bool IsBegined { get; }

        ITransaction BeginTransaction();

        void Commit();

        void Rollback();
    }
}

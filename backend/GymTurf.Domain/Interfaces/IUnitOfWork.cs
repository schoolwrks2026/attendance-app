using System;
using System.Data;
using System.Threading.Tasks;

namespace GymTurf.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IDbConnection Connection { get; }
    IDbTransaction? Transaction { get; }
    void BeginTransaction();
    void Commit();
    void Rollback();
}

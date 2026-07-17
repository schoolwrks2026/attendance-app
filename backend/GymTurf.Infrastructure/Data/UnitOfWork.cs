using System;
using System.Data;
using Npgsql;
using GymTurf.Domain.Interfaces;

namespace GymTurf.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly string _connectionString;
    private IDbConnection? _connection;
    private IDbTransaction? _transaction;

    public UnitOfWork(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection Connection
    {
        get
        {
            if (_connection == null)
            {
                _connection = new NpgsqlConnection(_connectionString);
                _connection.Open();
            }
            else if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            return _connection;
        }
    }

    public IDbTransaction? Transaction => _transaction;

    public void BeginTransaction()
    {
        if (_transaction != null) return;
        _transaction = Connection.BeginTransaction();
    }

    public void Commit()
    {
        _transaction?.Commit();
        _transaction = null;
    }

    public void Rollback()
    {
        _transaction?.Rollback();
        _transaction = null;
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _connection?.Dispose();
        _transaction = null;
        _connection = null;
    }
}

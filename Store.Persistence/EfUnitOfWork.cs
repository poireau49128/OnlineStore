using Microsoft.EntityFrameworkCore.Storage;
using Store.Persistence;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    private IDbContextTransaction? _transaction;

    public EfUnitOfWork(AppDbContext db)
    {
        _db = db;
    }

    public async Task BeginAsync()
    {
        _transaction = await _db.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction == null)
            throw new InvalidOperationException("Transaction not started");

        await _db.SaveChangesAsync();
        await _transaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
            await _transaction.RollbackAsync();
    }
}

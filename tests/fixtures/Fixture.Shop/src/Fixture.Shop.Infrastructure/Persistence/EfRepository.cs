using Fixture.Shop.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fixture.Shop.Infrastructure.Persistence;

public sealed class EfRepository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    private readonly ShopDbContext _dbContext;

    public EfRepository(ShopDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<TEntity?> FindAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Set<TEntity>().FindAsync([id], cancellationToken).AsTask();
    }
}

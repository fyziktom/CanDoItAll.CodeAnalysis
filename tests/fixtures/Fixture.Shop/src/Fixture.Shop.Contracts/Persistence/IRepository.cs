namespace Fixture.Shop.Contracts.Persistence;

public interface IRepository<TEntity>
    where TEntity : class
{
    Task<TEntity?> FindAsync(int id, CancellationToken cancellationToken = default);
}

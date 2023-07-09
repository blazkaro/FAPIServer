using Microsoft.EntityFrameworkCore;

namespace FAPIServer.Storage.EntityFramework.Services;

public class ChangesTracker<TEntity, TDbContext> : IChangesTracker<TEntity>
    where TEntity : class
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;

    public ChangesTracker(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async void BeginTracking(TEntity model)
    {
        if (_dbContext.Set<TEntity>().Entry(model).State == EntityState.Detached)
            _dbContext.Set<TEntity>().Attach(model);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

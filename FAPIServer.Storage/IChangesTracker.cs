namespace FAPIServer.Storage;

public interface IChangesTracker<TModel>
    where TModel : class
{
    void BeginTracking(TModel model);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

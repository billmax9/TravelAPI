namespace WebAPI.Services;

public interface IBaseService<T>
{

    Task<IEnumerable<T>> findAllAsync();

    Task<T?> findByIdAsycn(long id);
    
    Task<bool> deleteByIdAsync(long id);

}
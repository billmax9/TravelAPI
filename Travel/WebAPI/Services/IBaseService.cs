namespace WebAPI.Services;

public interface IBaseService<T>
{

    Task<IEnumerable<T>> FindAllAsync();

    Task<T?> FindByIdAsync(long id);

}
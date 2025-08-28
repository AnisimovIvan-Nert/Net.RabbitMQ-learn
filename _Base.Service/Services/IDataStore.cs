namespace Base.Service.Services;

public interface IDataStore<in T>
{
    ValueTask AddAsync(T data);
}
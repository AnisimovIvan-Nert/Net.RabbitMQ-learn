namespace Base.Service.Services;

public interface IDataSource<T>
{
    ValueTask<IEnumerable<T>> Pull();
}
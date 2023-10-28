namespace PoolingModule
{
    public interface IPool<T> where T :IPoolable
    {
        T Pull(object[] args = null);

        void Push(IPoolable p);
    }
}
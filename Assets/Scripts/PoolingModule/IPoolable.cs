using System;

namespace PoolingModule
{
    public interface IPoolable
    {
        event Action<IPoolable> ReturnToPoolEvent;
        
        void Activate();
        void Disable();
    }
}
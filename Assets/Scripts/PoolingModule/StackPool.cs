using System;
using System.Collections.Generic;

namespace PoolingModule
{
    public class StackPool<T> : IPool<T> where T : IPoolable
    {
        private readonly Stack<IPoolable> _objectPool;

        private readonly Func<object[], T> _createAction;
        
        public StackPool(Func<object[], T> createAction, int initialPoolSize = 0)
        {
            _createAction = createAction;
            _objectPool = new Stack<IPoolable>();
            
            CreatePool(initialPoolSize);
        }
        
        public T Pull(object[] args = null)
        {
            IPoolable p = _objectPool.Count > 0 
                ? _objectPool.Pop() 
                : Create(args);
            
            p.Activate();

            return (T)p;
        }

        public void Push(IPoolable p)
        {
            _objectPool.Push(p);
        }
        
        private void CreatePool(int size)
        {
            for (int index = 0; index < size; index++)
            {
                T instance = Create();
                Push(instance);
            }
        }

        private T Create(object[] args = null)
        {
            T t = _createAction.Invoke(args);
            t.ReturnToPoolEvent += Push;

            return t;
        }
    }
}
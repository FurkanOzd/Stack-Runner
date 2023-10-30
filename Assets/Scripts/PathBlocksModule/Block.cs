using System;
using UnityEngine;
using Zenject;
using IPoolable = PoolingModule.IPoolable;

namespace PathBlocksModule
{
    public abstract class Block : MonoBehaviour, IPoolable
    {
        public event Action<IPoolable> ReturnToPoolEvent;

        public abstract void Construct(BlockSpawnOptions blockSpawnOptions);
        
        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public virtual void Disable()
        {
            gameObject.SetActive(false);
            ReturnToPoolEvent?.Invoke(this);
        }

        public enum BlockType
        {
            SlidingBlock,
            BrokenBlock,
            FinishBlock,
        }

        public class Factory : PlaceholderFactory<object[], Block>{};
    }
}
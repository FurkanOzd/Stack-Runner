using System;
using DG.Tweening;
using UnityEngine;
using Zenject;
using IPoolable = PoolingModule.IPoolable;

namespace PathBlocksModule
{
    public class Block : MonoBehaviour, IPoolable
    {
        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private MeshRenderer _meshRenderer;

        private Vector3 _movePosition;

        private BlockType _blockType;

        private float _moveDuration;
        
        public event Action<IPoolable> ReturnToPoolEvent;

        public void Construct(float moveDuration, Material material, BlockType blockType)
        {
            _meshRenderer.material = material;
            _blockType = blockType;
            _moveDuration = moveDuration;
            
            Action action = _blockType == BlockType.Solid ? MoveToDirection : Fall;
            action.Invoke();
        }

        private void MoveToDirection()
        {
            Vector3 movePosition = transform.position;
            movePosition.x = -movePosition.x;
            transform.DOMoveX(movePosition.x, _moveDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }

        private void Fall()
        {
            _rigidbody.isKinematic = false;
        }

        public void Broke(float xScaleFactor)
        {
            if (xScaleFactor <= 0f)
            {
                Fall();
                return;
            }
            
            Vector3 scale = transform.localScale;
            scale.x = xScaleFactor;

            transform.localScale = scale;

            transform.DOKill();
        }

        public void Stop()
        {
            transform.DOKill();
        }

        private void OnTriggerExit(Collider other)
        {
            Disable();
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
            transform.DOKill();
            _rigidbody.isKinematic = true;
            
            ReturnToPoolEvent?.Invoke(this);
        }

        public enum BlockType
        {
            Solid,
            Broken,
        }

        public class Factory : PlaceholderFactory<BlockSpawnOptions, Block>{};
    }
}
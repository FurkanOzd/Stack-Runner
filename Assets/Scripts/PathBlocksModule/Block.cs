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

        private int _moveDirection;

        private bool _isFitted;
        
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
            transform.DOKill();
        }

        public bool Broke(float xScaleFactor, bool upper)
        {
            if (xScaleFactor <= 0f)
            {
                Fall();
                return false;
            }
            
            Vector3 scale = transform.localScale;
            Vector3 offset = Vector3.right * (transform.localScale.x - xScaleFactor) / 2;
            scale.x = xScaleFactor;
            
            transform.DOKill();
            
            transform.localScale = scale;
            transform.position += upper ? -offset : offset;

            _isFitted = true;

            return true;
        }

        public void Stack(Block previousBlock)
        {
            transform.DOKill();
            
            _isFitted = true;
            
            Vector3 position = transform.position;
            position.x = previousBlock.transform.position.x;
            transform.position = position;
        }

        private void OnTriggerExit(Collider other)
        {
            if (_isFitted)
            {
                Disable();
            }
        }

        public void Activate()
        {
            _isFitted = false;
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
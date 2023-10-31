using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace PathBlocksModule
{
    public class SlidingBlock : Block
    {
        [SerializeField]
        private Rigidbody _rigidbody;
        
        [SerializeField]
        private MeshRenderer _meshRenderer;

        private Vector3 _movePosition;
        
        private float _moveDuration;
        
        public override void Construct(BlockSpawnOptions blockSpawnOptions)
        {
            _meshRenderer.material = blockSpawnOptions.Material;
            _moveDuration = blockSpawnOptions.MoveDuration;
            
            Transform blockTransform = transform;
            blockTransform.SetParent(blockSpawnOptions.ParentTransform);

            Vector3 scale = blockTransform.localScale;
            scale.x = blockSpawnOptions.ScaleFactorX;
            
            blockTransform.localScale = scale;
            blockTransform.position = blockSpawnOptions.Position;

            MoveToDirection();
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
            DisableAsync();
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
            
            return true;
        }

        public void Stack(Transform previousBlockTransform)
        {
            transform.DOKill();
            
            Vector3 position = transform.position;
            position.x = previousBlockTransform.position.x;
            transform.position = position;
            transform.localScale = previousBlockTransform.localScale;
        }

        public override void Disable()
        {
            transform.DOKill();
            base.Disable();
        }

        private async void DisableAsync()
        {
            await Task.Delay(3000);
            Disable();
        }

        public override void Activate()
        {
            _rigidbody.isKinematic = true;
            base.Activate();
        }
    }
}
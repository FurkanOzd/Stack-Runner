using System.Threading;
using Cysharp.Threading.Tasks;
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
        
        private CancellationTokenSource _cancellationTokenSource;

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
            DisableAsync().Forget();
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
        
        private async UniTaskVoid DisableAsync()
        {
            const float disableDelay = 3f;
            
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            
            await UniTask.Delay(disableDelay, cancellationToken:_cancellationTokenSource.Token);
            
            Disable();
        }

        public override void Disable()
        {
            transform.DOKill();
            base.Disable();
        }

        public override void Activate()
        {
            _rigidbody.isKinematic = true;
            base.Activate();
        }
        
        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PathBlocksModule
{
    public class BrokenBlock : Block
    {
        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private MeshRenderer _meshRenderer;

        [SerializeField]
        private LayerMask _failLayer;

        private CancellationTokenSource _cancellationTokenSource;
        
        public override void Construct(BlockSpawnOptions blockSpawnOptions)
        {
            _meshRenderer.material = blockSpawnOptions.Material;

            Transform blockTransform = transform;
            blockTransform.SetParent(blockSpawnOptions.ParentTransform);

            Vector3 scale = blockTransform.localScale;
            scale.x = blockSpawnOptions.ScaleFactorX;
            
            blockTransform.localScale = scale;
            blockTransform.position = blockSpawnOptions.Position;
            
            Fall();
        }

        private void Fall()
        {
            _rigidbody.isKinematic = false;
            
            DisableAsync().Forget();
        }

        private async UniTaskVoid DisableAsync()
        {
            const float disableDelay = 3f;
            
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            
            await UniTask.Delay(disableDelay, cancellationToken:_cancellationTokenSource.Token);
            
            Disable();
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
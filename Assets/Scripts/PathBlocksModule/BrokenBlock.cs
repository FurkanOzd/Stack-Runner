using System.Threading.Tasks;
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
            
            DisableAsync();
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
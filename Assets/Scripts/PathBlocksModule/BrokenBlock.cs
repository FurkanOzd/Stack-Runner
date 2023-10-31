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
        }

        private void OnTriggerExit(Collider collider)
        {
            if ((collider.gameObject.layer & (1 << _failLayer)) != 0)
            {
               DisableAsync();
            }
        }

        private async void DisableAsync()
        {
            await Task.Delay(3000);
            Disable();
        }
    }
}
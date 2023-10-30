using GameManagementModule;
using Signals;
using UnityEngine;
using Zenject;

namespace PathBlocksModule
{
    public class FinishBlock : Block
    {
        [Inject]
        private SignalBus _signalBus;
        
        public override void Construct(BlockSpawnOptions blockSpawnOptions)
        {
            transform.SetParent(blockSpawnOptions.ParentTransform);
            transform.position = blockSpawnOptions.Position;
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            _signalBus.Fire(new GameStateChangedSignal(GameState.ReachToFinalPlatform));
        }
    }
}
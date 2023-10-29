using GameManagementModule;

namespace Signals
{
    public readonly struct GameStateChangedSignal
    {
        public readonly GameState GameState;

        public GameStateChangedSignal(GameState gameState)
        {
            GameState = gameState;
        }
    }
}
namespace Signals
{
    public readonly struct BlockFitSignal
    {
        public readonly bool IsSuccessFul;

        public BlockFitSignal(bool isSuccessFul)
        {
            IsSuccessFul = isSuccessFul;
        }
    }
}
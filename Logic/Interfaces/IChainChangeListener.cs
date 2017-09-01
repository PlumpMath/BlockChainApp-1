using Logic.Entitites;

namespace Logic.Interfaces
{
    public interface IChainChangeListener
    {
        void NewChainAdded(Chain chain);
    }
}
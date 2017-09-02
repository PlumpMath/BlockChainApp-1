namespace Logic.Interfaces
{
    public interface IObserverable
    {
        /// <summary>
        /// Установка "слушателя" событий биржи
        /// </summary>
        void SetChainChangeListener(IObserver listener);
    }
}
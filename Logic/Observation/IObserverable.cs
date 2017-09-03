namespace Logic.Observation
{
    public interface IObserverable
    {
        /// <summary>
        /// Установка "слушателя" событий биржи
        /// </summary>
        void SetChainChangeListener(IObserver listener);
    }
}
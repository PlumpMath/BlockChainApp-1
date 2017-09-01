namespace Logic.Interfaces
{
    public interface IFabric<TEntity>
    {
        TEntity GenerateEntity(int seed = 0);
    }
}
namespace Utils.Interfaces
{
    public interface IPoolable
    {
        IPool Pool { set; }

        string PoolId { get; }

    }
    
}
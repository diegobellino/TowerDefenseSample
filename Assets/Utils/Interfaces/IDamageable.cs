namespace Utils.Interfaces
{
    public interface IDamageable
    {
        float Health { get; }
        void TakeDamage(float amount);
    }
}

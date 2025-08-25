namespace Interfaces
{
    public interface IKnightEnemy
    {
        public float GetHealth();
        public void SetHealth(float newHealth);
        public void Hurt(float damageValue);
        public string GetName();
        public int GetLevel();
    }
}

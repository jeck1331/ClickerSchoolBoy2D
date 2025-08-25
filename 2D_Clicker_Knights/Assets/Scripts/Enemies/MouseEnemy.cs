using Enemies;
using Interfaces;

public class MouseEnemy: BaseEnemy, IKnightEnemy
{
    private string _name = "Mouse";
    private float _health = 100f;
    private int _level = 1;

    float IKnightEnemy.GetHealth()
    {
        return _health;
    }

    void IKnightEnemy.SetHealth(float newHealth)
    {
        _health = newHealth;
    }

    void IKnightEnemy.Hurt(float damageValue)
    {
        _health -= damageValue;
    }

    string IKnightEnemy.GetName()
    {
        return _name;
    }

    int IKnightEnemy.GetLevel()
    {
        return _level;
    }
}

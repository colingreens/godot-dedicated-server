using Godot;
using System;

public partial class HealthComponent : Node3D
{
    [Export(PropertyHint.Range, "0,200")] public int MaxHealth { get; set; } = 100;
    [Export(PropertyHint.Range, "0,30")] public int GibAt { get; set; } = -10;
    [Export] public bool verbose = false;

    public Action Died;
    public Action Healed;
    public Action Damaged;
    public Action Gibbed;
    public Action<int, int> HealthChanged;

    private int _currentHealth;

    public override void _Ready()
    {
        _currentHealth = MaxHealth;
        HealthChanged?.Invoke(_currentHealth, MaxHealth);
        if (verbose)
        {
            GD.Print($"Starting Health {_currentHealth} / {MaxHealth}");
        }
    }

    public void Hurt(DamageData damage)
    {
        if (_currentHealth <= 0)
        {
            return;
        }

        _currentHealth -= damage.Amount;

        if (_currentHealth <= GibAt)
        {
            Gibbed?.Invoke();
        }

        if (_currentHealth <= 0)
        {
            Died?.Invoke();
        }
        else
        {
            Damaged?.Invoke();
        }

        HealthChanged?.Invoke(_currentHealth, MaxHealth);

        if (verbose)
        {
            GD.Print($"Damaged for {damage.Amount}. Health {_currentHealth} / {MaxHealth}");
        }
    }

    public void Heal(int amount)
    {
        if (_currentHealth <= 0)
        {
            return;
        }

        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, MaxHealth);
        Healed?.Invoke();
        HealthChanged?.Invoke(_currentHealth, MaxHealth);
        if (verbose)
        {
            GD.Print($"Healed for {amount}. Health {_currentHealth} / {MaxHealth}");
        }
    }
}

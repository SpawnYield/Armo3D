using System;
using UnityEngine;

public class Humanoid : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 1000f; // Максимальное здоровье, видимое в инспекторе
    public float MaxHealth
    {
        get => maxHealth;
        set
        {
            if (maxHealth != value) // Проверяем, изменилось ли значение
            {
                maxHealth = value;
                OnMaxHealthChanged?.Invoke(); // Вызываем событие при изменении
            }
        }
    }
    private float health; // Текущее здоровье (скрытое поле)
    public float Health
    {
        get => health;
        set
        {
            if (health != value) // Проверяем, изменилось ли значение
            {
                health = Mathf.Clamp(value, 0, maxHealth); // Ограничиваем здоровье в пределах [0, maxHealth]
                OnTakeDamaged?.Invoke(); // Вызываем событие при изменении
            }
        }
    }

    [SerializeField]
    private float maxStamina = 1000f; // Максимальная выносливость

    private float stamina; // Текущая выносливость

    public float MaxStamina
    {
        get => maxStamina;
        set
        {
            if (maxStamina != value)
            {
                maxStamina = value;
                OnMaxStaminaChanged?.Invoke(); // Событие для изменения выносливости
            }
        }
    }

    public float Stamina
    {
        get => stamina;
        set => stamina = Mathf.Clamp(value, 0, maxStamina); // Ограничиваем выносливость в пределах [0, maxStamina]
    }

    [SerializeField]
    private float maxMana = 1000f; // Максимальная мана

    private float mana; // Текущая мана

    public float MaxMana
    {
        get => maxMana;
        set
        {
            if (maxMana != value)
            {
                maxMana = value;
                OnMaxManaChanged?.Invoke(); // Событие для изменения маны
            }
        }
    }

    public float Mana
    {
        get => mana;
        set => mana = Mathf.Clamp(value, 0, maxMana); // Ограничиваем ману в пределах [0, maxMana]
    }
    [SerializeField,Range(0f,100f)]
    private float walkSpeed = 1f; // Максимальное здоровье, видимое в инспекторе
    public float WalkSpeed
    {
        get => walkSpeed;
        set
        {
            if (walkSpeed != value)
            {
                walkSpeed = Mathf.Clamp(value,0,100);
                OnSpeedChanged?.Invoke(); // Событие для изменения маны
            }
        }
    }

    public event Action OnDied;
    public event Action OnTakeDamaged;
    public event Action OnMaxHealthChanged;
    public event Action OnMaxStaminaChanged;
    public event Action OnMaxManaChanged;
    public event Action OnSpeedChanged;

    public Humanoid Killer { get; private set; }
    public bool Died { get; private set; }

    public string Name = "Humanoid";

    public void TakeDamage(float damage, Humanoid killer)
    {
        if (Died || Health <= 0 || killer == null)
        {
            return;
        }
        Killer = killer;
        Health -= damage; // Уменьшаем здоровье
        if (Health <= 0)
        {
            Died = true;
            Debug.Log($"{Name} умер от {Killer.name}");
            OnDied?.Invoke();
        }
    }
    private void Start()
    {
        Health = MaxHealth;
        Stamina = MaxStamina;
        Mana = MaxMana;
    }
}

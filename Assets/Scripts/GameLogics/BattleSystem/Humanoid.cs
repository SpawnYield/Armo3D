using System;
using UnityEngine;

public class Humanoid : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 1000f; // ������������ ��������, ������� � ����������
    public float MaxHealth
    {
        get => maxHealth;
        set
        {
            if (maxHealth != value) // ���������, ���������� �� ��������
            {
                maxHealth = value;
                OnMaxHealthChanged?.Invoke(); // �������� ������� ��� ���������
            }
        }
    }
    private float health; // ������� �������� (������� ����)
    public float Health
    {
        get => health;
        set
        {
            if (health != value) // ���������, ���������� �� ��������
            {
                health = Mathf.Clamp(value, 0, maxHealth); // ������������ �������� � �������� [0, maxHealth]
                OnTakeDamaged?.Invoke(); // �������� ������� ��� ���������
            }
        }
    }

    [SerializeField]
    private float maxStamina = 1000f; // ������������ ������������

    private float stamina; // ������� ������������

    public float MaxStamina
    {
        get => maxStamina;
        set
        {
            if (maxStamina != value)
            {
                maxStamina = value;
                OnMaxStaminaChanged?.Invoke(); // ������� ��� ��������� ������������
            }
        }
    }

    public float Stamina
    {
        get => stamina;
        set => stamina = Mathf.Clamp(value, 0, maxStamina); // ������������ ������������ � �������� [0, maxStamina]
    }

    [SerializeField]
    private float maxMana = 1000f; // ������������ ����

    private float mana; // ������� ����

    public float MaxMana
    {
        get => maxMana;
        set
        {
            if (maxMana != value)
            {
                maxMana = value;
                OnMaxManaChanged?.Invoke(); // ������� ��� ��������� ����
            }
        }
    }

    public float Mana
    {
        get => mana;
        set => mana = Mathf.Clamp(value, 0, maxMana); // ������������ ���� � �������� [0, maxMana]
    }
    [SerializeField,Range(0f,100f)]
    private float walkSpeed = 1f; // ������������ ��������, ������� � ����������
    public float WalkSpeed
    {
        get => walkSpeed;
        set
        {
            if (walkSpeed != value)
            {
                walkSpeed = Mathf.Clamp(value,0,100);
                OnSpeedChanged?.Invoke(); // ������� ��� ��������� ����
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
        Health -= damage; // ��������� ��������
        if (Health <= 0)
        {
            Died = true;
            Debug.Log($"{Name} ���� �� {Killer.name}");
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

using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerVitalStats : MonoBehaviour
{
    [Header("vital stats")] [SerializeField]
    private float maxHealth;

    [SerializeField] private float maxStamina;
    [SerializeField] private float maxEnergy;
    [SerializeField] private float energyDecayPerMinute = 1f;

    [Header("FoodStats")] [SerializeField] private float maxProtein;
    [SerializeField] private float maxCarb;
    [SerializeField] private float maxFat;
    [SerializeField] private float maxHydration;

    private float _currentProtein;
    private float _currentCarb;
    private float _currentFat;
    private float _currentHydration;

    private int energyDecayTimer = 0;
    [SerializeField] private float proteinDecayTimer = 1;
    [SerializeField] private float carbDecayTimer = 2;
    [SerializeField] private float fatDecayTimer = .5f;
    [SerializeField] private float hydrationDecayTimer = 1;
    private float _currentEnergy;


    [SerializeField] private float staminaDrainRate;
    [SerializeField] private float staminaRegenRate;
    private float _currentStamina;

    private PlayerUI _playerUI;
    [Header("Dynamic values")] private float _currentHealth;

    private bool _isStaminaDrain;

    private MovementHandler _movementHandler;

    private void Awake()
    {
        _movementHandler = GetComponent<MovementHandler>();
        _playerUI = GetComponent<PlayerUI>();
        _currentEnergy = maxEnergy;
        _currentHealth = maxHealth;
        _currentStamina = maxStamina;
        _currentHydration = maxHydration;
        _currentProtein = maxProtein;
        _currentCarb = maxCarb;
        _currentFat = maxFat;
    }

    private void OnEnable()
    {
        EventBus.On5SecondsPassed += UpdateStats;
        EventBus.On5SecondsPassed += UpdateHealth;
    }

    private void OnDisable()
    {
        EventBus.On5SecondsPassed -= UpdateStats;
        EventBus.On5SecondsPassed -= UpdateHealth;
    }

    private void LateUpdate()
    {
        if (!_movementHandler._isSprinting)
        {
            UpdateStamina();
        }
    }

    private void UpdateStats()
    {
        energyDecayTimer += 5;
        if (energyDecayTimer >= 60)
        {
            _currentEnergy -= energyDecayPerMinute;
            _currentEnergy = Mathf.Clamp(_currentEnergy, 0f, maxEnergy);

            _currentProtein -= proteinDecayTimer;
            _currentProtein = Mathf.Clamp(_currentProtein, 0f, maxProtein);
            _currentCarb -= carbDecayTimer;
            _currentCarb = Mathf.Clamp(_currentCarb, 0f, maxCarb);
            _currentFat -= fatDecayTimer;
            _currentFat = Mathf.Clamp(_currentCarb, 0f, maxCarb);
            _currentHydration -= hydrationDecayTimer;
            _currentHydration = Mathf.Clamp(_currentHydration, 0f, maxHydration);
            // if all food bars are zero player starts starving and 
            _playerUI.UpdateFoodStats(_currentProtein / maxProtein, _currentCarb / maxCarb, _currentFat / maxFat,
                _currentHydration / maxHydration);

            _playerUI.EnergySlider(_currentEnergy / maxEnergy);

            energyDecayTimer = 0;
        }

        if (_currentStamina > _currentEnergy)
        {
            _currentStamina = _currentEnergy; // shrink stamina max when energy drops
            _playerUI.StaminaSlider(_currentStamina / maxStamina);
        }

        if (_currentEnergy <= 0)
        {
            // currently there is no impact on health 
            // Sleep();
        }
    }

    public void RestoreEnergy(float amount)
    {
        _currentEnergy = Mathf.Clamp(_currentEnergy + amount, 0f, maxEnergy);
        if (_currentStamina < _currentEnergy)
            _currentStamina = _currentEnergy;
    }

    public void DrainStamina(float dt)
    {
        _currentStamina -= staminaDrainRate * dt;
        _currentStamina = Mathf.Clamp(_currentStamina, 0f, _currentEnergy);
        _playerUI.StaminaSlider(_currentStamina / maxStamina);
    }

    private void UpdateStamina()
    {
        if (_currentStamina < _currentEnergy)
        {
            _currentStamina += staminaRegenRate;
            _currentStamina = Mathf.Clamp(_currentStamina, 0f, _currentEnergy);
            _playerUI.StaminaSlider(_currentStamina / maxStamina);
        }
    }

    private void UpdateHealth() // this funcion will update health from nutrient 
    {
        float p = _currentProtein / maxProtein;
        float c = _currentCarb / maxCarb;
        float h = _currentHydration / maxHydration;
        float f = _currentFat / maxFat;

        float avg = (p + c + h + f) / 4;
        _currentHealth = avg * maxHealth;

        _playerUI.HealthSlider(_currentHealth / maxHealth);
    }

    public void ConsumeFood(FoodSo so)
    {
        if (so == null) return;
        Debug.Log("ConsumeFood");
        // Add nutrients
        _currentProtein = Mathf.Clamp(_currentProtein + so.proteinCount, 0f, maxProtein);
        _currentCarb = Mathf.Clamp(_currentCarb + so.carbonCount, 0f, maxCarb);
        _currentFat = Mathf.Clamp(_currentFat + so.fatCount, 0f, maxFat);
        _currentHydration = Mathf.Clamp(_currentHydration + so.hydrationCount, 0f, maxHydration);

        _playerUI.EnergySlider(_currentEnergy / maxEnergy);
        _playerUI.StaminaSlider(_currentStamina / maxStamina);

        UpdateHealth();
        _playerUI.HealthSlider(_currentHealth / maxHealth);
    }

    public void DamageToHealth(float amount)
    {
        _currentHealth -= amount;
        _playerUI.HealthSlider(_currentHealth / maxHealth);
    }

    public void KillPlayer()
    {
        // Game Over
    }
}
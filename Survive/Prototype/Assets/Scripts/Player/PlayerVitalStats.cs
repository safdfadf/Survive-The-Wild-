using System;
using System.Collections;
using DefaultNamespace.EventBus;
using FoodSystem;
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
    private PlayerBody _playerBody;

    private bool _isSleeping = false;
    private float _originalTimeScale;
    [SerializeField] private float sleepTimeScale = 20f; // how fast time moves during sleep
    [SerializeField] private float energyRestorePerMinute = 5f;
    [SerializeField] private float dangerThreshold = 0.1f; // 10% of max nutrients
    [SerializeField] private float maxSleepHours = 8f; // configurable
    private float _sleepStartTime;

    private void Awake()
    {
        _movementHandler = GetComponent<MovementHandler>();
        _playerUI = GetComponent<PlayerUI>();
        _playerBody = GetComponent<PlayerBody>();
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
        EventManager.Instance.playerEvents.OnTriggerPlayerSleep += Sleep;
    }

    private void OnDisable()
    {
        EventBus.On5SecondsPassed -= UpdateStats;
        EventBus.On5SecondsPassed -= UpdateHealth;
        EventManager.Instance.playerEvents.OnTriggerPlayerSleep -= Sleep;
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
            Sleep();
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

    public void ConsumeFood(FoodConsumptionData so)
    {
        if (so == null) return;
        _currentProtein = Mathf.Clamp(_currentProtein + so.NutrientsCount.protien, 0f, maxProtein);
        _currentCarb = Mathf.Clamp(_currentCarb + so.NutrientsCount.carb, 0f, maxCarb);
        _currentFat = Mathf.Clamp(_currentFat + so.NutrientsCount.fat, 0f, maxFat);
        _currentHydration = Mathf.Clamp(_currentHydration + so.NutrientsCount.hydration, 0f, maxHydration);

        _playerUI.EnergySlider(_currentEnergy / maxEnergy);
        _playerUI.StaminaSlider(_currentStamina / maxStamina);

        UpdateHealth();
        _playerUI.HealthSlider(_currentHealth / maxHealth);
        if (so.SelfAttack.Effects != null)
            _playerBody.TakeDamage(so.SelfAttack);
    }

    public void DamageToHealth(float amount)
    {
        _currentHealth -= amount;
        _playerUI.HealthSlider(_currentHealth / maxHealth);
    }

    private void Sleep()
    {
        Debug.Log("trigger");
        if (_isSleeping) return;
        // update ui 

        _isSleeping = true;

        _originalTimeScale = TimeManager.Instance.timeScale;
        TimeManager.Instance.timeScale = (int)sleepTimeScale;
        StartCoroutine(SleepRoutine());
    }

    private IEnumerator SleepRoutine()
    {
        float maxSleepMinutes = maxSleepHours * 60f;

        float sleepEndTime = (_sleepStartTime + maxSleepMinutes) % 1440f;

        _playerUI.StartSleepUI(_sleepStartTime, sleepEndTime);
        while (_isSleeping)
        {
            _currentEnergy = Mathf.Clamp(
                _currentEnergy + energyRestorePerMinute * Time.deltaTime,
                0f, maxEnergy
            );

            if (_currentStamina < _currentEnergy)
                _currentStamina = _currentEnergy;

            _playerUI.EnergySlider(_currentEnergy / maxEnergy);
            _playerUI.StaminaSlider(_currentStamina / maxStamina);

            if (IsDangerState())
            {
                WakeUp();
                yield break;
            }


            if (_currentEnergy >= maxEnergy)
            {
                //notify player 
                UIManager.instance.DisplayNotification("Energy is full");
                WakeUp();
                yield break;
            }

            // Check max sleep duration
            float currentTime = TimeManager.Instance.GetTimeInMinutes();
            float sleptMinutes = (currentTime - _sleepStartTime + 1440f) % 1440f;
            _playerUI.SleepSlider.value = sleptMinutes;
            if (sleptMinutes >= maxSleepMinutes)
            {
                WakeUp();
                yield break;
            }

            yield return null;
        }
    }

    private bool IsDangerState()
    {
        float p = _currentProtein / maxProtein;
        float c = _currentCarb / maxCarb;
        float f = _currentFat / maxFat;
        float h = _currentHydration / maxHydration;

        float avg = (p + c + f + h) / 4f;

        bool avgLow = avg <= dangerThreshold;
        bool healthLow = (_currentHealth / maxHealth) <= dangerThreshold;

        return avgLow || healthLow;
    }

    public void WakeUp()
    {
        _isSleeping = false;


        TimeManager.Instance.timeScale = (int)_originalTimeScale;

        _playerUI.EnergySlider(_currentEnergy / maxEnergy);
        _playerUI.StaminaSlider(_currentStamina / maxStamina);
        _playerUI.EndSleepUI();
        Debug.Log("Player woke up");
    }

    public void KillPlayer()
    {
        // Game Over
    }
}
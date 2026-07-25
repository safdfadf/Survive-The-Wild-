using System;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject crosshair;
    [Header("Inventory")] [SerializeField] private GameObject invParent;
    [SerializeField] private Button rscInvButton;
    [SerializeField] private Button wpnInvButton;
    [SerializeField] private GameObject rscInventory;
    [SerializeField] private GameObject wpnInventory;

    [Header("Vital Stats")] [SerializeField]
    private Slider healthBar;

    [SerializeField] private Slider energyBar;
    [SerializeField] private Slider staminaBar;
    [Header("FoodStats")] [SerializeField] private Slider protienBar;
    [SerializeField] private Slider carbBar;
    [SerializeField] private Slider fatBar;
    [SerializeField] private Slider hydroBar;


    private List<GameObject> activeSoundUI = new();
    [SerializeField] private Image noiseImage;


    [Header("Compass System UI")] [SerializeField]
    private Image windDir;

    [SerializeField] private Image compassRing;

    [Header("Noise Scale Settings")] private readonly float _idleScale = 0f;
    private readonly float _crouchScale = 0.2f; // baseline when not moving
    private readonly float _walkScale = 0.4f; // max scale when walking
    private readonly float _sprintScale = 0.6f; // max scale when sprinting
    private readonly float _smoothSpeed = 2f; // responsiveness

    [Header("UI Control")] [SerializeField]
    private Button craftingButton;

    [SerializeField] private GameObject craftingUI;
    [SerializeField] private Button bodyStatButton;

    [SerializeField] private GameObject bodyStatusUI;

    // Script Reference  
    private PlayerNoiseEmitter _playerNoiseEmitter;
    private MovementHandler _movementHandler;
    private float currentScale;


    private void Awake()
    {
        _playerNoiseEmitter = GetComponent<PlayerNoiseEmitter>();
        _movementHandler = GetComponent<MovementHandler>();
        rscInvButton.onClick.AddListener(ShowRscInv);
        wpnInvButton.onClick.AddListener(ShowWpnInv);
        craftingButton.onClick.AddListener(ToggleSubSystems);
        bodyStatButton.onClick.AddListener(ToggleSubSystems);
        InitSliders();
    }


    private void Start()
    {
        rscInvButton.gameObject.SetActive(false);
        wpnInvButton.gameObject.SetActive(false);
        invParent.SetActive(false);
    }

    private void InitSliders()
    {
        healthBar.value = healthBar.maxValue;
        energyBar.value = energyBar.maxValue;
        staminaBar.value = staminaBar.maxValue;
        UpdateFoodStats(1, 1, 1, 1);
    }

    public void ToggleInventory()
    {
        if (invParent.gameObject.activeInHierarchy)
        {
            invParent.SetActive(false);
            rscInvButton.gameObject.SetActive(false);
            wpnInvButton.gameObject.SetActive(false);
        }
        else
        {
            invParent.SetActive(true);
            rscInvButton.gameObject.SetActive(true);
            wpnInvButton.gameObject.SetActive(true);
            ShowRscInv();
        }
    }

    public void ToggleSubSystems() // toggle crafting or body stats 
    {
        if (craftingButton.gameObject.activeInHierarchy)
        {
            craftingUI.SetActive(false);
            bodyStatusUI.SetActive(true);
        }
        else
        {
            craftingUI.SetActive(true);
            bodyStatusUI.SetActive(false);
        }
    }


    private void LateUpdate()
    {
        UpdateSoundUI();
    }

    public GameObject GetCrosshair()
    {
        return crosshair;
    }

    public bool IsInventoryOpen()
    {
        if (invParent.gameObject.activeInHierarchy)
        {
            return true;
        }

        return false;
    }

    private void ShowRscInv()
    {
        rscInventory.SetActive(true);
        wpnInventory.SetActive(false);
    }

    private void ShowWpnInv()
    {
        wpnInventory.SetActive(true);
        rscInventory.SetActive(false);
    }

    private void UpdateSoundUI()
    {
        float noiseValue = _playerNoiseEmitter.GetCurrentNoise();
        bool isWalking = _movementHandler._isWalking;
        bool isSprinting = _movementHandler._isSprinting;
        bool isCrouching = _movementHandler.IsCrouching;
        float targetScale = _idleScale;

        if (isWalking)
        {
            currentScale = _walkScale;
        }
        else if (isSprinting)
        {
            currentScale = _sprintScale;
        }
        else if (isCrouching)
        {
            currentScale = _crouchScale;
        }
        else
        {
            currentScale = _idleScale;
        }

        currentScale = Mathf.Lerp(currentScale, targetScale, Time.deltaTime * _smoothSpeed);

        noiseImage.rectTransform.localScale = new Vector3(currentScale, currentScale, 1f);
    }

    public void HealthSlider(float value) // can be made one function for all
    {
        healthBar.value = value;
    }

    public void EnergySlider(float value)
    {
        energyBar.value = value;
        StaminaSlider(energyBar.value);
    }

    public void StaminaSlider(float value)
    {
        staminaBar.value = value;
    }

    public void UpdateFoodStats(float protien, float carb, float fat, float hydro)
    {
        protienBar.value = protien;
        carbBar.value = carb;
        fatBar.value = fat;
        hydroBar.value = hydro;
    }
}
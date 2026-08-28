using System.Collections.Generic;
using DefaultNamespace.Interface;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Canvas worldCanvas;
    private List<GameObject> activeSoundUI = new();

    [SerializeField] private Image noiseImage;

    [Header("Compass System UI")] // this should be handled by player ui handler 
    [SerializeField]
    private Image windDir;

    [SerializeField] private Image compassRing;
    [SerializeField] private Button InteractButton;
    [Header("Cooking")] private List<Slot> cookingSlots = new();
    [SerializeField] private Slot cookingSpotUIPrefab;
    [Header("Object Menu")] public Button craftButton;
    public GameObject objectMenu;
    public GameObject firstMenu;
    public Button harvestButton;
    public Button removeButton;
    public Button useMe;
    public TextMeshProUGUI description;
    private IAction _currentTarget;
    private float currentScale;
    private List<Button> _activeButtons = new();
    private List<Button> _allButtons = new();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        ToggleInteractButton(false);
        _allButtons = new List<Button> { craftButton, harvestButton, useMe, removeButton };
        _activeButtons.Add(removeButton);
    }

    private void LateUpdate()
    {
        SetMainMenuPos();
        if (cookingSlots.Count != 0)
        {
            UpdateCookingSpots();
        }
    }

    public void SetAnimalUI(GameObject obj)
    {
        RectTransform rectTransform = worldCanvas.GetComponent<RectTransform>();
        obj.transform.SetParent(rectTransform, false);
        activeSoundUI.Add(obj);
    }

    public void ToggleSoundUI(bool toggle)
    {
        foreach (var UI in activeSoundUI)
        {
            UI.SetActive(toggle);
        }
    }

    public void UpdateWindDir(Quaternion quat)
    {
        windDir.rectTransform.localRotation = quat;
    }

    public void UpdateCompassRing(Quaternion quat)
    {
        compassRing.rectTransform.localRotation = quat;
    }

    public void ToggleInteractButton(bool toggle)
    {
        InteractButton.gameObject.SetActive(toggle);
    }


    public Vector2 WorldToCanvasPosition(Vector3 worldPos)
    {
        // when acrivate update them so they follow player movements 
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            worldCanvas.transform as RectTransform,
            screenPos,
            worldCanvas.worldCamera,
            out Vector2 canvasPos
        );

        return canvasPos;
    }

    public void UpdateCookingSpots()
    {
        for (int i = 0; i < cookingSlots.Count; i++)
        {
            Vector2 canvasPos = WorldToCanvasPosition(cookingSlots[i].worldPosition);
            cookingSlots[i].rect.anchoredPosition = canvasPos;
        }
    }

    public void DisplayCookingSpots(List<Transform> worldPositions, GameObject handler)
    {
        ClearAllCookingSpots();
        for (int i = 0; i < worldPositions.Count; i++)
        {
            Vector2 canvasPos = WorldToCanvasPosition(worldPositions[i].position);

            Slot uiSlot = Instantiate(cookingSpotUIPrefab, worldCanvas.transform);
            uiSlot.cookingData = new CookingData(SlotType.CookingSpot, handler);
            uiSlot.cookingSpotIndex = i;
            uiSlot.worldPosition = worldPositions[i].position;
            uiSlot.rect.anchoredPosition = canvasPos;
            cookingSlots.Add(uiSlot);
        }
    }

    public Slot GetCookingSlotUnderMouse(Vector3 screenPos)
    {
        foreach (var slot in cookingSlots)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(slot.rect, screenPos, worldCanvas.worldCamera))
            {
                return slot;
            }
        }

        return null;
    }

    public void ClearAllCookingSpots()
    {
        Debug.Log("clear cooking spots");
        foreach (var spot in cookingSlots)
            Destroy(spot.gameObject);

        cookingSlots.Clear();
    }

    public void ActivateUi(IAction action)
    {
        _currentTarget = action;
        if (_currentTarget == null) return;
        // Debug.Log(_currentObj.gameObject.name);
        if (_currentTarget.canCraft)
        {
            _activeButtons.Add(craftButton);
            craftButton.gameObject.SetActive(true);
            craftButton.onClick.AddListener(_currentTarget.Craft);
        }

        if (_currentTarget.canHarvest)
        {
            _activeButtons.Add(harvestButton);
            craftButton.onClick.AddListener(_currentTarget.Harvest);
        }

        if (_currentTarget.canUse)
        {
            _activeButtons.Add(useMe);
            TextMeshProUGUI textMesh = useMe.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.text = _currentTarget.useMeDescription;
            useMe.onClick.AddListener(() => _currentTarget.UseMe());
        }

        SetDescription();
        objectMenu.SetActive(true);
        DeactivateSubMenu();
    }

    private void SetMainMenuPos()
    {
        if (_currentTarget == null) return;
        Vector2 canvasPos = WorldToCanvasPosition(_currentTarget.obj.transform.position);
        RectTransform rect = objectMenu.GetComponent<RectTransform>();
        rect.anchoredPosition = canvasPos;
    }

    public void ActivateSubMenu()
    {
        if (!objectMenu.activeSelf || _currentTarget == null) return;
        firstMenu.SetActive(false);
        Debug.Log(_activeButtons.Count + " Activate");
        foreach (var buttons in _activeButtons)
        {
            buttons.gameObject.SetActive(true);
        }

        PlayerRepository.instance.CanPlayerMove(false);
        PlayerRepository.instance.ToggleCursor(true);
    }

    public void DeactivateSubMenu()
    {
        if (!objectMenu.activeSelf || _currentTarget == null) return;
        firstMenu.SetActive(true);
        Debug.Log("Deactivated");
        foreach (var buttons in _activeButtons)
        {
            buttons.gameObject.SetActive(false);
        }

        PlayerRepository.instance.CanPlayerMove(true);
        PlayerRepository.instance.ToggleCursor(false);
    }

    private void SetDescription()
    {
        if (_currentTarget == null) return;
        TextMeshProUGUI textMesh = useMe.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        textMesh.text = _currentTarget.useMeDescription;
        description.text = _currentTarget.Description;
    }

    public void DeactivateUi()
    {
        _currentTarget = null;
        objectMenu.SetActive(false);
        _activeButtons.Clear();
    }
}
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.Interface;
using DefaultNamespace.QuestSystem;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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

    [Header("Notification")] [SerializeField]
    private GameObject notificationParent;

    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float stayTime;
    [SerializeField] private int showPoint;
    [SerializeField] private int hidePoint;


    [Header("Mission Ui")] [SerializeField]
    private Transform missionUiParent;

    [SerializeField] private Image missionImage;
    [SerializeField] private TextMeshProUGUI tittle;
    [SerializeField] private TextMeshProUGUI questStepText;
    [SerializeField] private float missionDisplayPos;
    [SerializeField] private float missionRemovePos;
    [SerializeField] private float questDisplayPos;

    [FormerlySerializedAs("questTargetPos")] [SerializeField]
    private float questRemovePos;

    private QuestInfoSo currentMission;
    private QuestStep currentQuestStep;
    private RectTransform _missionParentRect;
    private RectTransform _questStepRect;

    private IInteractable _currentTarget;
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
        _missionParentRect = missionUiParent.GetComponent<RectTransform>();
        _questStepRect = questStepText.GetComponent<RectTransform>();
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
            uiSlot.ToggleAlpha(true);
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
        foreach (var spot in cookingSlots)
            Destroy(spot.gameObject);

        cookingSlots.Clear();
    }

    public void ActivateUi(IInteractable interactionUI)
    {
        _currentTarget = interactionUI;
        if (_currentTarget == null)
        {
            Debug.LogWarning("No interaction UI selected");
            return;
        }

        if (_currentTarget.CanCraft)
        {
            _activeButtons.Add(craftButton);
            craftButton.gameObject.SetActive(true);
            craftButton.onClick.AddListener(_currentTarget.Craft);
        }

        if (_currentTarget.CanHarvest)
        {
            _activeButtons.Add(harvestButton);
            harvestButton.onClick.AddListener(_currentTarget.Harvest);
        }

        if (_currentTarget.CanUse)
        {
            _activeButtons.Add(useMe);
            TextMeshProUGUI textMesh = useMe.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.text = _currentTarget.UseMeDescription;
            useMe.onClick.AddListener(() => _currentTarget.UseMe());
        }

        SetDescription();
        objectMenu.SetActive(true);
        DeactivateSubMenu();
    }

    private void SetMainMenuPos()
    {
        if (_currentTarget == null || _currentTarget.Gm == null) return;
        if (!_currentTarget.Gm.activeSelf) return;
        Vector2 canvasPos = WorldToCanvasPosition(_currentTarget.Gm.transform.position);
        RectTransform rect = objectMenu.GetComponent<RectTransform>();
        rect.anchoredPosition = canvasPos;
    }

    public void ActivateSubMenu()
    {
        if (!objectMenu.activeSelf || _currentTarget == null) return;
        firstMenu.SetActive(false);
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
        textMesh.text = _currentTarget.UseMeDescription;
        description.text = _currentTarget.Description;
    }

    public void DeactivateUi()
    {
        _currentTarget = null;
        objectMenu.SetActive(false);
        _activeButtons.Clear();
    }

    public void AddQuestName(List<QuestStep> questSteps)
    {
        /*
            foreach (var step in questSteps)
            {
                if (step.IsFinished) // update list
                {
                    if (questsUI.TryGetValue(step.StepName, out var value))
                        Destroy(value);
                }
                else
                {
                    var obj = Instantiate(questsText.gameObject, questUiParent);
                    var textMesh = obj.GetComponent<TextMeshProUGUI>();
                    textMesh.text = step.StepName;
                    questsUI.Add(step.StepName, textMesh);
                }
            }
        */
    }

    public void DisplayNotification(string discription)
    {
        notificationText.text = discription;
        StartCoroutine(ShowNotification());
    }

    private IEnumerator ShowNotification()
    {
        RectTransform rect = notificationParent.GetComponent<RectTransform>();
        Vector2 startPos = new Vector2(showPoint, rect.anchoredPosition.y);
        Vector2 targetPos = new Vector2(hidePoint, rect.anchoredPosition.y);


        rect.anchoredPosition = Vector2.zero;
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * .05f;
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
        }

        yield return new WaitForSeconds(stayTime);
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * .05f;
            rect.anchoredPosition = Vector2.Lerp(targetPos, startPos, t);
        }
    }

    public void SetNewMission(QuestInfoSo questSo, QuestStep currentStep)
    {
        StartCoroutine(NewMissionRoutine(questSo, currentStep));
    }

    private IEnumerator NewMissionRoutine(QuestInfoSo questSo, QuestStep currentStep)
    {
        if (currentMission != null && currentMission != questSo)
        {
            tittle.fontStyle = FontStyles.Strikethrough;
            questStepText.fontStyle = FontStyles.Strikethrough;
            tittle.text = questSo.missionName;
            yield return new WaitForSeconds(1f);
            ToggleMission(_missionParentRect, missionDisplayPos, missionRemovePos);
        }

        missionImage.sprite = questSo.missionSprite;
        tittle.text = questSo.missionName;
        questStepText.text = currentStep.StepName;
        tittle.fontStyle = FontStyles.Normal;
        questStepText.fontStyle = FontStyles.Normal;
        currentMission = questSo;
        currentQuestStep = currentStep;
        yield return new WaitForSeconds(.5f);
        ToggleMission(_missionParentRect, missionRemovePos, missionDisplayPos);
        yield return new WaitForSeconds(2f);
        ToggleMission(_missionParentRect, missionDisplayPos, missionRemovePos);
    }

    public void UpdateQuestStep(QuestStep newStep)
    {
        StartCoroutine(QuestStepRoutine(newStep));
    }

    private IEnumerator QuestStepRoutine(QuestStep newStep)
    {
        if (currentQuestStep != null && currentQuestStep != newStep)
        {
            questStepText.fontStyle = FontStyles.Strikethrough;
            yield return new WaitForSeconds(2f);
            ToggleMission(_questStepRect, questDisplayPos, questRemovePos);
        }

        currentQuestStep = newStep;
        questStepText.text = newStep.StepName;
        questStepText.fontStyle = FontStyles.Normal;
        ToggleMission(_questStepRect, questRemovePos, questDisplayPos);
    }


    private void ToggleMission(RectTransform rect, float start, float target)
    {
        Vector2 startPos = new();
        Vector2 targetPos = new();
        startPos = new Vector2(start, rect.position.y);
        targetPos = new Vector2(target, rect.position.y);

        rect.anchoredPosition = Vector2.zero;
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 1.5f;
            rect.position = Vector2.Lerp(startPos, targetPos, t);
        }
    }
}
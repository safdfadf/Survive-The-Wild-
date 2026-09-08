using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.EventBus;
using DefaultNamespace.QuestSystem;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RBookHandler : MonoBehaviour
{
    [Header("Tags")] [SerializeField] private Button questTag;
    [SerializeField] private Button weaponTag;
    [SerializeField] private Button baseBuildTage;
    [SerializeField] private Button trapsTag;
    [SerializeField] private Button fireTag;

    [SerializeField] private Transform page1;
    [SerializeField] private Transform page2;

    [Header("Arrows")] [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject previousButton;

    [Header("Prefabs")] [SerializeField] private GameObject recipiePrefab;
    [SerializeField] private GameObject questPrefab;
    [Header("ParentObj")] [SerializeField] private GameObject parentObj;

    private List<CraftingSO> _weaponRecipes;

    private List<CraftingSO> _baseRecipes;

    // private List<CraftingSO> trapRecipes;
    [Header("Crafting Handler")] private CraftingHandler _craftingHandler;

    private List<CraftingSO> activeList;
    private int currentIndex = 0;
    private List<GameObject> spawnedSlots = new();
    private Dictionary<string, TextMeshProUGUI> questTexts = new();
    List<TextMeshProUGUI> questTextList = new();

    private void Awake()
    {
        questTag.onClick.AddListener(ShowQuest);
        weaponTag.onClick.AddListener(ShowWeaponRecipe);
        baseBuildTage.onClick.AddListener(ShowBaseBuildRecipe);
        trapsTag.onClick.AddListener(ShowTrapsRecipe);
        fireTag.onClick.AddListener(ShowFireRecipe);
    }

    private void OnEnable()
    {
        EventManager.Instance.questEvent.onQuestComplete += CheckQuest;
    }

    private void OnDisable()
    {
        EventManager.Instance.questEvent.onQuestComplete -= CheckQuest;
    }

    public void ToggleRBook()
    {
        parentObj.SetActive(!parentObj.activeSelf);
        if (parentObj.activeSelf)
        {
            PlayerRepository.instance.CanPlayerMove(false);
            ShowWeaponRecipe();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            PlayerRepository.instance.CanPlayerMove(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void EnableRecipeBook()
    {
        parentObj.SetActive(true);
    }

    public void DisableRecipeBook()
    {
        parentObj.SetActive(false);
    }

    public void CategorizeRecipes(List<CraftingSO> allRecipes, CraftingHandler craftingHandler)
    {
        _craftingHandler = craftingHandler;
        _weaponRecipes = new List<CraftingSO>();
        //      trapRecipes = new List<CraftingSO>();
        _baseRecipes = new List<CraftingSO>();

        foreach (var so in allRecipes)
        {
            if (so == null)
            {
                Debug.Log("so is nul");
            }

            if (so.resSo == null)
            {
                Debug.Log("res so is null" + so);
            }

            if (so.resSo.prefab == null)
            {
                Debug.Log("resSo.prefab is null" + so.resSo);
            }

            GameObject prefab = so.resSo.prefab;
            if (prefab == null)
            {
                Debug.Log("prefab is null");
            }

            if (prefab.TryGetComponent<BaseWeapon>(out _))
            {
                _weaponRecipes.Add(so);
            }
            //   else if (prefab.TryGetComponent<Trap>(out _))
            //              trapRecipes.Add(so);
            else if (prefab.TryGetComponent<BaseStructure>(out _))
                _baseRecipes.Add(so);
        }
    }

    private void ShowRecipes()
    {
        foreach (var slot in spawnedSlots)
            Destroy(slot);
        spawnedSlots.Clear();

        Transform currentTransform;
        for (int i = 0; i < 4; i++)
        {
            currentTransform = i <= 1 ? page1 : page2;
            int index = currentIndex + i;
            if (index >= activeList.Count) break;

            GameObject r = Instantiate(recipiePrefab, currentTransform);
            r.GetComponent<Recipie>().Initialize(activeList[index], _craftingHandler);
            spawnedSlots.Add(r);
        }

        previousButton.SetActive(currentIndex > 0);
        nextButton.SetActive(currentIndex + 4 < activeList.Count);
    }

    public void NextPage()
    {
        if (currentIndex + 4 < activeList.Count)
        {
            currentIndex += 4;
            ShowRecipes();
        }
    }

    public void PreviousPage()
    {
        if (currentIndex > 0)
        {
            currentIndex -= 4;
            ShowRecipes();
        }
    }

    private void ShowWeaponRecipe()
    {
        activeList = _weaponRecipes;
        currentIndex = 0;
        ShowRecipes();
    }

    private void ShowBaseBuildRecipe()
    {
        Debug.Log("Show Base Build Recipe");
        activeList = _baseRecipes;
        currentIndex = 0;
        ShowRecipes();
    }

    private void ShowTrapsRecipe()
    {
    }

    private void ShowFireRecipe()
    {
    }

    private void ShowQuest()
    {
        foreach (var slot in spawnedSlots)// this will create an issue because it destroys objs that means text mesh obj created 
            spawnedSlots.Clear();

        Transform currentTransform;

        for (int i = 0; i < questTextList.Count; i++)
        {
            currentTransform = i <= 1 ? page1 : page2;

            int index = currentIndex + i;
            if (index >= questTextList.Count) break;
                
             var textMesh = questTextList[index];
             textMesh.gameObject.transform.SetParent(currentTransform);
             RectTransform rectTransform = textMesh.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = currentTransform.position;
            Debug.Log(textMesh.gameObject);
            textMesh.gameObject.SetActive(true);
            spawnedSlots.Add(textMesh.gameObject);
        }

        previousButton.SetActive(currentIndex > 0);
        nextButton.SetActive(currentIndex + 4 < questTextList.Count);
    }

    public void SetQuestSteps(List<QuestStep> questSteps)
    {
        foreach (var q in questSteps)
        {
            GameObject obj = Instantiate(questPrefab, gameObject.transform);
            var textMesh = obj.GetComponent<TextMeshProUGUI>();
            textMesh.text = q.StepName;
            questTexts.Add(q.QuestId, textMesh);
            questTextList.Add(textMesh);
            obj.SetActive(false);
        }

        questTextList = questTexts.Values.ToList();
        currentIndex = 0;
        ShowQuest();
    }

    //we need to store active id and text 
    private void CheckQuest(string id)
    {
        TextMeshProUGUI text = questTexts[id];
        text.fontStyle = FontStyles.Strikethrough;
    }
}
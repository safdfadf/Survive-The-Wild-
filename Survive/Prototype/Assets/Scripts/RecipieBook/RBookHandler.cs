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

    private List<RecipeData> _weaponRecipes;

    private List<RecipeData> _baseRecipes;

    // private List<CraftingSO> trapRecipes;
    [Header("Crafting Handler")] private CraftingHandler _craftingHandler;

    private List<RecipeData> activeList;
    private int currentIndex = 0;
    private List<GameObject> spawnedSlots = new();
    private Dictionary<string, List<TextMeshProUGUI>> questTexts = new();
    List<GameObject> questTextList = new();

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
        EventManager.Instance.questEvent.OnQuestStepComplete += CheckQuest;
    }

    private void OnDisable()
    {
        EventManager.Instance.questEvent.OnQuestStepComplete -= CheckQuest;
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

    public void CategorizeRecipes(List<RecipeData> data, CraftingHandler craftingHandler)
    {
        _craftingHandler = craftingHandler;
        _weaponRecipes = new List<RecipeData>();
        //      trapRecipes = new List<CraftingSO>();
        _baseRecipes = new List<RecipeData>();

        foreach (var r in data)
        {
            if (r == null)
            {
                Debug.Log("recipiedata is nul");
            }

            if (r.craftingSo == null)
            {
                Debug.Log("res so is null" + r);
            }

            if (r.craftingSo.resSo.prefab == null)
            {
                Debug.Log("resSo.prefab is null" + r.craftingSo);
            }

            GameObject prefab = r.craftingSo.resSo.prefab;
            if (prefab == null)
            {
                Debug.Log("prefab is null");
            }

            if (prefab.TryGetComponent<BaseWeapon>(out _))
            {
                _weaponRecipes.Add(r);
            }
            //   else if (prefab.TryGetComponent<Trap>(out _))
            //              trapRecipes.Add(so);
            else if (prefab.TryGetComponent<BaseStructure>(out _))
                _baseRecipes.Add(r);
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
            r.GetComponent<RecipeUI>().Initialize(activeList[index], _craftingHandler);
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

    private void RemoveLockedRecipes()
    {
        for (int i = activeList.Count - 1; i >= 0; i--)
        {
            if (activeList[i].isLocked)
            {
                activeList.RemoveAt(i);
            }
        }
    }

    private void ShowQuest() // instead of quest step i would like to show the whole mission 
    {
        foreach (var slot in
                 spawnedSlots) // this will create an issue because it destroys objs that means text mesh obj created 
            spawnedSlots.Clear();

        Transform currentTransform;

        foreach (var key in questTexts)
        {
            GameObject quest = Instantiate(questPrefab, parentObj.transform);
            var tittle = quest.GetComponent<TextMeshProUGUI>();
            tittle.text = key.Key;
            foreach (var text in key.Value)
            {
                text.gameObject.transform.SetParent(quest.transform);
            }

            questTextList.Add(quest);
        }

        for (int i = 0; i < questTextList.Count; i++) // based on no. of missions set their page 
        {
            currentTransform = i <= 1 ? page1 : page2;

            int index = currentIndex + i;
            if (index >= questTextList.Count) break;

            var questObj = questTextList[index];
            questObj.gameObject.transform.SetParent(currentTransform);
            RectTransform rectTransform = questObj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = currentTransform.position;
            questObj.gameObject.SetActive(true);
            spawnedSlots.Add(questObj.gameObject);
        }

        previousButton.SetActive(currentIndex > 0);
        nextButton.SetActive(currentIndex + 4 < questTextList.Count);
    }

    public void SetQuestSteps(List<QuestStep> questSteps)
    {
        questTextList.Clear();
        foreach (var q in questSteps)
        {
            if (questTexts.ContainsKey(q.QuestId))
            {
                var textMesh = GetNewTextMesh(q.StepName);
                questTexts[q.QuestId].Add(textMesh);
            }
            else
            {
                List<TextMeshProUGUI> t = new();
                var textMesh = GetNewTextMesh(q.StepName);
                RectTransform rt = textMesh.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(300, 50);
                t.Add(textMesh);
                questTexts.Add(q.QuestId, t);
            }
        }

        currentIndex = 0;
        ShowQuest();
    }

    private TextMeshProUGUI GetNewTextMesh(string id)
    {
        GameObject obj = new GameObject();
        var textMesh = obj.AddComponent<TextMeshProUGUI>();
        textMesh.text = id;
        return textMesh;
    }

    private void CheckQuest(string id)
    {
        // we dont just need 
        foreach (var obj in questTextList)
        {
            var textMesh = obj.GetComponent<TextMeshProUGUI>();
            if (textMesh.text == id)
            {
                //maybe an animation or something 
            }
        }
    }

    public void AddNewRecipe(RecipeData data)
    {
        GameObject prefab = data.craftingSo.resSo.prefab;
        if (prefab.TryGetComponent<BaseWeapon>(out _))
        {
            _weaponRecipes.Add(data);
        }
        //   else if (prefab.TryGetComponent<Trap>(out _))
        //              trapRecipes.Add(so);
        else if (prefab.TryGetComponent<BaseStructure>(out _))
            _baseRecipes.Add(data);
        // notify player 
    }
}
using System;
using System.Collections.Generic;
using DefaultNamespace.EventBus;
using Inventory;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CraftingHandler : MonoBehaviour
{
    [SerializeField] private List<CraftingSO> craftingSo;

    [FormerlySerializedAs("craftingUI")] [SerializeField]
    private RectTransform craftingUITransform;

    [Header("Testing")] [SerializeField] private List<CraftingSO> testingSo;


    private List<Ingredient> _currentIngredients = new();
    private Dictionary<ObjSo, List<GameObject>> _ingredientVisuals = new();
    private PlayerInventory _playerInventory;
    private ResourceInventory _resourceInventory;
    [SerializeField] private Vector3 tableOffset;

    [SerializeField] private Button craftButton;

    [Header("Recipe Book")] [SerializeField]
    private RBookHandler recipeBook;

    [Header("Early Access Recipe")] [SerializeField]
    private List<CraftingSO> earlyAccessRecipe = new();

    //TODo: Move recipe Book ui to PlayerUI script 
    public bool enableCrafting { get; set; }

    private int _ingredientVisualIndex = 0;

    private CraftingSO _currentSo;
    private BuildingHandler _buildingHandler;
    private List<InventoryItem> _currentItems = new();
    private List<RecipeData> _recipeData = new(); // all base recipes
    [SerializeField] private bool isTesting = false;

    private void OnEnable()
    {
        EventBus.OnCraftResource += AddIngredient;
        EventBus.OnUnCraftResource += RemoveResource;
        EventManager.Instance.questEvent.OnMissionComplete += UnLockRecipe;
    }

    private void OnDisable()
    {
        EventBus.OnCraftResource -= AddIngredient;
        EventBus.OnUnCraftResource -= RemoveResource;
        EventManager.Instance.questEvent.OnMissionComplete -= UnLockRecipe;
    }

    private void Awake()
    {
        InitRecipeData();
        craftButton.gameObject.SetActive(false);
        _playerInventory = GetComponent<PlayerInventory>();
        _resourceInventory = FindAnyObjectByType<ResourceInventory>();
        _buildingHandler = GetComponent<BuildingHandler>();
        recipeBook.CategorizeRecipes(_recipeData, this);
    }

    private void Start()
    {
        foreach (var so in testingSo)
        {
            for (int i = 0; i < so.resSo.Amount; i++)
            {
                Craft(so);
            }
        }
    }

    private void InitRecipeData()
    {
        foreach (var recipe in craftingSo)
        {
            RecipeData data = new(recipe);
            _recipeData.Add(data);
            if (earlyAccessRecipe.Contains(recipe))
                data.isLocked = false;
        }

        InitUpgradesData();
    }

    private void InitUpgradesData()
    {
        for (int i = _recipeData.Count - 1; i >= 0; i--) // go through base recipe 
        {
            if (_recipeData[i].craftingSo.upgrades.Count <= 0) continue; // if has upgrades
            var upData = GetRecipeData(_recipeData[i].craftingSo); // returns upgrade recipes 
            if (upData == null)
            {
                Debug.Log("upgrade data not found");
                continue;
            }

            foreach (var upgrade in upData)
            {
                _recipeData[i].upgrades.Add(upgrade);
                _recipeData.Remove(upgrade);
                Debug.Log(upgrade.craftingSo);
            }
        }
    }

    private List<RecipeData> GetRecipeData(CraftingSO so) // this function returns upgrade
    {
        List<RecipeData> upgrades = new();
        foreach (var up in so.upgrades)
        {
            foreach (var data in _recipeData)
            {
                if (data.craftingSo == up)
                    upgrades.Add(data);
            }
        }


        return upgrades;
    }

    private void AddIngredient(ObjSo So, InventoryItem uiPrefab)
    {
        if (!_ingredientVisuals.ContainsKey(So))
            _ingredientVisuals[So] = new List<GameObject>();


        var existing = _currentIngredients.Find(i => i.objSo == So);
        if (existing != null)
        {
            Debug.Log("old ingi");
            existing.amount++;
        }
        else
        {
            Debug.Log("new ingi");
            _currentIngredients.Add(new Ingredient { objSo = So, amount = 1 });
        }

        RectTransform rectTransform = uiPrefab.GetComponent<RectTransform>();
        rectTransform.position = craftingUITransform.position;
        _currentItems.Add(uiPrefab);
        CheckForRecipe();
    }

    private void RemoveResource(ObjSo So, InventoryItem item)
    {
        if (_ingredientVisuals.ContainsKey(So))
        {
            _ingredientVisuals.Remove(So); // remove the key and value 
        }

        for (int i = _currentIngredients.Count - 1; i >= 0; i--)
        {
            if (_currentIngredients[i].objSo == So)
            {
                _currentIngredients.RemoveAt(i);
            }
        }

        _resourceInventory.TryPlaceItem(So, item);
    }

    private void CheckForRecipe()
    {
        foreach (CraftingSO So in craftingSo)
        {
            if (Matches(So.ingredients, _currentIngredients))
            {
                _currentSo = So;
                craftButton.gameObject.SetActive(true);
                craftButton.onClick.RemoveAllListeners();
                craftButton.onClick.AddListener(() => Craft(_currentSo));
                return;
            }
        }

        _currentSo = null;
        craftButton.gameObject.SetActive(false);
    }

    private bool Matches(Ingredient[] recipeIngredient, List<Ingredient> current)
    {
        if (recipeIngredient.Length != current.Count)
            return false;

        foreach (var rec in recipeIngredient)
        {
            var match = current.Find(i => i.objSo == rec.objSo);
            if (match == null || match.amount < rec.amount)
                return false;
        }

        return true;
    }

    public void Craft(CraftingSO so)
    {
        if (so.resSo.prefab.TryGetComponent<BaseStructure>(out var structure))
        {
            SpawnStructure(so);
            recipeBook.ToggleRBook();
            EventManager.Instance.CraftEvents.ObjectCraft(so.resSo);
            return;
        }

        GameObject prefab = so.resSo.prefab;
        GameObject result = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        Obj<ObjSo> obj = result.GetComponent<Obj<ObjSo>>();
        if (obj == null)
        {
            Debug.Log(" obj is null ");
        }

        obj.So = so.resSo;
        EventManager.Instance.CraftEvents.ObjectCraft(obj.So);
        _playerInventory.AddWorldItem(result);
        craftButton.gameObject.SetActive(false);
        if (!isTesting)
            ConsumeIngredients(); // enable this 
    }

    private void ConsumeIngredients() // in consume ingredient we need 
    {
        foreach (var req in _currentSo.ingredients)
        {
            var match = _currentIngredients.Find(i => i.objSo == req.objSo);
            if (match != null)
            {
                match.amount -= req.amount;

                if (_ingredientVisuals.TryGetValue(req.objSo, out var visuals))
                {
                    for (int i = 0; i < req.amount && i < visuals.Count; i++)
                        Destroy(visuals[i]);

                    visuals.RemoveRange(0, Mathf.Min(req.amount, visuals.Count));
                }
            }
        }

        _currentIngredients.RemoveAll(i => i.amount <= 0);
        foreach (var item in _currentItems)
            Destroy(item.gameObject);
        _currentItems.Clear();
    }

    private void SpawnStructure(CraftingSO so) // spawns structure and lets base builder handle placement 
    {
        _buildingHandler.SetGHostObject(so);
    }

    private Vector3 GetNextIngredientSlotPosition()
    {
        float spacing = 0.15f; // tweak this for tighter or wider layout
        int rowSize = 4; // how many items per row before wrapping

        int row = _ingredientVisualIndex / rowSize;
        int col = _ingredientVisualIndex % rowSize;

        return new Vector3(col * spacing, 0, row * spacing);
    }

    private void UnLockRecipe(RewardData reward)
    {
        CraftingSO so = reward.RewardRecipe;
        if (so == null || IsAnUpgrade(so)) return;
        RecipeData newRecipe = new RecipeData(so);
        newRecipe.isLocked = false;
        recipeBook.AddNewRecipe(newRecipe);
        UIManager.instance.DisplayNotification("New Recipe Available");
    }

    private bool IsAnUpgrade(CraftingSO so)
    {
        // check  in base recipes if upgrade is there  
        foreach (var data in _recipeData)
        {
            foreach (var ups in data.upgrades)
            {
                if (ups.craftingSo != so) continue;
                ups.isLocked = false; // unlock upgrade
                UIManager.instance.DisplayNotification("New " +
                                                       ups.craftingSo.resSo.itemName + " Upgrade Available");
                return true;
            }
        }

        return false;
    }
}
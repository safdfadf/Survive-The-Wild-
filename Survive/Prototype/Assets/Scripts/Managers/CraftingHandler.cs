using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CraftingHandler : MonoBehaviour
{
    [SerializeField] private List<CraftingSO> craftingSo;

    [FormerlySerializedAs("craftingUI")] [SerializeField]
    private RectTransform craftingUITransform;

    [Header("Testing")] [SerializeField] private CraftingSO testingSo;


    private List<Ingredient> _currentIngredients = new();
    private Dictionary<ResourceSo, List<GameObject>> _ingredientVisuals = new();
    private PlayerInventory _playerInventory;
    private MovementHandler _movementHandler;
    private ResourceInventory _resourceInventory;
    [SerializeField] private Vector3 tableOffset;

    [SerializeField] private Button craftButton;

    [Header("Recipe Book")] [SerializeField]
    private RBookHandler recipeBook;

    //TODo: Move recipe Book ui to PlayerUI script 
    public bool enableCrafting { get; set; }

    private int _ingredientVisualIndex = 0;

    private CraftingSO _currentSo;
    private BuildingHandler _buildingHandler;

    private void OnEnable()
    {
        EventBus.OnResourceAdd += AddIngredient;
        EventBus.OnResourceRemove += RemoveResource;
    }

    private void OnDisable()
    {
        EventBus.OnResourceAdd -= AddIngredient;
        EventBus.OnResourceRemove -= RemoveResource;
    }

    private void Awake()
    {
        craftButton.gameObject.SetActive(false);
        _playerInventory = GetComponent<PlayerInventory>();
        _movementHandler = GetComponent<MovementHandler>();
        _resourceInventory = FindAnyObjectByType<ResourceInventory>();
        recipeBook.CategorizeRecipes(craftingSo, this);
        _buildingHandler = GetComponent<BuildingHandler>();
    }

    private void Start()
    {
        //    AddTestingWeapon(testingSo);
    }

    private void AddTestingWeapon(CraftingSO so)
    {
        GameObject prefab = so.resultPrefab;
        GameObject result = Instantiate(prefab, craftingUITransform.transform.position + Vector3.up * 0.2f,
            Quaternion.identity);
        if (result.TryGetComponent<BaseWeapon>(out var weapon))
        {
            weapon.SetCraftingSo(so);
            _movementHandler.InitializeWeapon(weapon);
        }

        ICollectable collectable = result.GetComponent<ICollectable>();
        AddToInventory(collectable);
        _currentIngredients.RemoveAll(i => i.amount <= 0);
        craftButton.gameObject.SetActive(false); // should be handled by ui manager
    }


    private void AddIngredient(ResourceSo So, InventoryItem uiPrefab)
    {
        if (!_ingredientVisuals.ContainsKey(So))
            _ingredientVisuals[So] = new List<GameObject>();


        var existing = _currentIngredients.Find(i => i.resourceSo == So);
        if (existing != null)
        {
            Debug.Log("old ingi");
            existing.amount++;
        }
        else
        {
            Debug.Log("new ingi");
            _currentIngredients.Add(new Ingredient { resourceSo = So, amount = 1 });
            // move ui to that craft position 
            RectTransform rectTransform = uiPrefab.GetComponent<RectTransform>();
            rectTransform.position = craftingUITransform.position;
        }

        CheckForRecipe();
    }

    private void RemoveResource(ResourceSo So, InventoryItem item)
    {
        // remove from ingredient list and remove from ingredient visual 
        if (_ingredientVisuals.ContainsKey(So))
        {
            _ingredientVisuals.Remove(So); // remove the key and value 
        }

        for (int i = _currentIngredients.Count - 1; i >= 0; i--)
        {
            if (_currentIngredients[i].resourceSo == So)
            {
                _currentIngredients.RemoveAt(i);
            }
        }
      
        _resourceInventory.TryPlaceItem(So,item); // issue here  is Inventory item 
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
            var match = current.Find(i => i.resourceSo == rec.resourceSo);
            if (match == null || match.amount < rec.amount)
                return false;
        }

        return true;
    }

    public void Craft(CraftingSO so)
    {
        if (so.resultPrefab.TryGetComponent<BaseStructure>(out var baseStructure))
        {
            SpawnStructure(so);
            recipeBook.ToggleRBook();
            return;
        }

        foreach (var req in _currentSo.ingredients)
        {
            var match = _currentIngredients.Find(i => i.resourceSo == req.resourceSo);
            if (match != null)
            {
                match.amount -= req.amount;
                if (_ingredientVisuals.TryGetValue(req.resourceSo, out var visuals))
                {
                    for (int i = 0; i < req.amount && i < visuals.Count; i++)
                    {
                        GameObject resGO = visuals[i];
                        Destroy(resGO);
                    }

                    visuals.RemoveRange(0, Mathf.Min(req.amount, visuals.Count));
                }
            }
        }

        // add  it to inventory 
        GameObject prefab = so.resultPrefab;
        GameObject result = Instantiate(prefab, craftingUITransform.transform.position + Vector3.up * 0.2f,
            Quaternion.identity);
        if (result.TryGetComponent<BaseWeapon>(out var weapon))
        {
            weapon.SetCraftingSo(so);
            _movementHandler.InitializeWeapon(weapon);
        }

        BaseResource baseResource = result.GetComponent<BaseResource>();
        if (baseResource != null)
        {
            baseResource.So = SoProvider.instance.GetSoForPrefab(so.resultPrefab);
        }

        ICollectable collectable = result.GetComponent<ICollectable>();
        AddToInventory(collectable);
        _currentIngredients.RemoveAll(i => i.amount <= 0);
        craftButton.gameObject.SetActive(false); // should be handled by ui manager
    }

    private void SpawnStructure(CraftingSO so) // spawns structure and lets base builder handle placement 
    {
        _buildingHandler.SetGHostObject(so);
    }

    public void ToggleRBook()
    {
        recipeBook.ToggleRBook();
    }

    private void AddToInventory(ICollectable collectable)
    {
        _playerInventory.AddResource(collectable);
    }

    private Vector3 GetNextIngredientSlotPosition()
    {
        float spacing = 0.15f; // tweak this for tighter or wider layout
        int rowSize = 4; // how many items per row before wrapping

        int row = _ingredientVisualIndex / rowSize;
        int col = _ingredientVisualIndex % rowSize;

        return new Vector3(col * spacing, 0, row * spacing);
    }
}
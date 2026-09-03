using System;
using System.Collections.Generic;
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
    private MovementHandler _movementHandler;
    private ResourceInventory _resourceInventory;
    private WeaponInventory _weaponInventory;
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
        EventBus.OnCraftResource += AddIngredient;
        EventBus.OnUnCraftResource += RemoveResource;
    }

    private void OnDisable()
    {
        EventBus.OnCraftResource -= AddIngredient;
        EventBus.OnUnCraftResource -= RemoveResource;
    }

    private void Awake()
    {
        craftButton.gameObject.SetActive(false);
        _playerInventory = GetComponent<PlayerInventory>();
        _movementHandler = GetComponent<MovementHandler>();
        _resourceInventory = FindAnyObjectByType<ResourceInventory>();
        _weaponInventory = FindAnyObjectByType<WeaponInventory>();
        recipeBook.CategorizeRecipes(craftingSo, this);
        _buildingHandler = GetComponent<BuildingHandler>();
    }

    private void Start()
    {
        foreach (var so in testingSo)
        {
            for (int i = 0; i <= so.resSo.Amount; i++)
            {
                Craft(so);
            }
        }
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
            RectTransform rectTransform = uiPrefab.GetComponent<RectTransform>();
            rectTransform.position = craftingUITransform.position;
        }

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

        _resourceInventory.TryPlaceItem(So, item); // issue here  is Inventory item 
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
        _playerInventory.AddWorldItem(result);
//        ConsumeIngredients();// enable this 
        craftButton.gameObject.SetActive(false);
    }

    private void ConsumeIngredients()
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
    }

    private void SpawnStructure(CraftingSO so) // spawns structure and lets base builder handle placement 
    {
        _buildingHandler.SetGHostObject(so );
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
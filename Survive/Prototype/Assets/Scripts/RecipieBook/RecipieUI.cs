using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RecipieUI : MonoBehaviour
{
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private Image image;
    [SerializeField] private Transform parent;
    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private Button upgradesSlot;
    [SerializeField] private Transform slotParent;
    private CraftingHandler _craftingHandler;
    private Image _image;
    private Dictionary<Button, RecipeData> _upgrades = new();
    private RecipeData _currentCraftingSo;

    private void Awake()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(SpawnRecipie);
        _image = GetComponentInChildren<Image>();
    }

// Recipe data is crafting so + locked data 
    public void Initialize(RecipeData data, CraftingHandler craftingHandler)
    {
        if (data.craftingSo == null)
        {
            Debug.Log("_craftingSo reci[ie is null");
            return;
        }

        _currentCraftingSo = data;
        _craftingHandler = craftingHandler;
        CreateSlot(data);

        BaseWeapon weapon = _currentCraftingSo.craftingSo.resSo.prefab.GetComponent<BaseWeapon>();
        if (weapon != null)
        {
            Button button = GetComponent<Button>();
            button.interactable = false;
        }

        HandleUpgradesSlot();
        ShowRecipe();
    }

    private void HandleUpgradesSlot()
    {
        if (!_currentCraftingSo.craftingSo.hasUpgrades) return;
        foreach (var up in _currentCraftingSo.upgrades)
        {
            CreateSlot(up);
        }
    }

    private void CreateSlot(RecipeData data)
    {
        Button obj = Instantiate(upgradesSlot, slotParent);
        obj.onClick.AddListener(() => OpenSlot(obj));
        if (data.isLocked)
        {
            obj.interactable = false;
            // switch to disabled 
        }

        _upgrades.Add(obj, data);
    }

    private void ShowRecipe()
    {
        _image.sprite = _currentCraftingSo.craftingSo.resSo.sprite;
        TextMeshProUGUI[] objs = parent.gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = objs.Length-1; i >= 0; i--)
        {
            Destroy(objs[i]);
        }

        foreach (var ing in _currentCraftingSo.craftingSo.ingredients)
        {
            Name.text = _currentCraftingSo.craftingSo.resSo.itemName;
            Debug.Log("create textmeh");
            GameObject textObj = Instantiate(textPrefab, parent);
            TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
            tmp.text = ing.objSo.prefab.name.ToString() + " * " + ing.amount.ToString();
        }
    }

    private void OpenSlot(Button button)
    {
        if (!_upgrades.ContainsKey(button)) return;
        _currentCraftingSo = _upgrades[button];
        ShowRecipe();
    }

    private void SpawnRecipie()
    {
        _craftingHandler.Craft(_currentCraftingSo.craftingSo);
    }
}
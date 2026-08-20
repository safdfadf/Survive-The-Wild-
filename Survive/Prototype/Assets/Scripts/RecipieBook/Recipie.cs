using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Recipie : MonoBehaviour
{
    private CraftingSO _craftingSo;
    private Ingredient[] _ingredients;
    [SerializeField] private GameObject textPrefab;

    private CraftingHandler _craftingHandler;
    private Image _image;

    private void Awake()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(SpawnRecipie);
        _image = GetComponent<Image>();
    }

    public void Initialize(CraftingSO craftingSo, CraftingHandler craftingHandler)
    {
        if (_craftingSo == null)
        {
            Debug.Log("_craftingSo reci[ie is null");
        }

        _craftingSo = craftingSo;
        _ingredients = craftingSo.ingredients;
        _craftingHandler = craftingHandler;
        BaseWeapon weapon = _craftingSo.resSo.prefab.GetComponent<BaseWeapon>();
        if (weapon != null)
        {
            Button button = GetComponent<Button>();
            button.interactable = false;
        }

        ShowRecipe();
    }

    private void ShowRecipe()
    {
        _image.sprite = _craftingSo.resSo.sprite;
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        foreach (var ing in _craftingSo.ingredients)
        {
            GameObject textObj = Instantiate(textPrefab, transform);
            TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
            tmp.text = ing.resourceSo.prefab.name.ToString() + " * " + ing.amount.ToString();
        }
    }

    private void SpawnRecipie()
    {
        _craftingHandler.Craft(_craftingSo);
    }
}
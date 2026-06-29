using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseStructure : MonoBehaviour
{
    protected bool IsAssembled;
    protected CraftingSO CraftingSo;
    public bool IsPlayerInRange { get;private set; }
    private Ingredient[] _requiredIngredients = new Ingredient[0];
    private StructureUI _structureUI;
    
    private Material _originalMaterial;
    private Material _ghostMaterial;
    private Material _invalidMat;
    private Material _currentMaterial;
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _structureUI = GetComponent<StructureUI>();
         _meshRenderer = GetComponentInChildren<MeshRenderer>();
         if (_meshRenderer == null)
         {
             Debug.LogError($"{name} has no MeshRenderer");
         }
        _currentMaterial = _meshRenderer.material;
        _originalMaterial = _meshRenderer.material;
        _structureUI.ToggleDescription(false);
    }
    private void Assemble()
    {
       
        IsAssembled = true;
        _currentMaterial = _originalMaterial;
        _meshRenderer.material = _currentMaterial;
        _structureUI.ToggleDescription(false);

    }
    public void Initialize(CraftingSO so)
    {
        CraftingSo = so;
        SetRequiredIngredients(CraftingSo.ingredients);
    }

    private void SetRequiredIngredients(Ingredient[] original)
    {
        _requiredIngredients = new Ingredient[original.Length];

        // Deep copy each ingredient
        for (int i = 0; i < original.Length; i++)
        {
            _requiredIngredients[i] = new Ingredient
            {
                resourceSo = original[i].resourceSo,
                amount = original[i].amount
            };
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            IsPlayerInRange = true;
        }
    }

    public void SubmitResource(ResourceSo resourceSo)// ingredient is a data type contains resource and amount 
    {
       if(!CheckSubmitResource(resourceSo))
          return; 
       
       
       Ingredient ing = GetIngredient(resourceSo);
       if(ing == null)return;
       Debug.Log("sumit resource");
       ing.amount--;
       if (CheckSubmited())
       {
           Debug.Log("submitted");
           Assemble();
       }
    }
    private bool CheckSubmitResource(ResourceSo resourceSo)// check if submited resource is valid 
    {
        foreach (var ing in _requiredIngredients)
        {
            if (ing.resourceSo != resourceSo)
            {
                continue;
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    private Ingredient GetIngredient(ResourceSo resourceSo)
    {
        foreach (var ing in _requiredIngredients)
        {
            if (ing.resourceSo == resourceSo)
            {
               return ing;
            }
        }
        return null;
    }
    private bool CheckSubmited()
    {
        foreach (var ing in _requiredIngredients)
        {
            if (ing.amount ==0)
            {
                continue;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public ResourceSo GetNextRequiredResource()
    {
        foreach (var ing in _requiredIngredients)
        {
            if (ing.amount > 0)
            {
                _structureUI.SetDescription(ing.resourceSo.name, ing.amount);
                return ing.resourceSo;
            }
        }
        return null; 
    }

    public void ToggleMat(bool valid)
    {
        if (valid)
        {
            _currentMaterial = _ghostMaterial;
        }
        else
        {
            _currentMaterial = _invalidMat;
        }

        if (_meshRenderer == null)
        {
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
        }
        _meshRenderer.material = _currentMaterial;
    }

    public void SetValidMat(Material mat)
    {
        _ghostMaterial = mat;
    }

    public void SetInvalidMat(Material mat)
    {
        _invalidMat = mat;
    }

    public void ToggleDescription(bool valid)
    {
        if(IsAssembled)return;
        _structureUI.ToggleDescription(valid);
    }
}

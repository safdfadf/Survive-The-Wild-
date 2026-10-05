using System;
using System.Collections.Generic;
using DefaultNamespace.EventBus;
using UnityEngine;
using UnityEngine.Serialization;

public class
    BaseStructure : Environment, IInteractable
{
    protected bool IsAssembled;
    public BuildingRecipe craftingRecipe { get; private set; }

    protected Ingredient[] _requiredIngredients = new Ingredient[0];
    protected StructureUI _structureUI;

    private Material _originalMaterial;
    private Material _ghostMaterial;
    private Material _invalidMat;
    private Material _currentMaterial;
    private MeshRenderer _meshRenderer;

    protected List<BaseStructure> childStructures = new();
    private Collider[] col;

    public bool outlineMe { get; set; }
    public bool canBeCollected { get; set; }
    public GameObject Gm { get; set; }
    public bool isHit { get; set; }
    public Vector3 hitPos { get; set; }


    public bool canDisplay { get; set; }
    public string useMeDescription { get; set; }
    public string Description { get; set; }
    public bool canUse { get; set; }
    public bool canHarvest { get; set; }
    public bool canCraft { get; set; }

    protected override void Awake()
    {
        base.Awake();
        _structureUI = GetComponent<StructureUI>();
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _currentMaterial = _meshRenderer.material;
        _originalMaterial = _meshRenderer.material;
        _structureUI.ToggleDescription(false);
        col = GetComponentsInChildren<Collider>();
        Gm = gameObject;
        SetBools();
    }

    public virtual void ExecuteAction()
    {
        ObjSo so = GetNextRequiredResource();
        if (!IsAssembled || so != null)
        {
            EventManager.Instance.StructureEvents.SubmitResource();
        }
    }

    private void Assemble()
    {
        IsAssembled = true;
        _currentMaterial = _originalMaterial;
        _meshRenderer.material = _currentMaterial;
        _structureUI.ToggleDescription(false);
        _requiredIngredients = new Ingredient[0];
        OnStructureAssembled();
    }

    protected virtual void OnStructureAssembled()
    {
    }

    public void InitializeStructure(CraftingSO so)
    {
        craftingRecipe = so as BuildingRecipe;
        if (craftingRecipe == null)
        {
            Debug.Log("recipe is null");
        }

        SetRequiredIngredients(craftingRecipe.ingredients);
    }

    private void SetRequiredIngredients(Ingredient[] original)
    {
        _requiredIngredients = new Ingredient[original.Length];

        for (int i = 0; i < original.Length; i++)
        {
            _requiredIngredients[i] = new Ingredient
            {
                objSo = original[i].objSo,
                amount = original[i].amount
            };
        }
    }

    public virtual void SubmitResource(ObjSo objSo) // ingredient is a data type contains resource and amount 
    {
        if (!CheckSubmitResource(objSo))
        {
            return;
        }

        Ingredient ing = GetIngredient(objSo);
        if (ing == null) return;
        ing.amount--;
        if (!CheckSubmited()) return;
        Assemble();
    }

    protected bool
        CheckSubmitResource(
            ObjSo objSo) // check if submited resource is valid we need to check wheather it is frame or we are trying to place a child 
    {
        foreach (var ing in _requiredIngredients)
        {
            if (ing.objSo != objSo)
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

    private Ingredient GetIngredient(ObjSo objSo)
    {
        foreach (var ing in _requiredIngredients)
        {
            if (ing.objSo == objSo)
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
            if (ing.amount == 0)
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

    public ObjSo GetNextRequiredResource()
    {
        foreach (var ing in _requiredIngredients)
        {
            if (ing.amount > 0)
            {
                Description = ing.objSo.name + " x " + ing.amount; // update ui 
                return ing.objSo;
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

    public void DisabelAllColliders()
    {
        foreach (var c in col)
        {
            c.enabled = false;
        }
    }

    public void EnableAllColliders()
    {
        foreach (var VARIABLE in col)
        {
            VARIABLE.enabled = true;
        }
    }

    public void Craft()
    {
        throw new NotImplementedException();
    }

    public void Harvest()
    {
        throw new NotImplementedException();
    }

    public void UseMe()
    {
        throw new NotImplementedException();
    }

    private void SetBools()
    {
        canDisplay = true;
        canCraft = false;
        canHarvest = false;
        canUse = false;
        outlineMe = false;
    }
}


public enum StructureType
{
    Frame,
    Wall,
    WindowWall,
    DoorWall,
    Floor,
    Roof,
    Stairs,
    Pillar
}
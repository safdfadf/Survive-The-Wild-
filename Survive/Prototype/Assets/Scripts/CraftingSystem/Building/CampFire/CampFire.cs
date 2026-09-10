using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.EventBus;
using FoodSystem;
using UnityEngine;


public class CampFire : BaseStructure, ICook // this will be base class for all the fire 
{
    [SerializeField] private Ingredient[] _combustibleIngredients;

    [SerializeField] private List<Transform> cookingSpots;

    [Header("Vfx")] [SerializeField] private GameObject _fireVfx;
    [SerializeField] private float maxFireTime = 60;
    [SerializeField] private float midThreshold = 30;
    [SerializeField] private float minThreshold = 10;

    private float _fireTime;
    private bool _canIgnite;
    private bool _isBurning;
    private bool _canCook;
    private List<Food> _foodInSpot = new();
    private Transform[] fxObjects;

    protected override void Awake()
    {
        fxObjects = _fireVfx.GetComponentsInChildren<Transform>();
        base.Awake();
    }

    private void OnEnable()
    {
        EventBus.OnToggleTracksMenu += Ignite;
        EventManager.Instance.reseourceEvent.onGatherResource += RemoveFood;
    }

    private void OnDisable()
    {
        EventBus.OnToggleTracksMenu -= Ignite;
        EventManager.Instance.reseourceEvent.onGatherResource -= RemoveFood;
    }

    private void RemoveFood(GameObject food)
    {
        Food f = food.GetComponent<Food>();
        f.canCookMe = false;
        if (_foodInSpot.Contains(f))
        {
            UIManager.instance.DisplayCookingSpots(cookingSpots, gameObject);
            _foodInSpot.Remove(f);
        }
    }

    protected override void OnStructureAssembled()
    {
        _requiredIngredients = new Ingredient[_combustibleIngredients.Length];
        for (int i = 0; i < _combustibleIngredients.Length; i++)
        {
            _requiredIngredients[i] = new Ingredient
            {
                objSo = _combustibleIngredients[i].objSo,
                amount = _combustibleIngredients[i].amount
            };
        }

        Debug.Log("Campfire assembled. Combustible phase started." + _requiredIngredients.Length);

        _structureUI.SetDescription(_requiredIngredients[0].objSo.name, _requiredIngredients[0].amount);
        _structureUI.ToggleDescription(true);
    }

    public override void SubmitResource(ObjSo objSo)
    {
        if (!IsAssembled)
        {
            base.SubmitResource(objSo);
            return;
        }

        Ingredient ing = GetCombustibleIngredient(objSo);
        if (ing == null) return;

        ing.amount--;

        if (CheckCombustibleSubmitted())
        {
            _canIgnite = true;
            _structureUI.SetDescription("Press E to Ignite", 0);
        }
        else
        {
            GetNextRequiredResource();
        }
    }

    private Ingredient GetCombustibleIngredient(ObjSo objSo)
    {
        foreach (var ing in _requiredIngredients)
        {
            if (ing.objSo == objSo)
                return ing;
        }

        return null;
    }

    private bool CheckCombustibleSubmitted()
    {
        foreach (var ing in _requiredIngredients)
        {
            if (ing.amount > 0)
                return false;
        }

        return true;
    }

    protected override void PlayerInRange(bool inRange)
    {
        if (inRange && IsAssembled)
        {
            UIManager.instance.DisplayCookingSpots(cookingSpots, gameObject);
        }
        else
        {
            UIManager.instance.ClearAllCookingSpots();
        }
    }

    private void Ignite() // now how ignite function will be called, we can still call E  
    {
        if (!_canIgnite || _isBurning || IsPlayerInRange) return;

        _isBurning = true;

        Debug.Log("Campfire ignited!");
        StartCoroutine(FireCoroutine());
    }

    private IEnumerator FireCoroutine()
    {
        _fireTime = maxFireTime;
        float damageTimer = 0f;

        _fireVfx.SetActive(true);

        while (_fireTime > 0) // instead of while loop use 5 sec events
        {
            _fireTime -= Time.deltaTime / 60;
            damageTimer += Time.deltaTime;
            CookFoods(Time.deltaTime);
            UpdateFire(_fireTime);

            if (damageTimer >= 60f)
            {
                SelfAttack atk = new SelfAttack(5, Vector3.zero);
                damageTimer = 0f;
                base.TakeDamage(atk);
            }

            yield return null;
        }

        _fireVfx.SetActive(false);
    }

    private void UpdateFire(float currentTime)
    {
        while (true)
        {
            if (currentTime > midThreshold)
            {
                UpdateFxScale(.3f);
            }
            else if (currentTime > minThreshold)
            {
                UpdateFxScale(.2f);
                continue;
            }
            else
            {
                UpdateFxScale(.13f);
                continue;
            }

            break;
        }
    }

    protected override void Break()
    {
        Debug.Log("Campfire breaking");
    }

    private void UpdateFxScale(float scale)
    {
        foreach (var t in fxObjects)
        {
            t.localScale = new Vector3(scale, scale, scale);
        }
    }

    public void ExecuteCooking(Food food)
    {
        if (food == null)
        {
            print("food is null");
        }

        //still need a 
        _canCook = true;
        _foodInSpot.Add(food);
    }

    private void CookFoods(float deltaTime)
    {
        if (!_canCook)
        {
            Debug.Log("returning");
            return;
        }

        foreach (var food in _foodInSpot)
        {
            if (food == null || !food.canCookMe) return;

            if (food.currentState == food.rawState)
            {
                food.cookTime -= deltaTime;
                if (food.cookTime <= 0)
                {
                    food.ApplyState(food.cookState);
                }
            }
            else if (food.currentState == food.cookState)
            {
                food.burnTime -= deltaTime;
                if (food.burnTime <= 0)
                {
                    food.ApplyState(food.burntState);
                }
            }
        }
    }
}
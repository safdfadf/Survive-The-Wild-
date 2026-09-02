using System;
using System.Collections.Generic;
using System.Linq;
using FoodSystem;
using Inventory;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.UI;

//Debug: ToDo : remove arrow from this scirpt
public class PlayerInventory : MonoBehaviour
{
    private MovementHandler _movementHandler;
    private Dictionary<ObjSo, List<GameObject>> resourcePool = new();

    [SerializeField] private GameObject uiItemPrefab;
    [SerializeField] private ResourceInventory _resourceInventory;
    [SerializeField] private WeaponInventory _weaponInventory;
    [SerializeField] private GameObject worldStorage;
    [Header("testing")] [SerializeField] private ObjSo ArrowSo; // testing purpose
    [SerializeField] private List<ObjSo> testResource;
    private ObjSo _requestedObj;
    private BaseStructure _currentStructure;
    private int _funcCount;

    private void Awake()
    {
        _movementHandler = GetComponent<MovementHandler>();
    }

    private void Start()
    {
        AddTEstResourse();
    }


    private void AddTEstResourse()
    {
        foreach (ObjSo so in testResource)
        {
            GameObject obj = Instantiate(so.prefab, transform.position, Quaternion.identity);
            Obj<ObjSo> res = obj.GetComponentInParent<Obj<ObjSo>>();
            res.Initialize(so);
            //     AddWorldItem(obj);
        }
    }

    public GameObject GetResource(ObjSo so)
    {
        if (resourcePool.ContainsKey(so) && resourcePool[so].Count > 0)
        {
            GameObject obj = resourcePool[so][0];
            resourcePool[so].RemoveAt(0);
            Obj<ObjSo> res = obj.GetComponent<Obj<ObjSo>>();
            _resourceInventory.RemoveResourse(res);
            return obj;
        }

        return null;
    }

    public void AddWorldItem(GameObject worldObj)
    {
        MoveTo(worldObj);
        if (worldObj.TryGetComponent<BaseWeapon>(out var weapon))
        {
            WeaponSo weaponSo = weapon.So as WeaponSo;
            AddToResPool(weaponSo, worldObj);
            MakeUI(weaponSo, weapon);
            return;
        }

        if (worldObj.TryGetComponent<Food>(out var food))
        {
            FoodSo foodSo = food.So as FoodSo;
            AddToResPool(foodSo, worldObj);
            MakeUI(foodSo, food);
            return;
        }

        if (worldObj.TryGetComponent<Obj<ObjSo>>(out var baseRes))
        {
            ObjSo so = baseRes.So;
            AddToResPool(so, worldObj);
            MakeUI(so, baseRes);
            return;
        }

        Debug.LogWarning("Unknown world item type picked up.");
    }

    private void AddToResPool(ObjSo objSo, GameObject obj)
    {
        if (resourcePool.ContainsKey(objSo))
        {
            resourcePool[objSo].Add(obj);
            return;
        }

        resourcePool.Add(objSo, new List<GameObject> { obj });
    }

    private void MakeUI(ObjSo so, Obj<ObjSo> res)
    {
        GameObject uiObj = Instantiate(uiItemPrefab);
        InventoryItem item = uiObj.GetComponent<InventoryItem>();
        SetInventoryItem(res, so, item);
        if (res.TryGetComponent<BaseWeapon>(out var weapon))
        {
            _movementHandler.InitializeWeapon(weapon); // maybe this is not the best place to init it 
            _weaponInventory.AddWeapon(so as WeaponSo, item);
            return;
        }

        _resourceInventory.TryPlaceItem(so, item);
    }

    private void MoveTo(GameObject obj) // we are moving physical objs here they should be in a list 
    {
        // someList.Add(Objs), instead of int dictionary should simpley store objs 
        obj.transform.position = worldStorage.transform.position;
        obj.transform.SetParent(worldStorage.transform);
        obj.SetActive(false);
    }

    private void SetInventoryItem(Obj<ObjSo> res, ObjSo So, InventoryItem item)
    {
        item.size = So.size;
        RectTransform rect = item.GetComponent<RectTransform>();
        item.rect = rect;
        Image image = item.GetComponent<Image>();
        item.icon = image;
        item.so = So;
        item.SetItem(So.sprite, res.gameObject);
        res.InventoryItem = item;
    }

    public void RemoveResource(Obj<ObjSo> resource, bool isToBeDestroy)
    {
        ObjSo So = resource.So;

        if (!resourcePool.ContainsKey(So)) return;

        if (resourcePool[So].Contains(resource.gameObject))
        {
            resourcePool[So].Remove(resource.gameObject);
            _resourceInventory.RemoveResourse(resource);
        }

        if (resourcePool[So].Count == 0)
        {
            Debug.Log("removing So ");
            resourcePool.Remove(So);
        }


        if (isToBeDestroy)
        {
            GlobalPool.instance.Return(So.prefab, resource.gameObject);
        }
        else
        {
            SpawnObject(So);
        }
    }

    public void RemoveWeapon(WeaponSo so)
    {
        _weaponInventory.RemoveWeapon(so);
    }

    private void SpawnObject(ObjSo So)
    {
        GameObject
            obj = GlobalPool.instance.Get(So.prefab,
                Vector3.forward); // ToDo: instead of vector3 forward replace with something else 
        Obj<ObjSo> res = obj.GetComponent<Obj<ObjSo>>();
        res.Initialize(So);
    }

    public void SetSubmitResource(ObjSo So, BaseStructure structure)
    {
        _requestedObj = So;
        _currentStructure = structure;
    }

    public void SubmitResource() // function is used to assemble structures 
    {
        if (_requestedObj == null || _currentStructure == null) return;
        if (!resourcePool.ContainsKey(_requestedObj)) return;
        Obj<ObjSo> res = resourcePool[_requestedObj][0].GetComponent<Obj<ObjSo>>();
        RemoveResource(res, true);
        _currentStructure.SubmitResource(_requestedObj);
    }

    public void MakeItemAnCraft(Obj<ObjSo> obj)
    {
        AddWorldItem(obj.gameObject); // add to inventory and create inventory item
        InventoryItem item = _resourceInventory.GetInventoryItem(obj.InventoryItem);
        obj.InventoryItem.Craft();
        // open inventory  
    }
}
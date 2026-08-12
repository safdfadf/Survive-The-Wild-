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
    private List<GameObject> arrowPool = new();
    [SerializeField] private GameObject uiItemPrefab;
    private Dictionary<ResourceSo, int> resourcePool = new();
    [SerializeField] private ResourceInventory _resourceInventory;
    [SerializeField] private WeaponInventory _weaponInventory;
    [SerializeField] private GameObject worldStorage;
    [Header("testing")] [SerializeField] private ResourceSo ArrowSo; // testing purpose
    [SerializeField] private List<ResourceSo> testResource;
    private ResourceSo _requestedResource;
    private BaseStructure _currentStructure;

    private void Awake()
    {
        _movementHandler = GetComponent<MovementHandler>();
    }

    private void Start()
    {
         AddTEstResourse();
    }

    private void MakeArrowForTesting()
    {
        for (int i = 0; i <= 100; i++)
        {
            GameObject obj = Instantiate(ArrowSo.prefab, transform.position, Quaternion.identity);
            CollectArrow(obj);
        }
    }

    private void AddTEstResourse()
    {
        foreach (ResourceSo so in testResource)
        {
            GameObject obj = Instantiate(so.prefab, transform.position, Quaternion.identity);
            BaseObj res = obj.GetComponentInParent<BaseObj>();
            res.Initialize(so);
            //   AddCollectable(collectable);
            AddWorldItem(obj);
        }
    }

    public GameObject GetNextArrow()
    {
        var arrow = arrowPool.FirstOrDefault(a => !a.activeInHierarchy);
        return arrow;
    }

    private void CollectArrow(GameObject arrow)
    {
        arrow.SetActive(false);
        arrow.transform.SetParent(null);
        arrowPool.Add(arrow);
    }
    public void AddWorldItem(GameObject worldObj)
    {
        MoveTo(worldObj);
        if (worldObj.TryGetComponent<BaseWeapon>(out var weapon))
        {
            WeaponSo weaponSo = weapon.So as WeaponSo;
           
            AddToInventory(weaponSo,weapon);
            return;
        }
        if (worldObj.TryGetComponent<BaseObj>(out var baseRes))
        {
            ResourceSo so = baseRes.So;
            AddToInventory(so, baseRes);
            return;
        }
        Debug.LogWarning("Unknown world item type picked up.");
    }

    private void AddToInventory(ResourceSo so, Obj<ResourceSo> res)
    {
        GameObject uiObj = Instantiate(uiItemPrefab);
        InventoryItem item = uiObj.GetComponent<InventoryItem>();
        SetInventoryItem(res, so, item);
        if (res.TryGetComponent<BaseWeapon>(out var weapon))
        {
            _movementHandler.InitializeWeapon(weapon);// maybe this is not the best place to init it 
            _weaponInventory.AddWeapon(so as WeaponSo,item);
            return;
        }
        _resourceInventory.TryPlaceItem(so, item);
    }
    private void MoveTo(GameObject obj)
    {
        obj.transform.position = worldStorage.transform.position;
        obj.transform.SetParent(worldStorage.transform);
        obj.SetActive(false);
    }

    public void RemoveArrow(GameObject arrow)
    {
        arrowPool.Remove(arrow);
    }
    private void SetInventoryItem(Obj<ResourceSo> res, ResourceSo So, InventoryItem item)
    {
        item.SetUseMe(res.canUseButton); // is there anything that needs to set
        item.size = So.size;
        RectTransform rect = item.GetComponent<RectTransform>();
        item.rect = rect;
        Image image = item.GetComponent<Image>();
        item.icon = image;
        item.so = So;
        item.SetItem(So.sprite,res.gameObject);
        item.UseMeFunctionality(res.UseMe);
    }

    public void RemoveResource(ResourceSo So, GameObject resource)
    {
        if (resourcePool.ContainsKey(So))
        {
            if (resourcePool[So] > 0)
            {
                resourcePool[So]--;
            }
            else
            {
                resourcePool.Remove(So);
            }
        }
        else
        {
            return;
        }

        GlobalPool.instance.Get(So.prefab, Vector3.forward); // ToDo: correct Position
        Obj<ResourceSo> res = resource.GetComponent<BaseObj>();
        res.Initialize(So);
        // spawn the real world in 
    }

    public void SetSubmitResource(ResourceSo So, BaseStructure structure)
    {
        _requestedResource = So;
        _currentStructure = structure;
    }

    public void SubmitResource()
    {
        if (_requestedResource == null || _currentStructure == null) return;
        if (resourcePool.ContainsKey(_requestedResource))
        {
            if (resourcePool[_requestedResource] > 0)
            {
                resourcePool[_requestedResource]--;
            }
            else
            {
                resourcePool.Remove(_requestedResource);
            }

            _resourceInventory.RemoveResourse(_requestedResource);
            _currentStructure.SubmitResource(_requestedResource);
        }
    }
}
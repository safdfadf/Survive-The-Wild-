using System;
using System.Collections.Generic;
using System.Linq;
using FoodSystem;
using Inventory;
using UnityEngine;

//Debug: ToDo : remove arrow from this scirpt
public class PlayerInventory : MonoBehaviour 
{
    private List<GameObject> arrowPool = new();
    private Dictionary<ResourceSo,int> resourcePool = new();
   [SerializeField] private ResourceInventory _resourceInventory;
    [SerializeField]private WeaponInventory _weaponInventory;
    [Header("testing")]
    [SerializeField] private ResourceSo ArrowSo;// testing purpose
    [SerializeField] private List<ResourceSo> testResource;
 
    private ResourceSo _requestedResource;  
    private BaseStructure _currentStructure;
    private void Start()
    {
        MAkeArrowForTesting();
        AddTEstResourse();
    }

    private void MAkeArrowForTesting()
    {
        for (int i = 0; i <=100; i++)
        {
         GameObject obj = Instantiate(ArrowSo.prefab, transform.position , Quaternion.identity);
         CollectArrow(obj);
        }
    }

    private void AddTEstResourse()
    {
        foreach (ResourceSo so in testResource)
        {
            GameObject obj = Instantiate(so.prefab, transform.position , Quaternion.identity);
            ICollectable collectable = obj.GetComponent<ICollectable>();
            BaseResource res = obj.GetComponentInParent<BaseResource>();
            res.Initialize(so);
            AddResource(collectable);
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
    public void AddResource(ICollectable collectable)// collects the highlighted object and adds it to the respective inventory
    {
        if (collectable == null)
        {
            Debug.Log("collector is null" + collectable.canBeCollected);
            return;
        }
        
        collectable.canBeCollected = false;
        GameObject item = collectable.Gm;

        if(item == null){Debug.Log("item is null");return;}
        
        if (item.TryGetComponent<ArrowScript>(out var arrowScript))
        {
            CollectArrow(arrowScript.gameObject);
            return;
        }

        if (item.TryGetComponent<BaseWeapon>(out var weapon))
        {
            _weaponInventory.AddWeapon(weapon);
            return;
        }
        ResourceSo so = (collectable as Resource<ResourceSo>)?.So;
        MeshFilter mf = item.gameObject.GetComponent<MeshFilter>();
        if (mf != null && so.inventoryItem != null)
        {
            mf.mesh = so.inventoryItem;
        }
        if (so == null)
        {
            so = (collectable as Resource<FoodSo>)?.So;
            Debug.Log("so is null");
        }
        if (resourcePool.ContainsKey(so))
        {
            resourcePool[so]++;
        }
        else
        {
            resourcePool[so] = 1;
        }
        if (item== null)
        {
            Debug.Log("resource is null");return;
        }
        AddToInventory(so, item);

    }

    public void RemoveArrow(GameObject arrow)
    {
        arrowPool.Remove(arrow);
    }
    private void AddToInventory(ResourceSo So, GameObject resource)
    {
        if(_resourceInventory == null){Debug.Log("_resourceInventory is null");return;}
        if(So == null){Debug.Log("So is null");return;}
        _resourceInventory.TryPlaceItem(So,resource);
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
        GlobalPool.instance.Return(So.prefab, resource);
    }

    public void SetSubmitResource(ResourceSo So, BaseStructure structure)
    {
        _requestedResource = So;
        _currentStructure = structure;
    }

    public void SubmitResource()
    {
        if(_requestedResource == null|| _currentStructure == null)return;
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




using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

public class GlobalPool:MonoBehaviour 
{
    public static GlobalPool instance;
    public static int TreeCount;
    
    private int _trimPoolCount;

    private Dictionary<GameObject, Queue<GameObject>> pool = new();//queue of inactive objects grouped by prefab
   // since So has the amount of how many it environment will be spawned in the chunk, this will be used to multiply it if needed

   
     public  Transform environmentParent;
     public  Transform resourceParent;
     public  Transform tracksParent;
     public Transform animalParent;
     
     [SerializeField] private int environmentLimit;
    [SerializeField] private int envMultiplier;
    
    private void Awake()
    {
        if (instance == null){instance = this;}
        else
        {
            Destroy(gameObject);
        }
    }
    public void PreWarm(GameObject prefab, int count)
    { 
        if (!pool.ContainsKey(prefab))
            pool[prefab] = new Queue<GameObject>();

        for (int i = 0; i < count* envMultiplier; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool[prefab].Enqueue(obj);
        }
    }
    public GameObject Get(GameObject prefab, Vector3 pos) // give the inactive object or instentiates a new one 
    {
        if (!pool.ContainsKey(prefab))
            pool[prefab] = new Queue<GameObject>();

        GameObject obj;

        if (pool[prefab].Count > 0)
        {
            
            obj = pool[prefab].Dequeue();//removes the first object from the queue and gives it back 
        }
        else
        {
            obj = Instantiate(prefab);
        }
        obj.transform.SetPositionAndRotation(pos, Quaternion.identity);
        if (obj.TryGetComponent<Environment>(out _))
        {
            obj.transform.SetParent(environmentParent,false);
        }
        else if (obj.TryGetComponent<BaseResource>(out _))
        {
            obj.transform.SetParent(resourceParent,false);
        }
        else if (obj.TryGetComponent<AnimalBase>(out _))
        {
            obj.transform.SetParent(animalParent,false);
        }
        else if (obj.TryGetComponent<Tracks>(out _))
        {
            obj.transform.SetParent(tracksParent,false);
        }
        obj.SetActive(true);
        return obj;
    }          
    public void Return(GameObject prefab, GameObject obj)
    {
        obj.SetActive(false);
        if (!pool.ContainsKey(prefab)) 
            pool[prefab] = new Queue<GameObject>();
        pool[prefab].Enqueue(obj);
    }
    public int GetInactiveCount(GameObject prefab)
    {
        if (!pool.ContainsKey(prefab))
            return 0;

        return pool[prefab].Count;
    }

}



using UnityEngine;

public class Environment : MonoBehaviour,ItakeDamage,IsoInitializer<EnvironSo>
{
   public  EnvironSo environSo{get;private set;}
   protected int currentHealth;
   [SerializeField] protected int ResourceDropCount;
   [SerializeField] protected Vector3 dropOffset;
   protected PosInChunk cashedPosInChunk;
 

   public void Initialize(EnvironSo so)
   {
      environSo = so ;

      if (environSo == null)
      {
         Debug.LogError("Wrong SO type passed to EnvironResource!");
         return;
      }

   }
   public void SeCashedPos(PosInChunk casedPos)
   {
    cashedPosInChunk = casedPos;  
   }
   public void TakeDamage(int damage)
   { 
      Debug.Log(gameObject.name + " taking damage " + damage);
      currentHealth -= damage;
      if (currentHealth <= 0)
      {
         Break();
      }
   }
   protected virtual void Break()
   {
      for (int i = 0; i < ResourceDropCount; i++)
      {
         Debug.Log("i am breaking");
         // call spawner
       GameObject result= Instantiate(environSo.resourceSo.prefab, transform.position + Random.insideUnitSphere * 0.5f + dropOffset, Quaternion.identity);
        BaseResource baseResource = result.GetComponent<BaseResource>();
        if (baseResource != null)
        {
           baseResource.Initialize(environSo.resourceSo);
           Rigidbody rb = result.GetComponent<Rigidbody>();
           rb.isKinematic = false;
           Collider collider = result.GetComponent<Collider>();
           if (collider != null)
           {
              collider.enabled = true;
           }
           else
           {
              Debug.Log(result.name + " is missing Collider");
           }
        }
      }
      Destroy(gameObject);
   }

}



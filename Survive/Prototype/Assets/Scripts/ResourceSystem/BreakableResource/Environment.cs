using UnityEngine;
using UnityEngine.Serialization;

public class Environment : MonoBehaviour,ItakeDamage,IsoInitializer<EnvironSo>
{
   [FormerlySerializedAs("ResourceDropCount")] [SerializeField] protected int resourceDropCount;
   [SerializeField] protected Vector3 dropOffset;
   public  EnvironSo environSo{get;private set;}
   protected int currentHealth;
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
   private void OnCollisionEnter(Collision other)
   {
      BaseWeapon weapon = other.gameObject.GetComponent<BaseWeapon>();
      if (weapon != null)
      {
         int damage = weapon.MaxDamage;
         Vector3 contactPoint = other.contacts[0].point;
         TakeDamage(damage,contactPoint);   
      }
   }
   public void TakeDamage(int damage,Vector3 contactPoint)
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
      for (int i = 0; i < resourceDropCount; i++)
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



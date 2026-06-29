using System;
using UnityEngine;

public class ArrowScript : MonoBehaviour,ICollectable   
{
    [SerializeField] private GameObject TestHitPoint;
   [SerializeField] private Vector3 offset;
   private ResourceType type;
   public  ResourceSo So { get; set; }
   public bool canBeCollected { get; set; }
   public GameObject Gm { get; private set; }
   private Rigidbody rb;
   private bool _isStuck;

   private void Awake()
   {
       Gm = gameObject;
       canBeCollected = true;
   }

   private void Start()
   {
       if (Display.displays.Length > 1)
           Display.displays[1].Activate();

   }

   public void Init(Collider PlayerCollider)
   {
       Collider arrowCollider = GetComponent<Collider>();
       Physics.IgnoreCollision(PlayerCollider, arrowCollider);

   }
   private void FixedUpdate()
   {
       rb = gameObject.GetComponent<Rigidbody>();
       if (rb != null && rb.linearVelocity.sqrMagnitude > 0.01f&& !_isStuck)
       {
           transform.forward = rb.linearVelocity.normalized;
       }
   }
   private void OnCollisionEnter(Collision collision)
    {
        if (_isStuck) return;
    _isStuck = true;
    IArrowStickable stickable = collision.collider.GetComponent<IArrowStickable>();
    if (stickable == null){Debug.Log("Stickable null" + collision.collider.gameObject.name);return;}
    Debug.Log(collision.gameObject.name);
    var rb = GetComponent<Rigidbody>();
    Debug.Log(collision.gameObject.name);
    rb.linearVelocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;
    rb.isKinematic = true;
    rb.detectCollisions = false;
    // Disable collider immediately to prevent bounce
    var col = GetComponent<Collider>();
    col.enabled = false;
    
    
    
   
   
    // Use exact physics hit point
    ContactPoint cp = collision.contacts[0];
    Instantiate(TestHitPoint, cp.point, Quaternion.identity);
    // Stick arrow
    transform.position = cp.point;
    transform.rotation = Quaternion.LookRotation(-cp.normal);

    // Slight offset so arrow doesn't clip
    transform.position += transform.forward * -0.05f;

    // Parent to target
    transform.SetParent(collision.transform, true);

    stickable.TakeDamage(cp.point);
}


    
    public void Collect(PlayerInventory  collector)
    {
        canBeCollected = false;
        collector.AddResource(this);
    }

    public void ToggleMenu()
    {
        
    }
}

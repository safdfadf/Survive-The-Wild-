using System;
using System.Collections;
using UnityEngine;


public class PlayerBow : BaseWeapon
{
    [SerializeField]private GameObject arrowPrefab;
    [SerializeField] private float _mixArrowSpeed = 0f;
    [SerializeField]private float _maxArrowSpeed = 30f;
    [SerializeField]private float gravityDuration;
 // [SerializeField] private Transform arrowRestPoint;
   [SerializeField] private Transform bowString;
    private int Calltime;
    [SerializeField]private int pullSpeed;
    
  
    
        
    public GameObject CurrentArrow { get;private set; }
    protected override void Awake()   
    {
        inventoryRotAngle = -90;
        isLeftHanded = true;
        isAimable = true;
        Gm= gameObject;
        transform.rotation = Quaternion.Euler(0, 0, -22);
        base.Awake();
    }
    protected override void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            if (isAiming && CurrentArrow != null)
            {
                UpdateBowRotation();
                PullArrow();
                base.Update();
            }
        }
    }

    private void LateUpdate()
    {
        player.SetSpineControl(isAiming);
    }

    private void PullArrow()
    {
        drawTime += Time.deltaTime;
        drawTime = Mathf.Clamp(drawTime, 0f, maxDrawTime);
        float drawPercent = Mathf.Clamp01(drawTime / maxDrawTime);
        float drawOffset = Mathf.Lerp(0f, -0.5f, drawPercent);
        CurrentArrow.transform.localPosition = new Vector3(0f, 0f, drawOffset);
        
    }
    private void PrepareNextArrow()// this function should be here 
    {
        animator.DrawArrow();
        CurrentArrow = playerInventory.GetNextArrow();
        
        if(CurrentArrow==null){Debug.Log("no arrow found");}
        if (CurrentArrow != null)
        {
            ArrowScript Arrow = CurrentArrow.GetComponent<ArrowScript>();
            if(Arrow == null){Debug.Log("no arrow found");}
            Arrow.canBeCollected = false;
            CurrentArrow.transform.SetParent(arrowRestPoint,false);
            CurrentArrow.transform.localPosition = Vector3.zero;
            CurrentArrow.transform.localRotation = Quaternion.identity;
            CurrentArrow.SetActive(true);
        }
    }
    public override void StartAiming()
    {
      if (CurrentArrow == null) 
      {
            PrepareNextArrow();
      }
      base.StartAiming();
    }
    private void UpdateBowRotation()
    {
        Vector3 direction = (aimTarget.position - arrowRestPoint.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        arrowRestPoint.rotation = Quaternion.Slerp(arrowRestPoint.rotation, lookRotation, Time.deltaTime * 10f);
    }
    protected override void Shoot()
    {
        animator.FireArrow(true);
        playerInventory.RemoveArrow(this.gameObject);
        CharacterController playerConytoller = playerInventory.GetComponent<CharacterController>();
      
        if(!isAimable)return;
        if (CurrentArrow == null)
        {
            return;
        }
        Rigidbody rb = CurrentArrow.GetComponent<Rigidbody>();
        ArrowScript arrowScript = CurrentArrow.GetComponent<ArrowScript>();
        if (arrowScript != null)
        {
            arrowScript.canBeCollected = true;
        }

        if (playerConytoller != null)
        {
            arrowScript.Init(playerConytoller);
        }
        if (rb == null)
        {
            rb = CurrentArrow.AddComponent<Rigidbody>();
        }
        if (CurrentArrow== null&&!isAiming) return;
        
        float powerPercent = drawTime / maxDrawTime;
        float  arrowSpeed = Mathf.Lerp(_mixArrowSpeed, _maxArrowSpeed, powerPercent);
        CurrentArrow.transform.SetParent(null);
       
       if (rb == null)
       {
           Debug.Log("no rb");
           return;
       }
       if (rb.isKinematic)
       {
           rb.isKinematic = false;
       }
    
        rb.mass = .3f;
      
        Vector3 shootDirection = (aimTarget.position - arrowRestPoint.position ).normalized;
       // CurrentArrow.transform.rotation =Quaternion.Euler(shootDirection.normalized);
        
        rb.AddForce(shootDirection * arrowSpeed, ForceMode.Impulse);
        StartCoroutine(animator.ResetFireArrow());
        
        CurrentArrow = null;
        drawTime = 0f;
        isAiming = false;
    }
    IEnumerator EnableGravity(Rigidbody rb)
    {
        yield return new WaitForSeconds(gravityDuration);
        rb.useGravity = true;
    }
}

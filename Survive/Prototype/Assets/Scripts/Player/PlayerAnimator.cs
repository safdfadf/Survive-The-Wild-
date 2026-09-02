using System;
using System.Collections;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private int upperBodyLayerIndex;

    private void Awake()
    {
        upperBodyLayerIndex = animator.GetLayerIndex("UpperBody"); 
    }
    
    public void MovePlayer(float forward,float starfe)
    {
        // this relies on 2 inputs to make sure animation stop if input is not being
        animator.SetFloat("Forward", forward);
        animator.SetFloat("Strafe", starfe);
    }

    public void TriggerSprint(bool isSprinting)
    {
        animator.SetBool("Sprint",isSprinting);
    }

    public void BowLayer(bool equipped)
    {
        float targetWeight = equipped ? 1f : 0f;
        float currentWeight = animator.GetLayerWeight(upperBodyLayerIndex);
        float newWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * 10f);
        animator.SetLayerWeight(upperBodyLayerIndex, newWeight);
    }
    public void DrawArrow()
    {
        animator.SetTrigger("Draw");
    }
    public void Aim(bool isAiming)
    {
        animator.SetBool("Aim", isAiming);
    }

    public void FireArrow(bool isFire)
    {
        animator.SetBool("Fire",isFire);
      
    }

    public IEnumerator ResetFireArrow()
    {
        yield return new WaitForSeconds(0.1f);
        FireArrow(false);
    }

    public void HandWeaponEquip(bool isEquipped)
    {
        animator.SetBool("SwordEquip",isEquipped);
    }
    public void SwordAttack()
    {
        animator.SetTrigger("SwordAttack");
    }

    public void SwordBlock()
    {
        animator.SetTrigger("SwordBlock");
    }
}

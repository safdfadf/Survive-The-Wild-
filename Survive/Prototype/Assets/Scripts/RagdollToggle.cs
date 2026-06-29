using System;
using System.Collections.Generic;
using UnityEngine;

public class RagdollToggle : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    
    [SerializeField] private List<GameObject> ragdollGm;
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
      //  RagdollActive(false);
    }
    public void RagdollActive(bool active)
    {
//        Debug.Log("RagdollActive " + active);
        animator.enabled = !active;

        // Toggle ragdoll bones
        foreach (var gm in ragdollGm)
        {
            var col = gm.GetComponent<Collider>();
            var rbChild = gm.GetComponent<Rigidbody>();

            col.enabled = active;
            rbChild.isKinematic = !active;
            rbChild.detectCollisions = active;
        }

        // Root Rigidbody must be disabled when ragdoll is active
        rb.isKinematic = active;
        rb.detectCollisions = !active;
    }
}

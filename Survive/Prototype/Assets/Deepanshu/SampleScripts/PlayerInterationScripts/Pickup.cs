using UnityEngine;

public class Pickup : MonoBehaviour
{
    private PlayerInputs playerInputs;
    private GameObject heldItem;
    public Transform holdPoint;
    public float pickupRange = 2f;

    private void Awake()
    {
        playerInputs = new PlayerInputs();
    }

    private void OnEnable()
    {
        playerInputs.PlayerInteract.Interact.performed += ctx => TryInteract();
        playerInputs.Enable();
    }

    private void OnDisable()
    {
        playerInputs.PlayerInteract.Interact.performed -= ctx => TryInteract();
        playerInputs.Disable();
    }

    private void TryInteract()
    {
        if (heldItem == null)
        {
            GameObject closestItem = FindClosestInteractable();
            if (closestItem != null)
            {
                PickupItem(closestItem);
            }
        }
        else
        {
            DropItem();
        }
    }

    private GameObject FindClosestInteractable()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange);
        GameObject closestItem = null;
        float closestDistance = pickupRange;

        foreach (var collider in colliders)
        {
            Iinteractable interactable = collider.GetComponent<Iinteractable>();
            if (interactable != null)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestItem = collider.gameObject;
                }
            }
        }

        return closestItem;
    }

    private void PickupItem(GameObject item)
    {
        heldItem = item;
        heldItem.transform.SetParent(holdPoint);
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.localRotation = Quaternion.identity;
        heldItem.GetComponent<Rigidbody>().isKinematic = true;

        Debug.Log($"{heldItem.name} picked up!");
    }

    private void DropItem()
    {
        heldItem.transform.SetParent(null);
        heldItem.GetComponent<Rigidbody>().isKinematic = false;
        heldItem = null;

        Debug.Log("Dropped item.");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}

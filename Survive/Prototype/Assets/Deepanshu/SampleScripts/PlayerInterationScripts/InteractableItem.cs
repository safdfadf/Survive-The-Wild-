using UnityEngine;

public class InteractableItem : MonoBehaviour, Iinteractable
{
    public void Interact()
    {
        Debug.Log(gameObject.name);
    }
}

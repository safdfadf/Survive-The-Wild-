using UnityEngine;

public class TargetPractice : MonoBehaviour
{
   [SerializeField] private int health;
   [SerializeField] private int basedamage;
    
    public void StickArrow(GameObject arrow, ContactPoint contact, Vector3  offset)
    {
        arrow.transform.SetParent(transform);
        arrow.transform.position = contact.point + offset;
        arrow.transform.rotation = Quaternion.LookRotation(-contact.normal);
    }

    public void TakeDamage(int damage)
    {
        if(health <= 0)return;
        int totatDamage = basedamage * damage; // health = 100, 10 * 8
        health -= totatDamage;
        if (health <= 0)
        {
           Debug.Log(gameObject.name + " is dead");
        }
    }
}

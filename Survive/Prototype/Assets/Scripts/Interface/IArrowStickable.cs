using UnityEngine;

public interface IArrowStickable
{
     public void StickArrow(GameObject arrow, Vector3 point, Vector3 offset, Vector3 normal);
     void TakeDamage(Vector3 contactPoint);
}

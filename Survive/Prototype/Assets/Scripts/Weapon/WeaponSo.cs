using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "WeaponSo", menuName = "Scriptable Objects/WeaponSo")]

public class WeaponSo : ObjSo
{
    [FormerlySerializedAs("maxDamage")] public int damage;
    [FormerlySerializedAs("maxBlock")] public int block;
    public int scale;//ui
}

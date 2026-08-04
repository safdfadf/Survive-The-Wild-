using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CraftingSO", menuName = "Scriptable Objects/CraftingSO")]
public class CraftingSO : ScriptableObject 
{
    public Sprite logo; 
    public Ingredient[] ingredients;
    public GameObject resultPrefab;
   public int maxDamage;
    public int maxBlock;
}

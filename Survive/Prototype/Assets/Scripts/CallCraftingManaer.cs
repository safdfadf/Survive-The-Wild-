using UnityEngine;
using UnityEngine.UI;

public class CallCraftingManaer : MonoBehaviour
{
    private BaseObj _baseObj;
    public void Awake()
    {
        _baseObj = GetComponentInParent<BaseObj>();
        
    }
    private void HarvestMe()
    {
        
    }
}

using UnityEngine;
using UnityEngine.UI;

public class CallCraftingManaer : MonoBehaviour
{
    private BaseResource _baseResource;
 

    public void Awake()
    {
        _baseResource = GetComponentInParent<BaseResource>();
        
    }

    

    private void HarvestMe()
    {
        
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class StructureUI : MonoBehaviour
{
    public TextMeshProUGUI resourceName;
    public TextMeshProUGUI amount;
    public GameObject parent;

    public void SetDescription(string description , int amount)
    {
        this.resourceName.text = description;
        this.amount.text = amount.ToString();
    }

    public void ToggleDescription(bool toggle)
    {
        parent.SetActive(toggle);
    }
}

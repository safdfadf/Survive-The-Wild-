using UnityEngine;
using UnityEngine.UI;

public class WoundUI : MonoBehaviour
{
    [SerializeField] private Image woundStatusImage;

    public void SetImage(Sprite sprite)
    {
        woundStatusImage.sprite = sprite;
    }
}
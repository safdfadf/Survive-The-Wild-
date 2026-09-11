using UnityEngine;
using UnityEngine.UI;

public class WoundUI : MonoBehaviour
{
    [SerializeField] private Image woundStatusImage;
    [SerializeField] private Slider woundStatusSlider;
    public void SetImage(Sprite sprite)
    {
        woundStatusImage.sprite = sprite;
    }
}
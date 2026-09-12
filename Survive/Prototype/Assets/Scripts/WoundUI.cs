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

    public void UpdateSlider(float elapsedTime, float maxTime)
    {
        float normalized = maxTime - elapsedTime / maxTime;
        normalized = Mathf.Clamp01(normalized);
        woundStatusSlider.value = normalized;
    }
}
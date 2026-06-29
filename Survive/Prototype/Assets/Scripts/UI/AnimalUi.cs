using System;
using UnityEngine;
using UnityEngine.UI;

public class AnimalUi : MonoBehaviour
{
    public AnimalData data;
    private Camera cam;
    private GameObject animalInstance;
    private bool _canFollow;
    private RectTransform rectTransform;
    private Image image;
    private Color _color;

    // this scirpt can hold refrence to other text and stuff which can be activated by player as it progresses 
    public void Initialize(AnimalData animalData)
    {
        Debug.Log("Initialize");
        data = animalData;
        cam = Camera.main;
        animalInstance = data.AnimalInstance;
        if (animalInstance == null)
            Debug.Log("Animal instance is null");
        _canFollow = true;
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        _color = image.color;
       
    }

    private void LateUpdate()
    {
        FollowAnimalInstance();
    }

    private void FollowAnimalInstance()
    {
        if(!_canFollow)return;
        AnimalBase animal = animalInstance.GetComponent<AnimalBase>();
        Vector3 worldPos = animal.GetFollowPoint().transform.position;
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);
        Vector3 dirToAnimal = (worldPos - cam.transform.position).normalized;
        bool isBehind = Vector3.Dot(cam.transform.forward, dirToAnimal) <= 0f;

        _color.a = isBehind ? 0f : 1f;
        image.color = _color;
        RectTransform canvasRect = UIManager.instance.worldCanvas.GetComponent<RectTransform>();


        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            null, // Screen Space Overlay uses null
            out anchoredPos
        );

        rectTransform.anchoredPosition = anchoredPos;
    }
}

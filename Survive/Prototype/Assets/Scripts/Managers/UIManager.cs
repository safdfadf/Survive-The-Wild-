using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Canvas worldCanvas;
    private List<GameObject> activeSoundUI = new();

    [SerializeField] private Image noiseImage;

    [Header("Compass System UI")] // this should be handled by player ui handler 
    [SerializeField]
    private Image windDir;

    [SerializeField] private Image compassRing;
    [SerializeField] private Button InteractButton;
    [Header("Cooking")] private List<Slot> cookingSlots = new();
    [SerializeField] private Slot cookingSpotUIPrefab;


    private float currentScale;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        ToggleInteractButton(false);
    }

    private void LateUpdate()
    {
        if (cookingSlots.Count != 0)
        {
            UpdateCookingSpots();
        }
    }

    public void SetAnimalUI(GameObject obj)
    {
        RectTransform rectTransform = worldCanvas.GetComponent<RectTransform>();
        obj.transform.SetParent(rectTransform, false);
        activeSoundUI.Add(obj);
    }

    public void ToggleSoundUI(bool toggle)
    {
        foreach (var UI in activeSoundUI)
        {
            UI.SetActive(toggle);
        }
    }

    public void UpdateWindDir(Quaternion quat)
    {
        windDir.rectTransform.localRotation = quat;
    }

    public void UpdateCompassRing(Quaternion quat)
    {
        compassRing.rectTransform.localRotation = quat;
    }

    public void ToggleInteractButton(bool toggle)
    {
        InteractButton.gameObject.SetActive(toggle);
    }

  

    public Vector2 WorldToCanvasPosition(Vector3 worldPos)
    {
        // when acrivate update them so they follow player movements 
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            worldCanvas.transform as RectTransform,
            screenPos,
            worldCanvas.worldCamera,
            out Vector2 canvasPos
        );

        return canvasPos;
    }

    public void UpdateCookingSpots()
    {
        for (int i = 0; i < cookingSlots.Count; i++)
        {
            Vector2 canvasPos = WorldToCanvasPosition(cookingSlots[i].worldPosition);
            cookingSlots[i].rect.anchoredPosition = canvasPos;
        }
    }

    public void DisplayCookingSpots(List<Transform> worldPositions,GameObject handler)
    {
        ClearAllCookingSpots();
        for (int i = 0; i < worldPositions.Count; i++)
        {
            Vector2 canvasPos = WorldToCanvasPosition(worldPositions[i].position);

            Slot uiSlot = Instantiate(cookingSpotUIPrefab, worldCanvas.transform);
            uiSlot.cookingData = new CookingData(SlotType.CookingSpot,handler);
            uiSlot.cookingSpotIndex = i;
            uiSlot.worldPosition = worldPositions[i].position;
            uiSlot.rect.anchoredPosition = canvasPos;
            cookingSlots.Add(uiSlot);
        }
    }

    public Slot GetCookingSlotUnderMouse(Vector3 screenPos)
    {
        foreach (var slot in cookingSlots)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(slot.rect, screenPos, worldCanvas.worldCamera))
            {
                return slot;
            }
        }

        return null;
    }

    public void ClearAllCookingSpots()
    {
        Debug.Log("clear cooking spots");
        foreach (var spot in cookingSlots)
            Destroy(spot.gameObject);

        cookingSlots.Clear();
    }
}
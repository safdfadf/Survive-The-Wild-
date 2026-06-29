using System;
using System.Collections.Generic;
using System.Diagnostics;
using Player;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class UIManager : MonoBehaviour
{
  public static UIManager instance;
  public Canvas worldCanvas;
  private List<GameObject> activeSoundUI = new();
  [SerializeField]private Image noiseImage;

 
  [Header("Compass System UI")]// this should be handled by player ui handler 
  [SerializeField]private Image windDir; 
  [SerializeField]private Image compassRing;
  [SerializeField] private Button InteractButton;
  
  // responsiveness

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
  
}

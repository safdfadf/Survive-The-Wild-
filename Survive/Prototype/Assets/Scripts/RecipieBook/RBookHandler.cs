using System;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.UI;

public class RBookHandler : MonoBehaviour
{
    [Header("Tags")]
    [SerializeField] private Button weaponTag;
    [SerializeField] private Button baseBuildTage;
    [SerializeField] private Button trapsTag;
    [SerializeField] private Button fireTag;
    
    [SerializeField] private Transform page1;
    [SerializeField] private Transform page2;
    
    [Header("Arrows")]    
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject previousButton;
    [Header("RecipePrefab")]
    [SerializeField] private GameObject recipiePrefab;
    [Header("ParentObj")]
    [SerializeField]private GameObject parentObj;

    private List<CraftingSO> _weaponRecipes;
   // private List<CraftingSO> trapRecipes;
    private List<CraftingSO> _baseRecipes;
    [Header("Crafting Handler")]
    private CraftingHandler _craftingHandler;
    
    private List<CraftingSO> activeList;
    private int currentIndex = 0;
    private List<GameObject> spawnedSlots = new();
    private void Awake()
    {
        weaponTag.onClick.AddListener(ShowWeaponRecipe);
        baseBuildTage.onClick.AddListener(ShowBaseBuildRecipe);
        trapsTag.onClick.AddListener(ShowTrapsRecipe);
        fireTag.onClick.AddListener(ShowFireRecipe);
    }

    public void ToggleRBook()
    {
        parentObj.SetActive(!parentObj.activeSelf);
        if (parentObj.activeSelf)
        {
            PlayerRepository.instance.CanPlayerMove(false);
            ShowWeaponRecipe();// or jorney so far 
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
        }
        else
        {
            PlayerRepository.instance.CanPlayerMove(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void EnableRecipeBook()
    {
        parentObj.SetActive(true);
    }

    public void DisableRecipeBook()
    {
        parentObj.SetActive(false);
    }

    public void CategorizeRecipes(List<CraftingSO> allRecipes, CraftingHandler craftingHandler)
    {
       
        _craftingHandler= craftingHandler;
        _weaponRecipes = new List<CraftingSO>();
  //      trapRecipes = new List<CraftingSO>();
        _baseRecipes = new List<CraftingSO>();

        foreach (var so in allRecipes)
        {
            GameObject prefab = so.resultPrefab;

            if (prefab.TryGetComponent<BaseWeapon>(out _))
            {
                _weaponRecipes.Add(so);
               
            } 
            //   else if (prefab.TryGetComponent<Trap>(out _))
  //              trapRecipes.Add(so);
            else if (prefab.TryGetComponent<BaseStructure>(out _))
                _baseRecipes.Add(so);
         
        }

       
    }
    private void ShowPage()
    {
      
        // Clear old slots
        foreach (var slot in spawnedSlots)
            Destroy(slot);
        spawnedSlots.Clear();
      
        Transform currentTransform;
        // Show 4 recipes starting from currentIndex
        for (int i = 0; i < 4; i++)
        {
            currentTransform = i <= 1 ? page1 : page2;
            int index = currentIndex + i;
            if (index >= activeList.Count) break;

            GameObject r = Instantiate(recipiePrefab, currentTransform);
            r.GetComponent<Recipie>().Initialize(activeList[index],_craftingHandler );
            spawnedSlots.Add(r);
        }
        // Update buttons
        previousButton.SetActive(currentIndex > 0);
        nextButton.SetActive(currentIndex + 4 < activeList.Count);
    }
    public void NextPage()
    {
        if (currentIndex + 4 < activeList.Count)
        {
            currentIndex += 4;
            ShowPage();
        }
    }

    public void PreviousPage()
    {
        if (currentIndex > 0)
        {
            currentIndex -= 4;
            ShowPage();
        }
    }

    private void ShowWeaponRecipe()
    {
        activeList = _weaponRecipes;
        currentIndex = 0;
        ShowPage();
    }

    private void ShowBaseBuildRecipe()
    {
        activeList =_baseRecipes;
        currentIndex = 0;
        ShowPage();
    }

    private void ShowTrapsRecipe()
    {
        
    }

    private void ShowFireRecipe()
    {
     
    }
}


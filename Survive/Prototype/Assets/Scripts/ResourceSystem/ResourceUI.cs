using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DefaultNamespace.ResourceSystem
{
    public class ResourceUI : MonoBehaviour
    {
        [SerializeField] private GameObject MainMenu;
        [SerializeField] private GameObject subMenu;
        [SerializeField] private TextMeshProUGUI discriptionText;
        [SerializeField] private Button craftButton;
        [SerializeField] private Button harvestButton;
        [SerializeField] private Button useMeButton;
        [SerializeField] private Button removeButton;
        private bool canCraft;
        private bool canHarvest;
        private bool canUse;
        private List<Button> activeButtons = new() { };
        private ObjSo so;

        private void Awake()
        {
          
        }

        public void Init(ObjSo so, bool canCraft, bool canHarvest, bool canUse)
        {
            this.so = so;
            this.canCraft = canCraft;
            this.canHarvest = canHarvest;
            this.canUse = canUse;
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            activeButtons = new List<Button>
            {
                canCraft ? craftButton : null,
                canHarvest ? harvestButton : null,
                canUse ? useMeButton : null,
                removeButton
            }.Where(b => b != null).ToList();
        }

        public void ToggleMenu()
        {
            MainMenu.SetActive(!MainMenu.activeSelf);
        }
        public void ToggleSubMenu()
        {
            subMenu.SetActive(!subMenu.activeSelf);
            foreach (var button in activeButtons)
            {
                button.gameObject.SetActive(!subMenu.activeSelf);
            }
        }


        public void Craft()
        {
        }

        public void Remove()
        {
        }

        public void Harvest()
        {
        }

        public void SetDescription(string description)
        {
        }

        public void Valid()
        {
        }

        public void Invalid()
        {
        }

        public void SetUseMe(UnityAction action)
        {
            useMeButton.onClick.AddListener(action);
        }
    }
}
using TMPro;
using UnityEngine;

namespace DefaultNamespace.Interface
{
    public interface IAction// ToDo : replace with more suitable name
    {
        public string useMeDescription{get;set;}
        public string Description{get;set;}
        public bool canUse{get;set;}
        public bool canHarvest{get;set;}
        public bool canCraft{get;set;}
        public void Craft();
        public void Harvest();
        public void UseMe();
        public GameObject obj{get;set;}
    }
}
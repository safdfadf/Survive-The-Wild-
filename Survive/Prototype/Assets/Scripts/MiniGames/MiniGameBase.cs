using SurviveTheWild.CraftingSystem.Building.CampFire;
using UnityEngine;
using UnityEngine.UI;

namespace SurviveTheWild.MiniGames
{
    public class MiniGameBase : MonoBehaviour
    {
        public virtual void Initialize(CampFire campFire){}//ToDo: replace campfire with interface 
        public virtual void OnLeftClick(){}
        public virtual void OnRightClick(){}
    }
}
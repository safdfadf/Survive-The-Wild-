using SurviveTheWild.CraftingSystem.Building.CampFire;
using SurviveTheWild.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace SurviveTheWild.MiniGames
{
    public class MiniGameBase : MonoBehaviour
    {
        public virtual void Initialize(CampFire campFire,MiniGameHandler miniGameHandler){}//ToDo: replace campfire with interface 
        public virtual void OnLeftClick(){}
        public virtual void OnRightClick(){}
        public virtual void EndGame(){}
    }
}
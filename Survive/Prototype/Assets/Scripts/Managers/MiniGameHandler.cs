using SurviveTheWild.CraftingSystem.Building.CampFire;
using UnityEngine;
using SurviveTheWild.MiniGames;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SurviveTheWild.Managers
{
    public class MiniGameHandler : MonoBehaviour
    {
        private MiniGameBase _currentMiniGame;
      
        public void StartGame(MiniGameBase miniGame, CampFire campfire) // Todo use interface instead of campfire
        {
            _currentMiniGame = miniGame;
            _currentMiniGame.Initialize(campfire);
            SceneRoot.Instance.EventManager.MiniGamesEvents.StartFireGame();
        }

        public void LeftClick(InputAction.CallbackContext ctx)
        {
            if (ctx.phase == InputActionPhase.Performed)
                _currentMiniGame?.OnLeftClick();
        }

        public void RightClick(InputAction.CallbackContext ctx)
        {
            if (ctx.phase == InputActionPhase.Performed)
                _currentMiniGame?.OnRightClick();
        }
    }
}
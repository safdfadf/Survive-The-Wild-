using SurviveTheWild.CraftingSystem.Building.CampFire;
using SurviveTheWild.Interface;
using UnityEngine;
using SurviveTheWild.MiniGames;
using UnityEngine.InputSystem;

namespace SurviveTheWild.Managers
{
    public class MiniGameHandler : MonoBehaviour,IRequestInput
    {
        public MiniGameBase _currentMiniGame { get; set; }

        public void StartGame(MiniGameBase miniGame, CampFire campfire) // Todo use interface instead of campfire
        {
            if (_currentMiniGame != null) return;
            _currentMiniGame = Instantiate(miniGame);
            _currentMiniGame.Initialize(campfire,this);
            SceneRoot.Instance.EventManager.playerEvents.onDeliverRequestInput(this);
        }

        private void LeftClick(InputAction.CallbackContext ctx)
        {
            if (ctx.phase == InputActionPhase.Performed)
                _currentMiniGame?.OnLeftClick();
        }

        private void RightClick(InputAction.CallbackContext ctx)
        {
            print("rightClick pressed");
            if (ctx.phase == InputActionPhase.Started)
                _currentMiniGame?.OnRightClick();
        }

        public void UseInput(InputAction.CallbackContext ctx)
        {
            if (ctx.action.name == "Left")
            {
                LeftClick(ctx);
            }
            else if (ctx.action.name == "Right")
            {
                RightClick(ctx);
            }
        }

        public void ResetInput()
        {
            _currentMiniGame?.EndGame();
        }
    }
}
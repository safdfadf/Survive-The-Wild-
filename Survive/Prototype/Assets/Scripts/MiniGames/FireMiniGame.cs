using System;
using SurviveTheWild.CraftingSystem.Building.CampFire;
using SurviveTheWild.EventBus;
using SurviveTheWild.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace SurviveTheWild.MiniGames
{
    public class FireMiniGame : MiniGameBase
    {
        private CampFire _currentCampFire;
        private float progress;
        [SerializeField] private float maxProgress = 100f;

        [SerializeField] private float progressPerClick = 5f;
        [SerializeField] private float drainRate = 8f;

        [SerializeField] private float stamina = 100f;
        [SerializeField] private float staminaDrainPerClick = 2f;


        private bool lastClickWasLeft;
        private MiniGameHandler _miniGameHandler;
        private bool OnGameStart { get; set; }

        public override void Initialize(CampFire campFire, MiniGameHandler miniGameHandler)
        {
            progress = 0f;
            _currentCampFire = campFire;
            OnGameStart = true;
            _miniGameHandler = miniGameHandler;
        }

        private void Update()
        {
            UpdateGame();
        }

        public override void OnLeftClick()
        {
            print("calling left click");
            if (!lastClickWasLeft)
            {
                progress += progressPerClick;
                SceneRoot.Instance.EventManager.playerEvents.OnDrainStamina(staminaDrainPerClick);
                lastClickWasLeft = true;
            }
        }

        public override void OnRightClick()
        {
            print("calling right click");
            if (lastClickWasLeft)
            {
                progress += progressPerClick;
                SceneRoot.Instance.EventManager.playerEvents.OnDrainStamina(staminaDrainPerClick);
                lastClickWasLeft = false;
            }
        }

        // i should give player a chance to get out of mini game 
        private void UpdateGame()
        {
            if (!OnGameStart) return;
            // lock player 

            var slider = UIManager.instance.loadingSlider;
            slider.gameObject.SetActive(true);
            UIManager.instance.ChangeLoadSliderColor(Color.red);

            progress -= drainRate * Time.deltaTime;
            progress = Mathf.Max(0, progress);
            UpdateFireVFX();
            slider.value = progress / maxProgress;
            UIManager.instance.loadingSlider.value = progress / maxProgress;
            if (progress >= maxProgress)
            {
                _currentCampFire.Ignite();

                EndGame();
            }

            if (stamina <= 0)
            {
                //    FireFailed();
                EndGame();
                OnGameStart = false;
            }
        }

        private void UpdateFireVFX()
        {
            var normalized = progress / maxProgress;

            var baseScale = Mathf.Lerp(0.2f, 0.4f, normalized);

            var flicker = Mathf.Sin(Time.time * 20f) * 0.02f;

            _currentCampFire.UpdateFireFxScale(Vector3.one * (baseScale + flicker));
        }

        public override void EndGame()
        {
            // unlock player 
            print("EndGame");
            OnGameStart = false;
            UIManager.instance.ChangeLoadColorToDefault();
            UIManager.instance.loadingSlider.gameObject.SetActive(false);
            SceneRoot.Instance.EventManager.playerEvents.ResetRequestedInput();
            SceneRoot.Instance.MiniGameHandler._currentMiniGame = null;
            Destroy(gameObject);
        }
    }
}
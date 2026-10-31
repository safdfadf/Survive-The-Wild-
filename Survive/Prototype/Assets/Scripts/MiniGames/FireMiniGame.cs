using SurviveTheWild.CraftingSystem.Building.CampFire;
using SurviveTheWild.EventBus;
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
        private bool OnGameStart { get; set; }

        public override void Initialize(CampFire campFire)
        {
            progress = 0f;
            _currentCampFire = campFire;
            UpdateGame();
        }

        public override void OnLeftClick()
        {
            if (!lastClickWasLeft)
            {
                progress += progressPerClick;
                SceneRoot.Instance.EventManager.playerEvents.OnDrainStamina(staminaDrainPerClick);
                lastClickWasLeft = true;
            }
        }

        public override void OnRightClick()
        {
            if (lastClickWasLeft)
            {
                progress += progressPerClick;
                SceneRoot.Instance.EventManager.playerEvents.OnDrainStamina(staminaDrainPerClick);
                lastClickWasLeft = false;
            }
        }

        private void UpdateGame()
        {
            if (!OnGameStart) return;
            var slider = UIManager.instance.loadingSlider;
            slider.gameObject.SetActive(true);
            UIManager.instance.ChangeLoadSliderColor(Color.red);
            while (OnGameStart)
            {
                progress -= drainRate * Time.deltaTime;
                progress = Mathf.Max(0, progress);
                UpdateFireVFX();
                slider.value = progress / maxProgress;
                UIManager.instance.loadingSlider.value = progress / maxProgress;
                if (progress >= maxProgress)
                {
                    _currentCampFire.Ignite();
                    OnGameStart = false;
                }

                if (stamina <= 0)
                {
                    //    FireFailed();

                    OnGameStart = false;
                }
            }

            UIManager.instance.ChangeLoadColorToDefault();
            UIManager.instance.loadingSlider.gameObject.SetActive(false);
            SceneRoot.Instance.EventManager.MiniGamesEvents.StartFireGame();
        }

        private void UpdateFireVFX()
        {
            var normalized = progress / maxProgress;

            var baseScale = Mathf.Lerp(0.2f, 0.4f, normalized);

            var flicker = Mathf.Sin(Time.time * 20f) * 0.02f;

            _currentCampFire.UpdateFireFxScale(Vector3.one * (baseScale + flicker));
        }
    }
}
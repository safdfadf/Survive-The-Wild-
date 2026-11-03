using System;

namespace SurviveTheWild.EventBus.Events
{
    public class MiniGamesEvents
    {
        public Action onToggleMiniGame;

        public void StartFireGame()
        {
            onToggleMiniGame();
        }
    }
}
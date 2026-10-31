using System;

namespace SurviveTheWild.EventBus.Events
{
    public class MiniGamesEvents
    {
        public Action onStartFireGame;

        public void StartFireGame()
        {
            onStartFireGame();
        }
    }
}
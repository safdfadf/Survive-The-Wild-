using DefaultNamespace.EventBus;

namespace Player
{
    public class PlayerStats
    {
        public int currentlevel;
        public int currentXP;
        private PlayerUI _ui;

        public PlayerStats(PlayerUI ui)
        {
            _ui = ui;
           
        }
        public void AddXp(int xp)
        {
            currentXP += xp;
            if (currentXP >= 100)
            {
                currentXP = 0;
                currentlevel++;
            }
            _ui.UpdateLevel(currentXP, currentlevel);
        }
        
    }
}
using Utils.Interfaces;

namespace TowerDefense.States
{
    

    public class LevelStateManager : BaseStateManager
    {
        #region VARIABLES

        private LevelStateController levelStateController => stateController as LevelStateController;
        private LevelUIController levelUIController => uiController as LevelUIController;

        private int levelCurrency = 100;
        private IPlaceable selectedTower;

        #endregion

        #region BASE STATE

        public LevelStateManager(): base()
        {

        }

        public override void OnAction(GameAction action)
        {
            switch (action)
            {
                
                case GameAction.Gameplay_LevelUpTower:
                    if (selectedTower != null)
                    {
                        levelStateController.LevelUpPlaceable(selectedTower);
                    }
                    levelUIController.TriggerLevelUp(selectedTower.CanLevelUp);
                    break;
                case GameAction.Gameplay_Unselect:
                    selectedTower = null;
                    levelUIController.ShowSpawnerUI();
                    break;
            }
        }

        public override void OnAction(GameAction action, BatonPassData data)
        {
            switch(action)
            {
                case GameAction.Gameplay_SpawnTower:
                    levelStateController.SpawnTower((TowerType)data.IntData);
                    break;
                case GameAction.Gameplay_Select:
                    selectedTower = (IPlaceable) data.ExtraData;

                    levelUIController.TriggerLevelUp(selectedTower.CanLevelUp);
                    levelUIController.ShowTowerUI();
                    break;
                case GameAction.Gameplay_TakeDamage:
                    levelStateController.TakeDamage(data.FloatData);
                    break;
            }
        }

        #endregion

        public void SyncUIValues(int hordesDefeated, int totalHordeCount)
        {
            var healthPercentage = levelStateController.CurrentHealth / levelStateController.MaxHealth;
            levelUIController.UpdateUI(healthPercentage, hordesDefeated, totalHordeCount);
        }

        public bool TakeCurrency(int amount)
        {
            if (levelCurrency - amount < 0)
            {
                return false;
            }

            levelCurrency -= amount;

            return true;
        }

        public void GiveCurrency(int amount)
        {
            levelCurrency += amount;
        }
    }
}
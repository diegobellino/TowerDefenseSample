using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.States
{
    public class LevelUIController : BaseStateController
    {
        #region VARIABLES

        [Header("Player")]
        [SerializeField] private Image playerHealthBar;
        
        [Header("Hordes")]
        [SerializeField] private Image hordeCounterBar;
        [SerializeField] private Text hordeCounterText;

        [Header("Tower")]
        [SerializeField] private GameObject spawnerUI;
        [SerializeField] private GameObject towerUI;
        [SerializeField] private Button levelUpButton;

        #endregion

        public void SpawnBaseTower()
        {
            GameStateController.Instance.FireAction(GameAction.Gameplay_SpawnTower, new BatonPassData { IntData = (int)TowerType.BaseTower });
        }

        public void SpawnSlowTower()
        {
            GameStateController.Instance.FireAction(GameAction.Gameplay_SpawnTower, new BatonPassData { IntData = (int)TowerType.SlowTower });
        }

        public void LevelUpTower()
        {
            GameStateController.Instance.FireAction(GameAction.Gameplay_LevelUpTower);
        }

        public void UpdateUI(float playerHealthPercentage, int hordesDefeated, int totalHordeCount)
        {
            playerHealthBar.fillAmount = playerHealthPercentage;
            hordeCounterBar.fillAmount = (float)hordesDefeated / (float)totalHordeCount;
            hordeCounterText.text = hordesDefeated.ToString();
        }

        public void ShowSpawnerUI()
        {
            spawnerUI.SetActive(true);
            towerUI.SetActive(false);
        }

        public void ShowTowerUI()
        {
            spawnerUI.SetActive(false);
            towerUI.SetActive(true);
        }

        public void TriggerLevelUp(bool isOn)
        {
            levelUpButton.interactable = isOn;
        }
    }
}
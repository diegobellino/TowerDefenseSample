using UnityEngine;

namespace TowerDefense.GameLogic.Runtime.Configs
{
    [CreateAssetMenu(menuName = "Tower Defense/Level Config", fileName = "New Level")]
    public class LevelConfig : ScriptableObject
    {
        #region VARIABLES

        private enum WinCondition
        {
            SurviveTime,
            SurviveHordes
        }

        [SerializeField] private WinCondition winCondition;

        [SerializeField] private float timeToSurvive;

        [SerializeField] private float health;

        public float Health => health;

        #endregion

        public bool CheckWin(float elapsedTime, HordeController[] hordeControllers)
        {
            switch (winCondition)
            {
                case WinCondition.SurviveTime:
                    return elapsedTime >= timeToSurvive;
                    
                case WinCondition.SurviveHordes:
                    var won = true;
                    foreach (var hordeController in hordeControllers)
                    {
                        won &= hordeController.NoHordesLeft();

                        if (!won)
                        {
                            break;
                        }
                    }
                    return won;
            }

            return false;
        }
    }
}
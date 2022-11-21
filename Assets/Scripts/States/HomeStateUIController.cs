using TowerDefense.Levels;

namespace TowerDefense.States
{
    public class HomeStateUIController : BaseStateController
    {
        public void StartGame(LevelConfig levelConfig)
        {
            GameStateController.Instance.ChangeState(StateId.Level, levelConfig);
        }
    }
}
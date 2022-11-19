namespace TowerDefense.States
{
    public class GameOverStateUI : BaseStateController
    {
        public void OnRestart()
        {
            GameStateController.Instance.ChangeState(StateId.Level);
        }

        public void OnBackButton()
        {
            GameStateController.Instance.ChangeState(StateId.Home);
        }
    }
}
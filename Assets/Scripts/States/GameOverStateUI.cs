namespace TowerDefense.States
{
    public class GameOverStateUI : BaseStateController
    {
        public void OnBackButton()
        {
            GameStateController.Instance.ChangeState(StateId.Home);
        }
    }
}
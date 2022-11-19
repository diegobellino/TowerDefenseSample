namespace TowerDefense.States
{
    public class HomeStateUIController : BaseStateController
    {
        public void StartGame()
        {
            GameStateController.Instance.ChangeState(StateId.Level);
        }
    }
}
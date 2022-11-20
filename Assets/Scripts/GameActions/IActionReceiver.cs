
namespace TowerDefense.GameActions
{
    public interface IActionReceiver
    {
        void OnAction(GameAction action);
        void OnAction(GameAction action, BatonPassData data);
    }
}

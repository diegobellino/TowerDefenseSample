using UnityEngine;

namespace TowerDefense.States
{
    [CreateAssetMenu(menuName = "Tower Defense/New State", fileName = "New State Config")]
    public class StateConfig : ScriptableObject
    {
        public StateId StateId;

        public BaseStateController UIController;
        public BaseStateController StateController;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.GameActions
{
    public enum GameAction
    {
        Gameplay_SpawnTower,
        Gameplay_Select,
        Gameplay_Unselect,
        Gameplay_LevelUpTower,
        Gameplay_TakeDamage,
        Gameplay_EnemyDefeated
    }

    public struct BatonPassData
    {
        public int IntData;
        public float FloatData;
        public string StringData;
        public bool BoolData;
        public Vector2 Vector2Data;
        public Vector3 Vector3Data;
        public object ExtraData;
    }

    public static class GameActionManager
    {
        private static HashSet<IActionReceiver> _actionReceivers = new();
        
        public static void RegisterActionReceiver(IActionReceiver actionReceiver)
        {
            if (!_actionReceivers.Contains(actionReceiver))
            {
                _actionReceivers.Add(actionReceiver);
            }
        }

        public static void UnregisterActionReceiver(IActionReceiver actionReceiver)
        {
            if (_actionReceivers.Contains(actionReceiver))
            {
                _actionReceivers.Remove(actionReceiver);
            }
        }

        public static void FireAction(GameAction action)
        {
            foreach (var actionReceiver in _actionReceivers)
            {
                actionReceiver.OnAction(action);
            }
        }

        public static void FireAction(GameAction action, BatonPassData data)
        {
            foreach (var actionReceiver in _actionReceivers)
            {
                actionReceiver.OnAction(action, data);
            }
        }
    }
}
using UnityEngine;

namespace TowerDefense.Hordes
{
    [CreateAssetMenu(menuName = "Tower Defense/Horde Config", fileName = "New Horde")]
    public class HordeConfig : ScriptableObject
    {
        [SerializeField] private GameObject[] enemies;

        public GameObject[] Enemies => enemies;
    }
}

using UnityEngine;
using Utils.SmartUpdate;
using Utils.Interfaces;
using System.Collections.Generic;
using TowerDefense.GameLogic.Runtime;
using TowerDefense.Levels;

namespace TowerDefense.States
{
    public enum TowerType
    {
        BaseTower,
        SlowTower
    }

    /// <summary>
    /// Main Controller for the Game Logic: Handles input, controls WIN/LOSE conditions, 
    /// updates other controllers, keeps track of placeables
    /// </summary> 
    public class LevelStateController : BaseStateController, ISmartUpdate, IPlaceableController
    {
        #region VARIABLES

        [SerializeField] private LevelConfig config;
        [SerializeField] private HordeController[] hordes;
        [SerializeField] private ObjectPool pool;

        public UpdateGroup Group => UpdateGroup.Timed;
        public float CurrentHealth => currentHealth;
        public float MaxHealth => config.castleHealth;
        private LevelStateManager manager => stateManager as LevelStateManager;

        private HashSet<IPlaceable> activePlaceables = new HashSet<IPlaceable>();
        private int totalHordeCount = 0;
        private float currentHealth;

        #endregion

        #region LIFETIME

        public override void OnOpenState()
        {
            base.OnOpenState();

            SmartUpdateController.Instance?.Register(this);

            foreach(var hordeController in hordes)
            {
                totalHordeCount += hordeController.HordeCount;
            }

            currentHealth = config.castleHealth;
        }

        public void SmartUpdate(float deltaTime)
        {
            // Check win condition
            if (HasWon())
            { 
                manager.SyncUIValues(totalHordeCount, totalHordeCount);
                GameStateController.Instance.ChangeState(StateId.GameOver);
                return;
            }

            var defeatedHordes = 0;
            foreach (var hordeController in hordes)
            {
                defeatedHordes += hordeController.DefeatedHordes;
                hordeController.UpdateHordes(deltaTime);
            }

            manager.SyncUIValues(defeatedHordes, totalHordeCount);
        }

        private bool HasWon()
        {
            var won = true;
            foreach (var hordeController in hordes)
            {
                won &= hordeController.NoHordesLeft();

                if (!won)
                {
                    break;
                }
            }
            return won;
        }

        public override void OnCloseState()
        {
            base.OnCloseState();

            SmartUpdateController.Instance?.Unregister(this);

        }

        #endregion

        public void TakeDamage(float amount)
        {
            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                GameStateController.Instance.ChangeState(StateId.GameOver);
                return;
            }
        }

        #region PLACEABLE MANAGEMENT

        public void SpawnTower(TowerType type)
        {
            var towerObject = pool.RetrieveObject(type.ToString());
            var tower = towerObject.GetComponent<IPlaceable>();
            tower.RegisterController(this);
            tower.InPlacing = true;
            towerObject.SetActive(true);

            foreach (var placeable in activePlaceables)
            {
                placeable.InPlacing = true;
            }
        }

        public void PlaceableSpawned(IPlaceable placeable)
        {
            if (activePlaceables.Contains(placeable))
            {
                return; 
            }

            activePlaceables.Add(placeable);

            foreach (var activePlaceable in activePlaceables)
            {
                activePlaceable.InPlacing = false;
            }
        }

        public bool IsPlacementValid(IPlaceable placeable)
        {
            var bounds = placeable.GetBounds();
            bounds.size = bounds.size + Vector3.up * 10f;

            foreach (var spawnedPlaceable in activePlaceables)
            {
                if (spawnedPlaceable == placeable)
                {
                    continue;
                }

                var tempBounds = spawnedPlaceable.GetBounds();
                tempBounds.size = tempBounds.size + Vector3.up * 10f;

                if (bounds.Intersects(tempBounds))
                {
                    return false;
                }
            }

            return true;
        }

        public void LevelUpPlaceable(IPlaceable placeable)
        {
            placeable?.LevelUp();
        }

        #endregion
    }
}
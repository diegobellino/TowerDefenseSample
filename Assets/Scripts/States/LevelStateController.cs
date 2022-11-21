using UnityEngine;
using Utils.SmartUpdate;
using Utils.Interfaces;
using System.Collections.Generic;
using TowerDefense.Hordes;
using TowerDefense.Levels;
using UnityEditor.UIElements;

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

        [SerializeField] private TowerDefense.ObjectPool.ObjectPool pool;
        [SerializeField] private GameObject castleObject;

        public UpdateGroup Group => UpdateGroup.Timed;
        public float CurrentHealth => currentHealth;
        public float MaxHealth => config.castleHealth;
        
        private LevelConfig config;
        private LevelStateManager manager => stateManager as LevelStateManager;

        private List<HordeController> hordeControllers = new();
        private HashSet<IPlaceable> activePlaceables = new();
        private float currentHealth;
        private int totalEnemycount;
        private int defeatedEnemyCount;

        private Vector3 castlePosition;
        private List<Bounds> towerBounds = new();

        #endregion

        #region LIFETIME

        public override void OnOpenState(object stateData)
        {
            base.OnOpenState(stateData);

            SmartUpdateController.Instance.Register(this);
            
            config = stateData as LevelConfig;
            
            // Position castle and set current health
            SetupCastle();

            // Prepare every horde controller and cache them
            SetupHordeControllers();
        }

        private void SetupCastle()
        {
            castlePosition = new Vector3(config.castlePosition.x, 0, config.castlePosition.y);
            castleObject.transform.position = castlePosition;
            currentHealth = config.castleHealth;
        }

        private void SetupHordeControllers()
        {
            foreach (var hordeConfig in config.hordeConfigs)
            {
                var hordeController = pool.RetrieveObject(nameof(HordeController)).GetComponent<HordeController>();
                hordeController.Initialize(hordeConfig);
                hordeControllers.Add(hordeController);
            }

            for (int i = 0; i < config.hordeSpawnerLocations.Length; i++)
            {
                var hordePosition = config.hordeSpawnerLocations[i];
                var newPosition = new Vector3(hordePosition.x, 0, hordePosition.y);
                hordeControllers[i].transform.position = newPosition;
            }

            foreach(var hordeController in hordeControllers)
            {
                totalEnemycount += hordeController.HordeCount;
                var path = CalculatePath(hordeController.transform.position,
                    castleObject.transform.position);
                hordeController.UpdatePath(path);
                hordeController.gameObject.SetActive(true);
            }
        }

        public void SmartUpdate(float deltaTime)
        {
            // Check win condition
            if (HasWon())
            { 
                manager.SyncUIValues(totalEnemycount, totalEnemycount);
                GameStateController.Instance.ChangeState(StateId.GameOver);
                return;
            }

            foreach (var hordeController in hordeControllers)
            {
                hordeController.UpdateController(deltaTime);
            }

            manager.SyncUIValues(defeatedEnemyCount, totalEnemycount);
        }

        private bool HasWon()
        {
            return totalEnemycount <= defeatedEnemyCount;
        }

        public override void OnCloseState()
        {
            base.OnCloseState();

            SmartUpdateController.Instance.Unregister(this);
        }

        #endregion
        
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

            foreach (var hordeController in hordeControllers)
            {
                var path = CalculatePath(hordeController.transform.position,
                    castleObject.transform.position);
                hordeController.UpdatePath(path);
            }
        }

        public bool IsPlacementValid(IPlaceable placeable)
        {
            var bounds = placeable.GetBounds();
            bounds.size += Vector3.up * 10f;

            foreach (var spawnedPlaceable in activePlaceables)
            {
                if (spawnedPlaceable == placeable)
                {
                    continue;
                }

                var tempBounds = spawnedPlaceable.GetBounds();
                tempBounds.size += Vector3.up * 10f;

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

        public void TakeDamage(float amount)
        {
            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                GameStateController.Instance.ChangeState(StateId.GameOver);
                return;
            }
        }

        public void OnEnemyDefeated()
        {
            defeatedEnemyCount++;
        }

        private List<Vector3> CalculatePath(Vector3 startPosition, Vector3 endPosition)
        {
            var path = new List<Vector3>();
            path.Add(startPosition);
            var pathFound = false;
            var isHorizontalSearch = true;
            var curPathPoint = startPosition;
            var iterationLimit = 10000;
            var currentIterations = 0;
            
            while (!pathFound && currentIterations < iterationLimit)
            {
                var objectivePoint = Vector3.zero;
                
                if (isHorizontalSearch)
                {
                    objectivePoint = curPathPoint + Vector3.right * (endPosition.x - curPathPoint.x);
                }
                else
                {
                    objectivePoint = curPathPoint + Vector3.forward * (endPosition.z - curPathPoint.z);
                }

                foreach (var placeable in activePlaceables)
                {
                    var bounds = placeable.GetBounds();
                    // Tweak bounds size so ray can intersect
                    bounds.size += Vector3.up * 10;

                    var ray = new Ray(curPathPoint, objectivePoint - curPathPoint);
                    if (bounds.IntersectRay(ray, out var distance))
                    {
                        distance = Mathf.RoundToInt(distance);
                        
                        if (isHorizontalSearch)
                        {
                            if (curPathPoint.x > objectivePoint.x)
                            {
                                objectivePoint = curPathPoint - Vector3.right * distance;
                            }
                            else
                            {
                                objectivePoint = curPathPoint + Vector3.right * distance;
                            }
                        }
                        else
                        {
                            if (curPathPoint.y > objectivePoint.y)
                            {
                                objectivePoint = curPathPoint - Vector3.forward * distance;
                            }
                            else
                            {
                                objectivePoint = curPathPoint + Vector3.forward * distance;
                            }
                        }
                            
                        break;
                    }
                }

                curPathPoint = objectivePoint;
                path.Add(curPathPoint);

                isHorizontalSearch = !isHorizontalSearch;
                pathFound = curPathPoint == endPosition;

                currentIterations++;
            }
            return path;
        }
    }
}
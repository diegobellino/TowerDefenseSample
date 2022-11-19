using UnityEngine;
using TowerDefense.Towers.AI;
using TowerDefense.Towers.Config;
using Utils.SmartUpdate;
using Utils.Interfaces;
using TowerDefense.Input;
using TowerDefense.States;
using Consts = TowerDefense.Constants.Constants;

namespace TowerDefense.Towers
{
    public enum TowerState
    {
        Positioning,
        Gameplay,
        Destroy
    }

    /// <summary>
    /// Controls main flow of the tower and enables behaviours based on current state (positioning, in-game or destroyed)
    /// </summary>
    public class TowerController : MonoBehaviour, IDraggable, IPlaceable
    {
        #region VARIABLES

        public bool InPlacing { 
            set {
                hud.ShowPlacementIndicator(value && currentState == TowerState.Positioning);
                hud.ShowRangeIndicator(value);
            }
        }

        public bool CanLevelUp { get => currentLevel < (config.MaxLevel - 1); }

        public IPool Pool { private get; set; }
        public string PoolId { get => config.TowerType.ToString(); }

        [SerializeField] private UpdateGroup updateGroup;
        [SerializeField] private TowerConfig config;
        [SerializeField] private TowerAIStateController AIController;
        [SerializeField] private TowerHUD hud;

        private TowerState currentState = TowerState.Gameplay;
        private int currentLevel = 0;
        private LayerMask pathMask;
        private bool canPlaceTower;

        private IPlaceableController placeableController;

        #endregion

        #region LIFETIME

        private void Awake()
        {
            pathMask = LayerMask.GetMask("Path");
        }

        #endregion

        public void RegisterController(IPlaceableController controller)
        {
            placeableController = controller;
            transform.position = Vector3.zero + Vector3.up * Consts.TOWER_GAMEPLAY_HEIGHT;
            TriggerState(TowerState.Positioning);

            hud.SetTowerSize(config.Size);
            hud.SetTowerRange(config.GetRange(currentLevel));
        }

        private void SetupAI()
        {
            AIController.Setup(
                config.GetRange(currentLevel),
                config.GetDamage(currentLevel),
                config.GetAttackRate(currentLevel));
        }

        public void TriggerState(TowerState state)
        {
            currentState = state;
            hud.AdjustIndicatorsToState(state);

            switch (currentState)
            {
                case TowerState.Positioning:
                    InputController.Instance.SetMap("Tower Placement");
                    AIController.EnableAI(false);
                    transform.position += Vector3.up * Consts.TOWER_POSITIONING_HEIGHT;
                    break;

                case TowerState.Gameplay:
                    InputController.Instance.SetMap("Gameplay");
                    SetupAI();
                    AIController.EnableAI(true);
                    var position = transform.position;
                    transform.position = new Vector3(position.x, Consts.TOWER_GAMEPLAY_HEIGHT, position.z);
                    break;

                case TowerState.Destroy:
                    AIController.EnableAI(false);
                    break;
            }
        }

        #region POSITIONING

        public Bounds GetBounds()
        {
            var bounds = new Bounds();

            bounds.center = transform.position;
            bounds.size = new Vector3(config.Size.x - Consts.TOWER_BOUNDARY_OFFSET, 0f, config.Size.y - Consts.TOWER_BOUNDARY_OFFSET);

            return bounds;
        }

        private Vector3[] GetBoundaryPointsAroundCenter()
        {
            var bounds = GetBounds();

            return new Vector3[]
            {
                bounds.center,
                new Vector3(bounds.min.x, bounds.center.y, bounds.min.z),
                new Vector3(bounds.max.x, bounds.center.y, bounds.min.z),
                new Vector3(bounds.min.x, bounds.center.y, bounds.max.z),
                new Vector3(bounds.max.x, bounds.center.y, bounds.max.z),
            };
        }

        public bool IsPlacementValid()
        {
            if (!placeableController.IsPlacementValid(this))
            {
                hud.SetPlacementIndicatorColor(false);
                return false;
            }

            var raycastOrigins = GetBoundaryPointsAroundCenter();

            // Raycast to the ground in a square around the tower
            foreach (var origin in raycastOrigins)
            {
                if (Physics.Raycast(origin, Vector3.down, Consts.TOWER_RAYCAST_LENGTH, pathMask, QueryTriggerInteraction.Ignore))
                {
                    hud.SetPlacementIndicatorColor(false);
                    return false;
                }
            }

            hud.SetPlacementIndicatorColor(true);
            return true;
        }

        public bool CanDrag()
        {
            return currentState == TowerState.Positioning;
        }

        public void Drag(Vector2 offset)
        {
            transform.position += new Vector3(offset.x, 0, offset.y) * Consts.GRID_SIZE;

            IsPlacementValid();
        }

        public void OnDragStart()
        {

        }

        public void OnDragEnd()
        {
            if (IsPlacementValid())
            {
                placeableController.PlaceableSpawned(this);
                TriggerState(TowerState.Gameplay);
            }
        }

        #endregion

        #region PLACEABLE

        public void LevelUp()
        {
            currentLevel = Mathf.Clamp(currentLevel + 1, currentLevel, config.MaxLevel);

            SetupAI();

            hud.SetTowerRange(config.GetRange(currentLevel));
        }

        public bool CanSelect()
        {
            return currentState == TowerState.Gameplay;
        }

        public void Select()
        {
            hud.ShowRangeIndicator(true);

            var actionData = new BatonPassData
            {
                ExtraData = this
            };

            GameStateController.Instance.FireAction(GameAction.Gameplay_Select, actionData);
        }

        public void Unselect()
        {
            hud.ShowRangeIndicator(false);

            GameStateController.Instance.FireAction(GameAction.Gameplay_Unselect);
        }

        #endregion

        #region GIZMOS

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying || currentState != TowerState.Positioning)
            {
                return;
            }

            Gizmos.color = canPlaceTower ? Color.green : Color.red;

            foreach (var position in GetBoundaryPointsAroundCenter())
            {
                Gizmos.DrawLine(position, position + Vector3.down * Consts.TOWER_RAYCAST_LENGTH);
            }
        }

#endif 
        #endregion
    }
}

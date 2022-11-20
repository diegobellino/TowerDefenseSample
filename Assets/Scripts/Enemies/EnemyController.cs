using UnityEngine;
using Utils.SmartUpdate;
using TowerDefense.Enemies.Config;
using Utils.Interfaces;
using TowerDefense.GameActions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TowerDefense.Enemies
{
    public enum EnemyState
    {
        Idle,
        Attacking
    }

    /// <summary>
    /// Main controller for Enemy logic (movement, damage, etc.)
    /// </summary>
    public class EnemyController : MonoBehaviour, ISmartUpdate, IEnemy
    {
        #region VARIABLES

        private const float MINIMUM_DISTANCE_TO_TARGET = .05f;
        private const float SLOW_TIME = 2f;
        private const float SLOW_SPEED_FACTOR = 2f;

        [SerializeField] private EnemyConfig enemyConfig;
        [SerializeField] private EnemyHUD hud;

        public UpdateGroup Group => UpdateGroup.OddFrames;
        public EnemyType Type => enemyConfig.Type;
        public float Health => enemyConfig.MaxHealth - damageTaken;
        public Vector3[] pathWaypoints { private get; set; }
        public bool shouldMove { private get; set; }

        public IPool Pool { private get; set; }
        public string PoolId { get => enemyConfig.Type.ToString(); }

        private float damageTaken;
        private bool isSlowed;
        private float slowedElapsedTime;

        private int curWaypoint = 1;
        private Vector3 targetPosition = Vector3.negativeInfinity;
        private Vector3 movementVector = Vector3.negativeInfinity;
        private float currentSpeed;

        #endregion

        #region LIFETIME

        private void Start()
        {
            SmartUpdateController.Instance?.Register(this);

            currentSpeed = enemyConfig.Speed;
        }

        private void OnDestroy()
        {
            SmartUpdateController.Instance?.Unregister(this);
        }

        public void SmartUpdate(float deltaTime)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            if (isSlowed)
            {
                slowedElapsedTime += deltaTime;

                if (slowedElapsedTime >= SLOW_TIME)
                {
                    isSlowed = false;
                    slowedElapsedTime = 0f;
                    currentSpeed = enemyConfig.Speed;
                }
                else
                {
                    currentSpeed = enemyConfig.Speed / SLOW_SPEED_FACTOR;
                }

            }

            Move(deltaTime);
        }

        #endregion

        #region MOVEMENT

        private void Move(float deltaTime)
        {
            if (!shouldMove)
            {
                return;
            }

            var distanceToTarget = Vector3.Distance(targetPosition, transform.position);

            if (distanceToTarget <= MINIMUM_DISTANCE_TO_TARGET)
            {
                transform.position = targetPosition;

                curWaypoint++;

                if (curWaypoint >= pathWaypoints.Length)
                {
                    GameActionManager.FireAction(
                        GameAction.Gameplay_TakeDamage,
                        new BatonPassData
                        {
                            FloatData = enemyConfig.Damage
                        });
                    shouldMove = false;

                    Pool.PoolObject(gameObject, PoolId);
                    return;
                }

                targetPosition = pathWaypoints[curWaypoint];
                movementVector = (targetPosition - transform.position).normalized;

                transform.LookAt(targetPosition);
            }

            var newPosition = transform.position + movementVector * (currentSpeed * deltaTime);
            transform.position = newPosition;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        #endregion

        #region ENEMY

        public void TakeDamage(float amount)
        {
            damageTaken += amount;

            if (Health <= 0)
            {
                Pool.PoolObject(gameObject, PoolId);
                return;
            }

            hud.UpdateHealthBar(Health / enemyConfig.MaxHealth);
        }

        public bool IsTargeteable()
        {
            return gameObject.activeSelf && !IsDead();
        }

        public bool IsDead()
        {
            return Health <= 0;
        }

        public void ResetValues()
        {
            damageTaken = 0f;

            // Setup movement
            if (pathWaypoints?.Length >= 2)
            {
                curWaypoint = 1;
                var rawMovement = pathWaypoints[1] - pathWaypoints[0];
                targetPosition = transform.position + rawMovement;
                movementVector = rawMovement.normalized;
                shouldMove = false;
            }
        }

        public void Slow()
        {
            isSlowed = true;
            slowedElapsedTime = 0f;
        }

        #endregion

        #region GIZMOS

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Handles.Label(transform.position + Vector3.up, Health.ToString());
        }

        private void OnDrawGizmosSelected()
        {
            Handles.color = Color.red;

            Handles.DrawLine(transform.position, targetPosition);
            Handles.DrawWireCube(targetPosition, Vector3.one * .2f);
        }

#endif

        #endregion
    }
}
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Utils.SmartUpdate;
using Utils.Interfaces;

namespace TowerDefense.Input
{ 
    public class InputController : MonoBehaviour, ISmartUpdate
    {
        #region VARIABLES

        private const float RAYCAST_LENGTH = 50f;
        private const float DRAG_SPEED = 1f;
        private const float DRAG_LIMIT = .5f;

        public static InputController Instance;

        public UpdateGroup Group => UpdateGroup.Always;

        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private Camera uiCamera;

        private LayerMask towerLayerMask;
        private IDraggable currentDraggable;
        private Vector2 cumulativeDrag = Vector2.zero;
        private ISelectable currentSelectable;

        public Action<int> OnTowerSelected;
        public Action<Vector3> OnUpdateTowerPosition;

        #endregion

        #region LIFETIME

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this);

            towerLayerMask = LayerMask.GetMask("Tower");
        }

        private void Start()
        {
            SmartUpdateController.Instance?.Register(this);
        }

        private void OnDestroy()
        {
            SmartUpdateController.Instance?.Unregister(this);
        }

        public void SmartUpdate(float deltaTime)
        {
            if (currentDraggable != null && currentDraggable.CanDrag())
            {
                var delta = Touchscreen.current.primaryTouch.delta.ReadValue() * deltaTime * DRAG_SPEED;

                delta.x = Mathf.Clamp(delta.x, - DRAG_LIMIT, DRAG_LIMIT);
                delta.y = Mathf.Clamp(delta.y, - DRAG_LIMIT, DRAG_LIMIT);

                cumulativeDrag += delta;

                var moveVertical = Mathf.Abs(cumulativeDrag.y) >= DRAG_LIMIT;
                var moveHorizontal = Mathf.Abs(cumulativeDrag.x) >= DRAG_LIMIT;
                var movementVector = Vector2.zero;

                if (moveVertical)
                {
                    if (cumulativeDrag.y > 0 )
                    {
                        cumulativeDrag.y -= DRAG_LIMIT;
                        movementVector.y = 1f;
                    }
                    else
                    {
                        cumulativeDrag.y += DRAG_LIMIT;
                        movementVector.y = -1f;
                    }
                }

                if (moveHorizontal)
                {
                    if (cumulativeDrag.x > 0)
                    {
                        cumulativeDrag.x -= DRAG_LIMIT;
                        movementVector.x = 1f;
                    }
                    else
                    {
                        cumulativeDrag.x += DRAG_LIMIT;
                        movementVector.x = -1f;
                    }
                }

                if (moveVertical || moveHorizontal)
                {
                    currentDraggable.Drag(movementVector);
                }
            }
        }

        #endregion

        public void SetMap(string mapName)
        {
            playerInput.SwitchCurrentActionMap(mapName);
        }

        #region PLAYER_INPUT_MESSAGES

        private void OnSelect(InputValue value)
        {
            var touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            var touchOrigin = new Vector3(touchPosition.x, touchPosition.y, Camera.main.nearClipPlane);
            var ray = Camera.main.ScreenPointToRay(touchOrigin);
            
            if (Physics.Raycast(ray, out var hit, RAYCAST_LENGTH, towerLayerMask))
            {
                if (hit.transform.TryGetComponent<ISelectable>(out var selectable))
                {
                    if (selectable == currentSelectable)
                    {
                        return;
                    }

                    currentSelectable?.Unselect();
                    selectable.Select();
                    currentSelectable = selectable;
                    return;
                }
            }

            currentSelectable?.Unselect();
            currentSelectable = null;
        }

        private void OnMove(InputValue value)
        {
            if (currentDraggable != null)
            {
                return;
            }

            cumulativeDrag = Vector2.zero;

            var touchPosition = Touchscreen.current.position.ReadValue();
            var ray = Camera.main.ScreenPointToRay(new Vector3(touchPosition.x, touchPosition.y, Camera.main.nearClipPlane));

            if (Physics.Raycast(ray, out var hit, RAYCAST_LENGTH, towerLayerMask))
            {
                if (hit.transform.TryGetComponent<IDraggable>(out var draggable))
                {
                    currentDraggable = draggable;
                }
            }
        }

        private void OnPlace(InputValue value)
        {
            var touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            var ray = Camera.main.ScreenPointToRay(new Vector3(touchPosition.x, touchPosition.y, Camera.main.nearClipPlane));

            if (Physics.Raycast(ray, out var hit, RAYCAST_LENGTH, towerLayerMask))
            {
                if (hit.transform.TryGetComponent<IDraggable>(out var draggable))
                {
                    if (currentDraggable == draggable)
                    {
                        currentDraggable.OnDragEnd();
                        currentDraggable = null;
                    }

                }
            }
        }

        #endregion
    }
}
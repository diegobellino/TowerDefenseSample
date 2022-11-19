using UnityEngine;
using UnityEngine.UI;
using Utils.SmartUpdate;

namespace TowerDefense.Enemies
{
    public class EnemyHUD : MonoBehaviour, ISmartUpdate
    {
        #region VARIABLES

        private const float HEALTH_INDICATOR_SHOW_TIME = 1.5f;

        [SerializeField] private GameObject healthIndicator;
        [SerializeField] private Image healthFillImage;

        private bool showHealthIndicator;
        private float elapsedTime;

        public UpdateGroup Group => UpdateGroup.EvenFrames;

        #endregion

        #region LIFETIME

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
            if (showHealthIndicator)
            {
                // Adjust rotation to face camera
                healthIndicator.transform.rotation = Quaternion.Euler(Vector3.down * Camera.main.transform.rotation.eulerAngles.y);
                
                elapsedTime += deltaTime;

                if (elapsedTime >= HEALTH_INDICATOR_SHOW_TIME)
                {
                    healthIndicator.SetActive(false);
                    elapsedTime = 0f;
                    showHealthIndicator = false;
                }
            }
        }

        #endregion
    
        public void UpdateHealthBar(float percentage)
        {
            showHealthIndicator = true;
            elapsedTime = 0f;
            healthIndicator.SetActive(true);

            healthFillImage.fillAmount = percentage;
        }
    }
}

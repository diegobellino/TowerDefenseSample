using UnityEngine;
using UnityEngine.UI;
using Consts = TowerDefense.Constants.Constants;

namespace TowerDefense.Towers
{
    public class TowerHUD : MonoBehaviour
    {
        #region VARIABLES

        [Header("Placement")]
        [SerializeField] private GameObject placementIndicator;
        [SerializeField] private Image placementIndicatorImage;
        [SerializeField] private Color invalidPlacementColor;
        [SerializeField] private Color validPlacementColor;

        [Header("Range")]
        [SerializeField] private GameObject rangeIndicator;

        #endregion

        public void ShowPlacementIndicator(bool show)
        {
            if (show)
            {
            }

            placementIndicator.SetActive(show);
        }

        public void SetTowerSize(Vector2 size)
        {
            placementIndicator.transform.localScale = Vector3.one * (size / 2f);
        }

        public void SetPlacementIndicatorColor(bool valid)
        {
            placementIndicatorImage.color = valid ? validPlacementColor : invalidPlacementColor;
        }

        public void AdjustIndicatorsToState(TowerState state)
        {
            if (state == TowerState.Positioning)
            {
                placementIndicator.transform.localPosition = Vector3.down * (Consts.TOWER_GAMEPLAY_HEIGHT + Consts.TOWER_POSITIONING_HEIGHT - .1f);
                rangeIndicator.transform.localPosition = Vector3.down * (Consts.TOWER_GAMEPLAY_HEIGHT + Consts.TOWER_POSITIONING_HEIGHT - .15f);
                return;
            }

            if (state == TowerState.Gameplay)
            {
                placementIndicator.transform.localPosition = Vector3.down * (Consts.TOWER_GAMEPLAY_HEIGHT - .05f);
                rangeIndicator.transform.localPosition = Vector3.down * (Consts.TOWER_GAMEPLAY_HEIGHT - .01f);
                return;
            }
        }

        public void ShowRangeIndicator(bool show)
        {
            rangeIndicator.SetActive(show);
        }

        public void SetTowerRange(float range)
        {
            rangeIndicator.transform.localScale = range * 2 * Vector3.one;
        }
    }
}
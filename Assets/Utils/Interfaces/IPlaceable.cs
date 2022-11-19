using UnityEngine;

namespace Utils.Interfaces
{
    public interface IPlaceable : IPoolable, ISelectable
    {
        bool InPlacing { set; }


        bool CanLevelUp { get; }

        void RegisterController(IPlaceableController controller);

        Bounds GetBounds();

        void LevelUp();

    }
}

namespace Utils.Interfaces
{
    public interface IPlaceableController
    {
        void PlaceableSpawned(IPlaceable placeable);

        bool IsPlacementValid(IPlaceable placeable);
    }
}
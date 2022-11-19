using UnityEngine;

namespace Utils.Interfaces
{
    public interface IDraggable
    {
        bool CanDrag();

        void Drag(Vector2 offset);

        void OnDragStart();

        void OnDragEnd();
    }
}
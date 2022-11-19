namespace Utils.Interfaces
{
    public interface ISelectable
    {
        bool CanSelect();

        void Select();

        void Unselect();
    }
}
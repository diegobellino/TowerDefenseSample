namespace Utils.SmartUpdate
{
    /// <summary>
    /// SmartUpdate is a wrapper for FixedUpdate that's called by SmartUpdateController on the frame specified by UpdateGroup. 
    /// Needs to be registered to be called.
    /// </summary>
    public interface ISmartUpdate
    {
        UpdateGroup Group { get; }

        void SmartUpdate(float deltaTime);
    }
}
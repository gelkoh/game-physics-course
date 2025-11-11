namespace GameLibrary
{
    /// <summary>
    /// A GameObject consists of several Components, each implementing a different functionalities.
    /// </summary>
    public abstract class Component
    {
        public GameObject GameObject { get; private set; }
        public virtual void Connect(GameObject gameObject)
        {
            GameObject = gameObject;
        }

        public virtual void Destroy() 
        { }

        public Component CloneComponent()
        {
            return (Component)MemberwiseClone();
        }
    }
}

namespace Base.Actors
{
    public class SceneIDComponent : IActorComponent, ISerializedObject
    {
        SceneIDComponentData _sceneIDdata;

        public SceneIDComponent()
        {
        }

        public string ID => _sceneIDdata.ID;

        public ISerializedObjectData Data
        {
            get => _sceneIDdata;
            set { _sceneIDdata = value as SceneIDComponentData; }
        }

        public void DeInit()
        {
        }

        public void Init()
        {
        }
    }
}

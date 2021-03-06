using System.Linq;

namespace Base.Actors
{
    public class SceneIDComponentDef : ActorComponentDef
    {
        public override IActorComponent CreateActorComponent(Actor actor)
        {
            var comp = new SceneIDComponent();
            var sceneIDs = actor.ActorComponent.GetComponentInParent<SceneIDs>();
            if (sceneIDs != null)
            {
                var sceneID = sceneIDs.SceneObjectIDs.FirstOrDefault(x => x.GameObject == actor.ActorComponent.gameObject);
                comp.Data = new SceneIDComponentData { ID = sceneID.ID };
            }
            return comp;
        }
    }
}

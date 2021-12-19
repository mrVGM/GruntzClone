using Base.Status;

namespace Gruntz.Statuses
{
    public class SceneIDStatusDef : StatusDef
    {
        protected override StatusData StatusData
        {
            get
            {
                var sceneIDStatusData = new SceneIDStatusData();
                return sceneIDStatusData;
            }
        }
    }
}

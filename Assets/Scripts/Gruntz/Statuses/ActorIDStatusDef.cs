using Base.Status;
using System.Linq;
using UnityEditor;

namespace Gruntz.Statuses
{
    public class ActorIDStatusDef : StatusDef
    {
        protected override StatusData StatusData
        {
            get
            {
                var actorIDStatusData = new ActorIDStatusData();
                return actorIDStatusData;
            }
        }


        [MenuItem("asd/asd")]
        public static void asd()
        {
            var idStatus = Utils.EditorUtils.GetAllAssets<ActorIDStatusDef>().FirstOrDefault();
            var statusComponents = Utils.EditorUtils.GetAllAssets<StatusComponentDef>();

            foreach (var statusComponent in statusComponents) {
                if (statusComponent.InitialStatuses.Contains(idStatus)) {
                    continue;
                }

                var statuses = Enumerable.Repeat(idStatus, 1).Concat(statusComponent.InitialStatuses);
                statusComponent.InitialStatuses = statuses.ToArray();
                EditorUtility.SetDirty(statusComponent);
            }

            AssetDatabase.SaveAssets();
        }
    }
}

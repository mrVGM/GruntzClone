using Base.Actors;
using Gruntz.Actors;
using Base.Status;
using System.Collections.Generic;
using System.Linq;
using Base.UI;

namespace Gruntz.UI.ActorControl
{
    public class FilterSelectedUnits : CoroutineProcess
    {
        public ProcessContextTagDef SelectedActorsTag;
        protected override IEnumerator<object> Crt()
        {
            while (true)
            {
                var selectedActors = context.GetItem(SelectedActorsTag) as IEnumerable<Actor>;
                if (selectedActors == null) {
                    yield return null;
                    continue;
                }
                selectedActors = selectedActors.Where(x => {
                    var statusComponent = x.GetComponent<StatusComponent>();
                    var healthStatus = statusComponent
                    .GetStatuses(x => x.StatusData is HealthStatusData).FirstOrDefault();
                    if (healthStatus == null) {
                        return false;
                    }
                    var healthStatusData = healthStatus.StatusData as HealthStatusData;
                    if (healthStatusData.Health <= 0) {
                        return false;
                    }

                    return true;
                });
                context.PutItem(SelectedActorsTag, selectedActors.ToList());
                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}

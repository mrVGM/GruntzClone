using Base;
using Gruntz.Actors;
using Gruntz.Status;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gruntz.UserInteraction
{
    public class UpdateHealthbars : CoroutineProcess
    {
        public RectTransform HealthbarsContainer;
        public Healthbar HealthbarTemplate;
        public Vector2 HealthbarOffset;
        protected override IEnumerator<object> Crt()
        {
            var game = Game.Instance;
            var actorManager = ActorManager.GetActorManagerFromContext();
            var actorsWithHealthbar = actorManager.Actors.Where(x => x.ActorComponent.GetComponent<StatusComponent>() != null);

            while (true)
            {
                int healthbarsUsed = 0;
                foreach (var actor in actorManager.Actors)
                {
                    var statusComponent = actor.GetComponent<StatusComponent>();
                    if (statusComponent == null)
                    {
                        continue;
                    }
                    var healthStatus = statusComponent.GetStatuses(x => x.Data is HealthStatusData).FirstOrDefault();
                    if (healthStatus == null)
                    {
                        continue;
                    }
                    ++healthbarsUsed;
                    if (HealthbarsContainer.childCount < healthbarsUsed)
                    {
                        Instantiate(HealthbarTemplate, HealthbarsContainer);
                    }
                    var healthbar = HealthbarsContainer.GetChild(healthbarsUsed - 1).GetComponent<Healthbar>();

                    Vector3 screenPos = game.Camera.WorldToScreenPoint(actor.Pos) + (Vector3) HealthbarOffset;
                    healthbar.transform.position = screenPos;
                    healthbar.gameObject.SetActive(true);
                    var healthStatusData = healthStatus.Data as HealthStatusData;
                    healthbar.Bar.localScale = new Vector3(healthStatusData.Health / healthStatusData.MaxHealth, 1.0f, 1.0f);
                }

                for (int i = healthbarsUsed; i < HealthbarsContainer.childCount; ++i)
                {
                    var cur = HealthbarsContainer.GetChild(i);
                    cur.gameObject.SetActive(false);
                }

                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
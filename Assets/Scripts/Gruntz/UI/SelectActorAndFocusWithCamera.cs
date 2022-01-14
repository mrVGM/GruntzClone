using System.Collections.Generic;
using System.Linq;
using Base;
using Base.Actors;
using Base.UI;
using Cinemachine;
using Gruntz.Team;

namespace Gruntz.UI
{
    public class SelectActorAndFocusWithCamera : CoroutineProcess
    {
        public ProcessContextTagDef SelectedActorsTag;
        protected override IEnumerator<object> Crt()
        {
            var actorManager = ActorManager.GetActorManagerFromContext();
            var actors = actorManager.Actors.Where(x => {
                var teamComponent = x.GetComponent<TeamComponent>();
                if (teamComponent == null) {
                    return false;
                }

                return teamComponent.UnitTeam == TeamComponent.Team.Player;
            });

            var actorToSelect = actors.FirstOrDefault();
            if (actorToSelect == null) {
                yield break;
            }

            var selected = new List<Actor>();
            selected.Add(actorToSelect);
            context.PutItem(SelectedActorsTag, selected);
            
            var game = Game.Instance;
            var cam = game.Camera;
            var brain = cam.GetComponent<CinemachineBrain>();
            CinemachineVirtualCamera vcam = null;
            while (vcam == null) {
                vcam = brain.ActiveVirtualCamera as CinemachineVirtualCamera;
                yield return null;
            }
            vcam.LookAt.transform.position = actorToSelect.Pos;
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}

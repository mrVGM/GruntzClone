using Base;
using System.Collections.Generic;
using UnityEngine;
using Base.UI;
using Cinemachine;

namespace Gruntz.UI
{
    public class UpdateCamera : CoroutineProcess
    {
        public BoxCollider MapBounds;
        public Transform CameraLookAt;
        public float MovementSpeed = 0.02f;
        protected override IEnumerator<object> Crt()
        {
            var game = Game.Instance;
            var cam = game.Camera;
            var brain = cam.GetComponent<CinemachineBrain>();
            Collider bounds = null;

            CinemachineVirtualCamera vcam = null;
            while (true) {
                vcam = brain.ActiveVirtualCamera as CinemachineVirtualCamera;
                if (vcam != null) {
                    var lookAt = vcam.LookAt;
                    if (lookAt != null) {
                        bounds = lookAt.GetComponentInParent<Collider>();
                    }
                }
                if (bounds != null) {
                    break;
                }
                yield return null;
            }

            while (true) {
                var screenPos = Input.mousePosition;
                Vector3 dir = Vector3.zero;
                if (screenPos.x < 1) {
                    dir += Vector3.left;
                }
                if (screenPos.x > Screen.width - 2) {
                    dir += Vector3.right;
                }
                if (screenPos.y < 1) {
                    dir += Vector3.back;
                }
                if (screenPos.y > Screen.height - 2) {
                    dir += Vector3.forward;
                }

                dir.Normalize();
                vcam.LookAt.position += Time.deltaTime * MovementSpeed * dir;
                vcam.LookAt.position = bounds.bounds.ClosestPoint(vcam.LookAt.position);
                
                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}

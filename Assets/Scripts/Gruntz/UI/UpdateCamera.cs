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
                var viewportPosition = cam.ScreenToViewportPoint(Input.mousePosition);
                Vector3 dir = Vector3.zero;
                if (viewportPosition.x < 0) {
                    dir += Vector3.left;
                }
                if (viewportPosition.x > 1) {
                    dir += Vector3.right;
                }
                if (viewportPosition.y < 0) {
                    dir += Vector3.back;
                }
                if (viewportPosition.y > 1) {
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

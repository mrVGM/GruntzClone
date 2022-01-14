using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Gruntz.UI
{
    public class Notification : MonoBehaviour
    {
        private enum State
        {
            Hidden,
            Shown,
            VideoShown
        }

        public GameObject MenuButton;
        public Text Text;
        public VideoPlayer VideoPlayer;
        public GameObject Throbber;

        public Animator Animator;
        public Button WatchVideoButton;
        

        private State _state = State.Hidden;

        IEnumerator PlayVideo(string videoName)
        {
            VideoPlayer.url = Path.Combine(Application.streamingAssetsPath, videoName);
            VideoPlayer.Prepare();
            Throbber.SetActive(true);
            while (!VideoPlayer.isPrepared) {
                yield return null;
            }
            Throbber.SetActive(false);
            VideoPlayer.isLooping = true;
            VideoPlayer.Play();
        }

        public void Show(string text, string videoName)
        {
            MenuButton.SetActive(false);
            StopAllCoroutines();
            Text.text = text;

            WatchVideoButton.gameObject.SetActive(false);
            if (!string.IsNullOrEmpty(videoName)) {
                WatchVideoButton.gameObject.SetActive(true);
                StartCoroutine(PlayVideo(videoName));
            }

            _state = State.Shown;
            Animator.SetInteger("Shown", 1);
        }

        public void Hide()
        {
            MenuButton.SetActive(true);
            StopAllCoroutines();
            VideoPlayer.Stop();
            VideoPlayer.clip = null;

            _state = State.Hidden;
            Animator.SetInteger("Shown", 0);
        }

        public void ShowVideo()
        {
            if (_state != State.Shown) {
                return;
            }

            _state = State.VideoShown;
            Animator.SetInteger("Shown", 2);
        }

        public void HideVideo()
        {
            if (_state != State.VideoShown) {
                return;
            }

            _state = State.Shown;
            Animator.SetInteger("Shown", 1);
        }

        public void DismissNotification()
        {
            if (_state != State.Shown) {
                return;
            }
            Hide();
        }
    }
}

using System.Collections;
using System.IO;
using Base;
using Gruntz.TriggerBox;
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

        public TagDef LevelProgressTagDef;
        public GameObject MenuButton;
        public Text Text;
        public VideoPlayer VideoPlayer;
        public GameObject Throbber;

        public Animator Animator;
        public Button WatchVideoButton;
        public Button StartLevelButton;

        public RectTransform ImagesContainer;

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

        public void Show(NotificationDataBehaviour.Notification notification)
        {
            MenuButton.SetActive(false);
            StopAllCoroutines();
            Text.text = notification.NotificationText;

            WatchVideoButton.gameObject.SetActive(false);
            if (!string.IsNullOrEmpty(notification.VideoName)) {
                WatchVideoButton.gameObject.SetActive(true);
                StartCoroutine(PlayVideo(notification.VideoName));
            }
            
            StartLevelButton.gameObject.SetActive(false);
            StartLevelButton.onClick.RemoveAllListeners();
            if (notification.LevelToStart != null) {
                StartLevelButton.onClick.AddListener(() => {
                    var game = Game.Instance;
                    game.SavesManager.CreateSave(LevelProgressTagDef);
                    game.LoadLevel(notification.LevelToStart, () => { });
                });
                StartLevelButton.gameObject.SetActive(true);
            }

            ImagesContainer.gameObject.SetActive(false);
            for (int i = 0; i < ImagesContainer.childCount; ++i) {
                var child = ImagesContainer.GetChild(i);
                child.gameObject.SetActive(false);
            }
            int index = 0;
            foreach (var img in notification.ImagesToDisplay) {
                var notificationImage = ImagesContainer.GetChild(index).GetComponent<NotificationImage>();
                notificationImage.Image.sprite = img;
                notificationImage.gameObject.SetActive(true);
                ImagesContainer.gameObject.SetActive(true);
                ++index;
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

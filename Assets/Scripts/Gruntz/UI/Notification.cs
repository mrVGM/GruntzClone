using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Base;
using Gruntz.TriggerBox;
using LevelResults;
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
        public Button MenuButton;
        public Text Title;
        public Text Text;
        public VideoPlayer VideoPlayer;
        public GameObject Throbber;

        public Animator Animator;
        public Button WatchVideoButton;
        public Button StartLevelButton;
        public Button TryLevelAgainButton;
        public Button RetryButton;
        public Button GiveUpButton;

        public Button Dismiss;
        public Button Previous;
        public Button Next;
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

        public void Show(IEnumerable<NotificationDataBehaviour.Notification> notifications)
        {
            Show(notifications.ToList(), 0);
        }

        private void Show(List<NotificationDataBehaviour.Notification> notifications, int current)
        {
            var notification = notifications[current];

            MenuButton.enabled = false;
            StopAllCoroutines();
            Title.text = notification.Title;
            Text.text = notification.NotificationText;

            WatchVideoButton.gameObject.SetActive(false);
            if (!string.IsNullOrEmpty(notification.VideoName)) {
                WatchVideoButton.gameObject.SetActive(true);
                StartCoroutine(PlayVideo(notification.VideoName));
            }
            
            StartLevelButton.gameObject.SetActive(false);
            TryLevelAgainButton.gameObject.SetActive(false);
            
            RetryButton.gameObject.SetActive(false);
            GiveUpButton.gameObject.SetActive(false);
            
            Dismiss.gameObject.SetActive(false);
            
            StartLevelButton.onClick.RemoveAllListeners();
            TryLevelAgainButton.onClick.RemoveAllListeners();
            if (notification.LevelToStart != null)
            {
                var button = StartLevelButton;
                var levelProgress = LevelProgressInfo.GetLevelProgressInfoFromContext();
                var levelProgressInfoData = levelProgress.Data as LevelProgressInfoData;
                if (levelProgressInfoData.FinishedLevels.Select(x => (LevelDef)x).Contains(notification.LevelToStart)) {
                    button = TryLevelAgainButton;
                }

                button.onClick.AddListener(() => {
                    var game = Game.Instance;
                    game.SavesManager.CreateSave(LevelProgressTagDef);
                    game.LoadLevel(notification.LevelToStart, () => { });
                });
                button.gameObject.SetActive(true);
            }
            
            if (notification.RetryNotification) {
                Dismiss.gameObject.SetActive(false);
                RetryButton.gameObject.SetActive(true);
                GiveUpButton.gameObject.SetActive(true);
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
            
            Previous.gameObject.SetActive(false);
            Next.gameObject.SetActive(false);
            
            Previous.onClick.RemoveAllListeners();
            Next.onClick.RemoveAllListeners();

            if (!notification.RetryNotification && current == notifications.Count - 1) {
                Dismiss.gameObject.SetActive(true);
            }
            if (current < notifications.Count - 1) {
                Next.onClick.AddListener(() => Show(notifications, current + 1));
                Next.gameObject.SetActive(true);
            }

            if (current > 0) {
                Previous.onClick.AddListener(() => Show(notifications, current - 1));
                Previous.gameObject.SetActive(true);
            }

            _state = State.Shown;
            Animator.SetInteger("Shown", 1);
        }

        public void Hide()
        {
            MenuButton.enabled = true;
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

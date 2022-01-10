using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Gruntz.UI
{
    public class Notification : MonoBehaviour
    {
        public Text Text;
        public VideoPlayer VideoPlayer;
        public GameObject VideoScreen;

        public Button Prev;
        public Button Next;
        public Transform KnobsContainer;
        public Button Knob;
        public Transform PagesContainer;
        public GameObject Throbber;

        public void DoPaging()
        {
            IEnumerable<Transform> activePages()
            {
                for (int i = 0; i < PagesContainer.childCount; ++i) {
                    var cur = PagesContainer.GetChild(i);
                    if (cur.gameObject.activeSelf) {
                        yield return cur;
                    }
                }
            }

            Prev.gameObject.SetActive(false);
            Next.gameObject.SetActive(false);

            Prev.onClick.RemoveAllListeners();
            Next.onClick.RemoveAllListeners();

            for (int i = 0; i < KnobsContainer.childCount; ++i) {
                var cur = KnobsContainer.GetChild(i).GetComponent<Button>();
                cur.onClick.RemoveAllListeners();
                cur.gameObject.SetActive(false);
            }

            int index = 0;
            Button getOrCreateKnob()
            {
                while (index >= KnobsContainer.childCount) {
                    Instantiate(Knob, KnobsContainer);
                }

                var button = KnobsContainer.GetChild(index++).GetComponent<Button>();
                button.gameObject.SetActive(true);
                return button;
            }

            var pages = activePages();

            if (pages.Count() <= 1) {
                return;
            }

            Button selected = null;
            var allKnobs = new List<Button>();

            foreach (var page in pages) {
                var button = getOrCreateKnob();
                allKnobs.Add(button);
                if (selected == null) {
                    selected = button;
                }
                button.onClick.AddListener(() => {
                    foreach (var p in pages) {
                        p.gameObject.SetActive(false);
                    }
                    page.gameObject.gameObject.SetActive(true);
                    button.Select();
                });
            }

            Next.onClick.AddListener(() => {
                int selectedIndex = allKnobs.IndexOf(selected);
                ++selectedIndex;
                if  (selectedIndex >= allKnobs.Count) {
                    selected.Select();
                    return;
                }
                selected = allKnobs[selectedIndex];
                selected.onClick.Invoke();
            });

            Prev.onClick.AddListener(() => {
                int selectedIndex = allKnobs.IndexOf(selected);
                --selectedIndex;
                if (selectedIndex < 0) {
                    selected.Select();
                    return;
                }
                selected = allKnobs[selectedIndex];
                selected.onClick.Invoke();
            });

            Prev.gameObject.SetActive(true);
            Next.gameObject.SetActive(true);
            selected.onClick.Invoke();
        }
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
            StopAllCoroutines();
            Text.text = text;
            VideoScreen.SetActive(false);
            gameObject.SetActive(true);

            if (!string.IsNullOrEmpty(videoName)) {
                VideoScreen.SetActive(true);
                StartCoroutine(PlayVideo(videoName));
            }
            DoPaging();
        }

        public void Hide()
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
            VideoPlayer.Stop();
            VideoPlayer.clip = null;
        }
    }
}

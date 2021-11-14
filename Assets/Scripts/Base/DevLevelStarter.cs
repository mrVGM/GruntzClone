using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Base
{
    public class DevLevelStarter : MonoBehaviour
    {
        public LevelDef LevelDef;

        void Awake()
        {
            if (Game.Instance != null) {
                return;
            }
            DontDestroyOnLoad(gameObject);

            IEnumerator<object> loadCrt() {
                while (Game.Instance == null) {
                    yield return null;
                }
                while (Game.Instance.LoadingCurtain.gameObject.activeSelf) {
                    yield return null;
                }

                Game.Instance.LoadLevel(LevelDef, () => { });
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
            StartCoroutine(loadCrt());

            SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        }
    }
}

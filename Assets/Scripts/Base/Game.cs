using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Base
{
    public class Game : MonoBehaviour
    {
        public static Game Instance { get; private set; } = null;
        public DefRepositoryDef DefRepositoryDef;
        public GameObject LoadingCurtain;
        public MainUpdater MainUpdater => GetComponent<MainUpdater>();
        public Context Context => GetComponent<Context>();

        private LevelDef currentLevel;

        void Awake()
        {
            Instance = this;
            var initialLevel = DefRepositoryDef.AllDefs.OfType<InitialLevelDef>().FirstOrDefault();
            LoadLevel(initialLevel);
        }

        private void LoadLevelScenes(LevelDef levelDef, Action<IEnumerable<Scene>> levelLoaded)
        {
            int index = 0;
            List<Scene> loadedScenes = new List<Scene>();
            void loadSingleScene() 
            {
                if (index == levelDef.SceneIds.Length) 
                {
                    currentLevel = levelDef;
                    levelLoaded(loadedScenes);
                    return;
                }

                void sceneLoaded(Scene scene, LoadSceneMode mode)
                {
                    ++index;
                    loadedScenes.Add(scene);
                    loadSingleScene();
                    SceneManager.sceneLoaded -= sceneLoaded;
                }
                SceneManager.sceneLoaded += sceneLoaded;
                SceneManager.LoadSceneAsync(levelDef.SceneIds[index], LoadSceneMode.Additive);
            }

            loadSingleScene();
        }

        private void UnloadLevelScenes(Action levelUnloaded)
        {
            if (currentLevel == null)
            {
                levelUnloaded();
                return;
            }

            int index = currentLevel.SceneIds.Length - 1;
            void unloadSingleScene()
            {
                if (index < 0)
                {
                    currentLevel = null;
                    levelUnloaded();
                    return;
                }

                void sceneUnloaded(Scene scene)
                {
                    --index;
                    unloadSingleScene();
                    SceneManager.sceneUnloaded -= sceneUnloaded;
                }
                SceneManager.sceneUnloaded += sceneUnloaded;
                SceneManager.UnloadSceneAsync(currentLevel.SceneIds[index]);
            }

            unloadSingleScene();
        }

        public void LoadLevel(LevelDef levelDef)
        {
            LoadingCurtain.SetActive(true);
            UnloadLevelScenes(() => {

                IEnumerator<object> loadCrt()
                {
                    yield return new WaitForSeconds(0);
                    LoadLevelScenes(levelDef, (loadedScenes) =>
                    {
                        LoadingCurtain.SetActive(false);
                        Context.ClearContext();
                        foreach (var scene in loadedScenes)
                        {
                            var rootObjects = scene.GetRootGameObjects();
                            var levelInit = rootObjects.Select(x => x.GetComponent<LevelInit>()).FirstOrDefault(x => x != null);
                            if (levelInit != null)
                            {
                                levelInit.InitLevel();
                            }
                        }
                    });
                }

                StartCoroutine(loadCrt());
            });
        }
    }
}

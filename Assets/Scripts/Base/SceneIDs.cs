using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Base
{
    public class SceneIDs : MonoBehaviour
    {
        const string NeedsSceneIDTag = "NeedsSceneID"; 
        [Serializable]
        public class SceneObjectID
        {
            public GameObject GameObject;
            public string ID;
        }
        public List<SceneObjectID> SceneObjectIDs = new List<SceneObjectID>();

        public void Refresh(Scene scene)
        {
            IEnumerable<GameObject> allGOs(IEnumerable<GameObject> roots)
            {
                var gos = new Queue<GameObject>();
                foreach (var root in roots) {
                    gos.Enqueue(root);
                }

                while (gos.Count > 0)
                {
                    var cur = gos.Dequeue();
                    yield return cur;
                    for (int i = 0; i < cur.transform.childCount; ++i)
                    {
                        gos.Enqueue(cur.transform.GetChild(i).gameObject);
                    }
                }
            }
            var objectsWithID = allGOs(scene.GetRootGameObjects()).Where(x => x.CompareTag(NeedsSceneIDTag)).ToList();
            SceneObjectIDs.RemoveAll(x => !objectsWithID.Contains(x.GameObject));
            foreach (var go in objectsWithID)
            {
                if (SceneObjectIDs.Any(x => x.GameObject == go))
                {
                    continue;
                }

                SceneObjectIDs.Add(new SceneObjectID { GameObject = go, ID = Guid.NewGuid().ToString() });
            }
        }
    }
}

using System.Linq;
using UnityEngine;

namespace Hegametech.Framework
{
    public class MonoSingleton : MonoBehaviour
    {
        public static bool IsApplicationQuit { get; set; } = false;

        public static K CreateMonoSingleton<K>() where K : MonoBehaviour, ISingleton
        {
            if (IsApplicationQuit)
            {
                return null;
            }

            K instance = null;

            if (instance == null && !IsApplicationQuit)
            {
                instance = GameObject.FindObjectOfType(typeof(K)) as K;
                if (instance == null)
                {
                    System.Reflection.MemberInfo info = typeof(K);
                    object[] attributes = info.GetCustomAttributes(true);
                    for (int i = 0; i < attributes.Length; ++i)
                    {
                        MonoSingletonAttribute defineAttri = attributes[i] as MonoSingletonAttribute;
                        if (defineAttri == null)
                        {
                            continue;
                        }
                        instance = CreateComponentOnGameObject<K>(defineAttri.AbsolutePath, true);
                        break;
                    }

                    if (instance == null)
                    {
                        GameObject obj = new GameObject("Singleton of " + typeof(K).Name);
                        UnityEngine.Object.DontDestroyOnLoad(obj);
                        instance = obj.AddComponent<K>();
                    }
                }
                else
                {
                    System.Reflection.MemberInfo info = typeof(K);
                    object[] attributes = info.GetCustomAttributes(true);
                    for (int i = 0; i < attributes.Length; ++i)
                    {
                        MonoSingletonAttribute defineAttri = attributes[i] as MonoSingletonAttribute;
                        if (defineAttri == null)
                        {
                            continue;
                        }
                        instance.transform.SetParent(MoveGameObjectToPath(defineAttri.AbsolutePath, true).transform);
                        instance.gameObject.name = defineAttri.AbsolutePath.Split('/').Last();
                        break;
                    }
                }

                instance.OnSingletonInit();
            }

            return instance;
        }

        protected static K CreateComponentOnGameObject<K>(string path, bool dontDestroy) where K : MonoBehaviour
        {
            GameObject obj = FindGameObject(null, path, true, dontDestroy);
            if (obj == null)
            {
                obj = new GameObject("Singleton of " + typeof(K).Name);
                if (dontDestroy)
                {
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                }
            }

            return obj.AddComponent<K>();
        }

        protected static GameObject MoveGameObjectToPath(string path, bool dontDestroy)
        {
            GameObject obj = FindGameObjectOfExist(null, path, true, dontDestroy);
            if (obj == null)
            {
                obj = new GameObject("Singleton");
                if (dontDestroy)
                {
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                }
            }

            return obj;
        }

        public static GameObject FindGameObject(GameObject root, string path, bool build, bool dontDestroy)
        {
            if (path == null || path.Length == 0)
            {
                return null;
            }

            string[] subPath = path.Split('/');
            if (subPath == null || subPath.Length == 0)
            {
                return null;
            }

            return FindGameObject(null, subPath, 0, build, dontDestroy);
        }

        public static GameObject FindGameObjectOfExist(GameObject root, string path, bool build, bool dontDestroy)
        {
            if (path == null || path.Length == 0)
            {
                return null;
            }

            string[] subPath = path.Split('/');
            if (subPath == null || subPath.Length == 0)
            {
                return null;
            }
            string[] newSubPath = new string[subPath.Length - 1];
            for(int i=0;i< newSubPath.Length; i++)
            {
                newSubPath[i] = subPath[i];
            }

            return FindGameObject(null, newSubPath, 0, build, dontDestroy);
        }

        public static GameObject FindGameObject(GameObject root, string[] subPath, int index, bool build, bool dontDestroy)
        {
            GameObject client = null;

            if (root == null)
            {
                client = GameObject.Find(subPath[index]);
            }
            else
            {
                var child = root.transform.Find(subPath[index]);
                if (child != null)
                {
                    client = child.gameObject;
                }
            }

            if (client == null)
            {
                if (build)
                {
                    client = new GameObject(subPath[index]);
                    if (root != null)
                    {
                        client.transform.SetParent(root.transform);
                    }
                    if (dontDestroy && index == 0)
                    {
                        GameObject.DontDestroyOnLoad(client);
                    }
                }
            }

            if (client == null)
            {
                return null;
            }

            if (++index == subPath.Length)
            {
                return client;
            }

            return FindGameObject(client, subPath, index, build, dontDestroy);
        }
    }
}

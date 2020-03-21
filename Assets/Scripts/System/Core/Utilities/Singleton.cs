using UnityEngine;

namespace LowPolyHnS.Core
{
    /// <summary>
    ///     Singleton pattern.
    /// </summary>
    /// https://stackoverflow.com/questions/2667024/singleton-pattern-for-c-sharp
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private const string MSG_MULTI_INSTANCE = "[Singleton] Multiple instances of a singleton gameObject '{0}'";
        private static T _instance;

        #region PROPERTIES

        private static bool ShowDebug => false;
        protected bool IsExiting;

        #endregion

        #region CONSTRUCTOR

        public static T Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = (T) FindObjectOfType(typeof(T));

                if (FindObjectsOfType(typeof(T)).Length > 1)
                {
                    DebugLogFormat(MSG_MULTI_INSTANCE, _instance.gameObject.name);

                    return _instance;
                }

                if (_instance == null)
                {
                    GameObject singleton = new GameObject();
                    _instance = singleton.AddComponent<T>();
                    singleton.name = $"{typeof(T)}(singleton)";

                    Singleton<T> component = _instance.GetComponent<Singleton<T>>();
                    component.OnCreate();

                    if (component.ShouldNotDestroyOnLoad())
                    {
                        DontDestroyOnLoad(singleton);
                    }

                    DebugLogFormat("[Singleton] Creating an instance of {0} with DontDestroyOnLoad", typeof(T));
                }
                else
                {
                    DebugLogFormat("[Singleton] Using instance already created '{0}'", _instance.gameObject.name);
                }

                return _instance;
            }
        }

        #endregion

        #region VIRTUAL METHODS

        protected virtual void OnCreate()
        {
        }

        protected void WakeUp()
        {
        }

        protected virtual bool ShouldNotDestroyOnLoad()
        {
            return true;
        }

        #endregion

        #region PRIVATE METHODS

        private void OnApplicationQuit()
        {
            IsExiting = true;
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        private static void DebugLogFormat(string content, params object[] parameters)
        {
            if (ShowDebug == false)
            {
                return;
            }

            Debug.LogFormat(content, parameters);
        }

        #endregion
    }
}
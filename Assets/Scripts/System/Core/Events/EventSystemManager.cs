using LowPolyHnS.Core.Hooks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("LowPolyHnS/Managers/EventSystemManager", 100)]
    public class EventSystemManager : Singleton<EventSystemManager>
    {
        protected override void OnCreate()
        {
            EventSystem eventSystem = FindObjectOfType<EventSystem>();
            StandaloneInputModule input = FindObjectOfType<StandaloneInputModule>();

            if (input != null)
            {
                Destroy(input);
            }

            if (eventSystem != null)
            {
                Destroy(eventSystem);
            }

            gameObject.AddComponent<EventSystem>();
            inputModule = gameObject.AddComponent<GameInputModule>();

            SceneManager.sceneLoaded += OnSceneLoad;
        }

        public void Wakeup()
        {
        }


        private GameInputModule inputModule;

        public GameObject GetPointerGameObject(int pointerID = -1)
        {
            return inputModule.GameObjectUnderPointer(pointerID);
        }

        public bool IsPointerOverUI(int pointerID = -1)
        {
            GameObject pointer = GetPointerGameObject(pointerID);
            if (pointer == null) return false;

            return pointer.transform as RectTransform != null;
        }

        public void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
        {
            RequireInit();
        }

        private void RequireInit()
        {
            if (HookCamera.Instance == null)
            {
                RequireCamera();
            }

            if (HookCamera.Instance == null)
            {
                return;
            }

            PhysicsRaycaster raycaster3D = HookCamera.Instance.Get<PhysicsRaycaster>();
            Physics2DRaycaster raycaster2D = HookCamera.Instance.Get<Physics2DRaycaster>();

            if (raycaster3D == null) HookCamera.Instance.gameObject.AddComponent<PhysicsRaycaster>();
            if (raycaster2D == null) HookCamera.Instance.gameObject.AddComponent<Physics2DRaycaster>();
        }

        private void RequireCamera()
        {
            GameObject cameraTag = GameObject.FindWithTag("MainCamera");
            if (cameraTag != null)
            {
                cameraTag.AddComponent<HookCamera>();
                return;
            }

            Camera cameraComp = FindObjectOfType<Camera>();
            if (cameraComp != null)
            {
                cameraComp.gameObject.AddComponent<HookCamera>();
            }
        }
    }
}
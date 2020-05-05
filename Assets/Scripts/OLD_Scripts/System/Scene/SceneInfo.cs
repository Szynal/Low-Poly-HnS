using UnityEditor;
using UnityEngine;

public enum ESceneType
{
    Development,
    Gameplay,
    Cutscene,
    Camera,
    Geometry,
    Menu,
    Enemies,
    Starter
}

public class SceneInfo : MonoBehaviour
{
    public ESceneType SceneType = ESceneType.Development;
    public string CameraSceneName = default;
    public bool HasEnemyScene = false;

#if UNITY_EDITOR
    [MenuItem("GameObject/LowPolyHnS/Scene Info", priority = 10)]
#endif
    public static void CreateObject()
    {
        GameObject gameObject = new GameObject("SceneInfo");
        gameObject.AddComponent<SceneInfo>();
        gameObject.tag = "SceneInfo";
    }
}
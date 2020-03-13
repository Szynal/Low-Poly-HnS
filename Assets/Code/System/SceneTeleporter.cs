using LowPolyHnS;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleporter : MonoBehaviour
{
    public string TargetSceneName = "";
    public LoadSceneMode LoadMode;
    public bool OverridesPlayerPosition = false;
    public Transform OverridedTransform;
    public bool OneWayTeleport = false;
    public bool OverrideInventory = true;
    public bool OverrideHealth = true;
    [SerializeField] private bool loadDistinctGameplayScene = false;
    [SerializeField] private string distinctGameplayToLoad = "";

    public void HandleTeleport()
    {
        if (OverridesPlayerPosition == false)
        {
            OverridedTransform = transform;
        }

        FE_PlayerLoadParams _params = new FE_PlayerLoadParams(OverridesPlayerPosition, OverridedTransform.position,
            OverridedTransform.rotation.eulerAngles, OverrideInventory, OverrideHealth);

        if (loadDistinctGameplayScene == false)
        {
            SceneLoader.Instance.LoadLevel(SceneManager.GetSceneByName(TargetSceneName).buildIndex, TargetSceneName,
                LoadMode, OneWayTeleport, _params);
        }
        else
        {
            SceneLoader.Instance.LoadLevel(SceneManager.GetSceneByName(TargetSceneName).buildIndex, TargetSceneName,
                LoadMode, OneWayTeleport, _params, distinctGameplayToLoad);
        }
    }
}
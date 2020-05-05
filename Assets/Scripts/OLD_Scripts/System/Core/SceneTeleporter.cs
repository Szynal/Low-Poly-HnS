using System;
using System.Threading.Tasks;
using LowPolyHnS;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleporter : MonoBehaviour
{
    public string TargetSceneName = "";
    public LoadSceneMode LoadMode;
    public bool OverridesPlayerPosition = false;
    public Transform OverriddenTransform;
    public bool OneWayTeleport = false;
    public bool OverrideInventory = true;
    public bool OverrideHealth = true;
    [SerializeField] private bool loadDistinctGameplayScene = false;
    [SerializeField] private string distinctGameplayToLoad = "";

    /// <summary>
    ///     TODO RUN ON ENABLE
    /// </summary>
    public bool TeleportOnEnable = false;

    public float DelayBeforeActivating = 0f;


    private void OnEnable()
    {
        if (TeleportOnEnable)
        {
            DelayTeleport();
        }
    }

    private async void DelayTeleport()
    {
        if (DelayBeforeActivating > 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(DelayBeforeActivating));
        }

        HandleTeleport();
    }

    public void HandleTeleport()
    {
        if (OverridesPlayerPosition == false)
        {
            OverriddenTransform = transform;
        }

        PlayerLoadParams loadParams = new PlayerLoadParams(OverridesPlayerPosition, OverriddenTransform.position,
            OverriddenTransform.rotation.eulerAngles, OverrideInventory, OverrideHealth);

        if (loadDistinctGameplayScene == false)
        {
            SceneLoader.Instance.LoadLevel(SceneManager.GetSceneByName(TargetSceneName).buildIndex, TargetSceneName,
                LoadMode, OneWayTeleport, loadParams);
        }
        else
        {
            SceneLoader.Instance.LoadLevel(SceneManager.GetSceneByName(TargetSceneName).buildIndex, TargetSceneName,
                LoadMode, OneWayTeleport, loadParams, distinctGameplayToLoad);
        }
    }
}
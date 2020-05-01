using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using LowPolyHnS;
using UnityEngine;
using UnityEngine.Playables;

public enum ECameraTransitionType
{
    SharpCut,
    FadeToBlack,
    SmoothMove,
    None
}

/// <summary>
///     This script is meant to be used on an object with timeline director component
/// </summary>
public class CutsceneController : MonoBehaviour
{
    public delegate void OnSelfCountStarted(float countdownTime);

    public OnSelfCountStarted CountdownDelegate;

    [Header("General properties")] public string SceneName = "";
    public Camera CutsceneCam = null;
    public Camera LerpCam = null;
    [SerializeField] protected bool oneShotOnly = true;
    [Header("Skipping")] [SerializeField] protected GameObject skipCanvas = null;
    [Header("Transitions")] public ECameraTransitionType StartTransitionType;
    public float StartTransitionTime = 1f;
    public ECameraTransitionType EndTransitionType;
    public float EndTransitionTime = 1f;

    [Header("Things related to having, or not, the director and a scene")] [SerializeField]
    protected bool hasItsOwnScene = false;

    [SerializeField] protected bool hasDirector = true;
    public float TimeToPlayWithoutDirector = 0f;
    [SerializeField] protected GameObject[] objectsToActivateWithoutDirector = null;

    [Header("Properties related to teleporting to another scene")]
    public bool TeleportsToDifferentScene;

    [Header("After cutscene changes")] [SerializeField]
    private List<FE_ActionContainer> afterCutsceneChanges = null;

    protected PlayableDirector director;
    protected Coroutine transitionCoroutine;
    protected SceneTeleporter sceneTeleporter;
    protected Camera targetCamera;
    protected bool canSkip;
    protected ScreenFadeManager fadeManager;

    private void Awake()
    {
        if (EndTransitionType == ECameraTransitionType.SharpCut)
        {
            EndTransitionTime = 0f;
        }

        if (hasDirector)
        {
            director = GetComponent<PlayableDirector>();
            if (director != null)
            {
                director.stopped += onCutsceneStopped;
            }
            else
            {
                Debug.LogError("CutsceneController on object " + name + " couldn't find PlayableDirector!");
            }
        }

        if (CutsceneCam == null)
        {
            Debug.LogError("CutsceneController " + name + " doesn't have camera attached!");
        }

        if (LerpCam == null && (StartTransitionType == ECameraTransitionType.SmoothMove ||
                                EndTransitionType == ECameraTransitionType.SmoothMove))
        {
            Debug.LogWarning("CutsceneController " + name +
                             " wants a smooth transition, but has no lerp cam attached. Changing to sharp cut transition...");
            StartTransitionType = ECameraTransitionType.SharpCut;
            EndTransitionType = ECameraTransitionType.SharpCut;
        }

        if (TeleportsToDifferentScene)
        {
            sceneTeleporter = GetComponent<SceneTeleporter>();
            if (sceneTeleporter == null)
            {
                Debug.LogError("Cutscene controller " + name +
                               " has 'teleportsToDifferentScene' set as true, but has no SceneTeleporter component!");
                TeleportsToDifferentScene = false;
            }
        }
    }

    private void Start()
    {
        targetCamera = Camera.main;
        fadeManager = GameManager.Instance.ScreenFadeScript;
    }

    private void Update()
    {
        if (skipCanvas.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.F) && canSkip)
            {
                StopAllCoroutines();
                fadeManager.StopFading();
                if (hasDirector)
                {
                    director.Stop();
                }
                else
                {
                    onCutsceneStopped(null);
                }

                canSkip = false;
                skipCanvas.SetActive(false);
            }
        }
    }

    public void PlayCutscene()
    {
        if (GameManager.Instance.IsInCutscene)
        {
            GameManager.Instance.ScheduleCutscene(this);
            return;
        }

        GameManager.Instance.OnCutsceneStart();

        //Play cutscene based on our transition type
        switch (StartTransitionType)
        {
            case ECameraTransitionType.SharpCut:
                HandleSharpCut(targetCamera, CutsceneCam, true);
                break;
            case ECameraTransitionType.FadeToBlack:
                if (transitionCoroutine != null)
                {
                    StopCoroutine(transitionCoroutine);
                }

                fadeManager.FadeToBlack(true, StartTransitionTime / 2f,
                    () => handleFadeToBlack(targetCamera, CutsceneCam, StartTransitionTime, true));
                break;
            case ECameraTransitionType.SmoothMove:
                if (transitionCoroutine != null)
                {
                    StopCoroutine(transitionCoroutine);
                }

                transitionCoroutine =
                    StartCoroutine(handleSmoothMove(targetCamera, CutsceneCam, StartTransitionTime, true));
                break;
            default:
                Debug.LogError("CutsceneController " + name + " encountered an unknown StartTransitionType!");
                break;
        }
    }

    private void OnStartPlayback()
    {
        UIController.Instance.HandleCutscene(true);

        if (hasDirector)
        {
            director.Play();
        }
        else
        {
            if (StartTransitionType != ECameraTransitionType.FadeToBlack)
            {
                activateObjects();
            }

            StartCoroutine(handleSelfTiming());
        }

        skipCanvas.SetActive(true);
        StartCoroutine(WaitForSkippability(0.5f));
    }

    private IEnumerator WaitForSkippability(float time)
    {
        yield return new WaitForSecondsRealtime(time);

        canSkip = true;
    }

    private void HandleSharpCut(Camera current, Camera target, bool startPlaying)
    {
        current.enabled = false;
        target.enabled = true;

        if (startPlaying)
        {
            OnStartPlayback();
        }

        if (startPlaying == false)
        {
            endCutscene();
        }
    }

    private async void handleFadeToBlack(Camera current, Camera target, float transitionTime, bool startPlaying)
    {
        activateObjects();
        current.enabled = false;
        target.enabled = true;
        await Task.Delay(TimeSpan.FromSeconds(0.1f));

        if (startPlaying)
        {
            OnStartPlayback();
        }

        if (startPlaying == false)
        {
            endCutscene();
        }

        if (TeleportsToDifferentScene == false || startPlaying)
        {
            fadeManager.UnfadeFromBlack(0f, transitionTime / 2f);
        }
    }

    private IEnumerator handleSmoothMove(Camera current, Camera target, float transitionTime, bool startPlaying)
    {
        //First we put lerp cam at our current cam pos and copy its' FOV
        LerpCam.transform.position = current.transform.position;
        LerpCam.transform.rotation = current.transform.rotation;
        LerpCam.fieldOfView = current.fieldOfView;

        //Some variables to help with lerping
        Vector3 _basePos = current.transform.position;
        Quaternion _baseRot = current.transform.rotation;
        float _baseFOV = current.fieldOfView;

        //Then we disable old camera and enable lerpcam
        current.enabled = false;
        LerpCam.enabled = true;

        //We lerp lerp cam to target position and rotation and fov
        float _lerpRate = 1f / transitionTime;
        float _lerpTime = 0f;

        while (_lerpTime <= 1f)
        {
            _lerpTime += Time.unscaledDeltaTime * _lerpRate;

            LerpCam.transform.position = Vector3.Lerp(_basePos, target.transform.position, _lerpTime);
            LerpCam.transform.rotation = Quaternion.Lerp(_baseRot, target.transform.rotation, _lerpTime);
            LerpCam.fieldOfView = Mathf.Lerp(_baseFOV, target.fieldOfView, _lerpTime);

            yield return null;
        }

        //Disable lerp cam and enable target camera
        LerpCam.enabled = false;
        target.enabled = true;


        //Start playback
        if (startPlaying)
        {
            OnStartPlayback();
        }

        if (startPlaying == false)
        {
            endCutscene();
        }

        transitionCoroutine = null;
    }

    private IEnumerator handleSelfTiming()
    {
        CountdownDelegate?.Invoke(TimeToPlayWithoutDirector);

        yield return new WaitForSecondsRealtime(TimeToPlayWithoutDirector);

        onCutsceneStopped(null);
    }

    protected virtual void onCutsceneStopped(PlayableDirector _stoppedDirector)
    {
        switch (EndTransitionType)
        {
            case ECameraTransitionType.SharpCut:
                HandleSharpCut(CutsceneCam, targetCamera, false);
                break;

            case ECameraTransitionType.FadeToBlack:
                if (transitionCoroutine != null)
                {
                    StopCoroutine(transitionCoroutine);
                }

                fadeManager.FadeToBlack(true, EndTransitionTime / 2f,
                    () => handleFadeToBlack(CutsceneCam, targetCamera, EndTransitionTime, false));
                break;

            case ECameraTransitionType.SmoothMove:
                if (transitionCoroutine != null)
                {
                    StopCoroutine(transitionCoroutine);
                }

                transitionCoroutine =
                    StartCoroutine(handleSmoothMove(CutsceneCam, targetCamera, EndTransitionTime, false));
                break;

            case ECameraTransitionType.None:

                endCutscene();
                break;

            default:
                Debug.LogError("CutsceneController " + name + " encountered an unknown EndTransitionType!");
                break;
        }
    }

    protected virtual void endCutscene()
    {
        if (TeleportsToDifferentScene)
        {
            sceneTeleporter.HandleTeleport();
        }

        foreach (FE_ActionContainer _action in afterCutsceneChanges)
        {
            FE_StateChanger.HandleObjectChange(_action);
        }

        GameManager.Instance.OnCutsceneEnd(EndTransitionTime / 2f);

        if (hasItsOwnScene)
        {
            SceneLoader.Instance.CloseCutscene(SceneName);
        }
        else
        {
            if (oneShotOnly)
            {
                Destroy(gameObject);
            }
            else
            {
                skipCanvas.SetActive(false);
                foreach (GameObject _go in objectsToActivateWithoutDirector)
                {
                    _go.SetActive(false);
                }
            }
        }
    }

    private void activateObjects()
    {
        foreach (GameObject _go in objectsToActivateWithoutDirector)
        {
            _go.SetActive(true);
        }
    }
}
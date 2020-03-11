using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using LowPolyHnS;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public enum ECameraTransitionType
{
    SharpCut,
    FadeToBlack,
    SmoothMove,
    None
};

//This script is meant to be used on an object with timeline director component
public class FE_CutsceneController : MonoBehaviour
{
    public delegate void OnSelfCountStarted(float _countdownTime);
    public OnSelfCountStarted CountdownDelegate;

    [Header("General properties")]
    public string SceneName = "";
    public Camera CutsceneCam = null;
    public Camera LerpCam = null;
    [SerializeField] protected bool oneShotOnly = true;
    [Header("Skipping")]
    [SerializeField] protected GameObject skipCanvas = null;
    [Header("Transitions")]
    public ECameraTransitionType StartTransitionType = default;
    public float StartTransitionTime = 1f;
    public ECameraTransitionType EndTransitionType = default;
    public float EndTransitionTime = 1f;
    [Header("Things related to having, or not, the director and a scene")]
    [SerializeField] protected bool hasItsOwnScene = false;
    [SerializeField] protected bool hasDirector = true;
    public float TimeToPlayWithoutDirector = 0f;
    [SerializeField] protected GameObject[] objectsToActivateWithoutDirector = null;
    [Header("Properties related to teleporting to another scene")]
    public bool TeleportsToDifferentScene = false;
    [Header("After cutscene changes")]
    [SerializeField] List<FE_ActionContainer> afterCutsceneChanges = null;

    protected PlayableDirector director = null;
    protected Coroutine transitionCoroutine = null;
    protected FE_PlayerInventoryInteraction playerRef = null;
    protected FE_SceneTeleporter sceneTeleporter;
    protected Camera targetCamera;
    protected bool canSkip = false;
    protected ScreenFadeManager fadeManager = null;

    private void Awake()
    {
        if(EndTransitionType == ECameraTransitionType.SharpCut)
        {
            EndTransitionTime = 0f;
        }

        if (hasDirector == true)
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

        if(CutsceneCam == null)
        {
            Debug.LogError("CutsceneController " + name + " doesn't have camera attached!");
        }

        if(LerpCam == null && (StartTransitionType == ECameraTransitionType.SmoothMove || EndTransitionType == ECameraTransitionType.SmoothMove))
        {
            Debug.LogWarning("CutsceneController " + name + " wants a smooth transition, but has no lerp cam attached. Changing to sharp cut transition...");
            StartTransitionType = ECameraTransitionType.SharpCut;
            EndTransitionType = ECameraTransitionType.SharpCut;
        }

        if (TeleportsToDifferentScene == true)
        {
            sceneTeleporter = GetComponent<FE_SceneTeleporter>();
            if (sceneTeleporter == null)
            {
                Debug.LogError("Cutscene controller " + name + " has 'teleportsToDifferentScene' set as true, but has no SceneTeleporter component!");
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
        if (skipCanvas.activeSelf == true)
        {
            if(FE_CrossInput.UseItem() == true && canSkip == true)
            {
                StopAllCoroutines();
                fadeManager.StopFading();
                if (hasDirector == true)
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

    public void PlayCutscene(FE_PlayerInventoryInteraction _interactionController = null)
    {
        if(GameManager.Instance.IsInCutscene == true)
        {
            GameManager.Instance.ScheduleCutscene(this);
            return;
        }

        GameManager.Instance.OnCutsceneStart();

        if(_interactionController != null)
        {
            playerRef = _interactionController;
            playerRef.InputController.ChangeInputMode(EInputMode.None);
        }

        //Play cutscene based on our transition type
        switch (StartTransitionType)
        {
            case ECameraTransitionType.SharpCut:
                handleSharpCut(targetCamera, CutsceneCam, true);
                break;
            case ECameraTransitionType.FadeToBlack:
                if(transitionCoroutine != null)
                {
                    StopCoroutine(transitionCoroutine);
                }

                fadeManager.FadeToBlack(true, StartTransitionTime / 2f, () => handleFadeToBlack(targetCamera, CutsceneCam, StartTransitionTime, true));
                break;
            case ECameraTransitionType.SmoothMove:
                if (transitionCoroutine != null)
                {
                    StopCoroutine(transitionCoroutine);
                }

                transitionCoroutine = StartCoroutine(handleSmoothMove(targetCamera, CutsceneCam, StartTransitionTime, true));
                break;
            default:
                Debug.LogError("CutsceneController " + name + " encountered an unknown StartTransitionType!");
                break;
        }
    }

    private void onStartPlayback()
    {
        FE_UIController.Instance.HandleCutscene(true);

        if (hasDirector == true)
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
        StartCoroutine(waitForSkippability(0.5f));
    }

    private IEnumerator waitForSkippability(float _time)
    {
        yield return new WaitForSecondsRealtime(_time);

        canSkip = true;
    }

    private void handleSharpCut(Camera _current, Camera _target, bool _startPlaying)
    {
        _current.enabled = false;
        _target.enabled = true;

        if (_startPlaying == true)
        {
            onStartPlayback();
        }
        else if (playerRef != null)
        {
            playerRef.InputController.ChangeInputMode(EInputMode.Full);
            playerRef = null;
        }
        if (_startPlaying == false)
        {
            endCutscene();
        }
    }

    private async void handleFadeToBlack(Camera _current, Camera _target, float _transitionTime, bool _startPlaying)
    {
        activateObjects();
        _current.enabled = false;
        _target.enabled = true;
        await Task.Delay(TimeSpan.FromSeconds(0.1f));
        
        if (_startPlaying == true)
        {
            onStartPlayback();
        }
        else if (playerRef != null)
        {
            playerRef.InputController.ChangeInputMode(EInputMode.Full);
            playerRef = null;
        }
        if (_startPlaying == false)
        {
            endCutscene();
        }

        if (TeleportsToDifferentScene == false || _startPlaying == true)
        {
            fadeManager.UnfadeFromBlack(0f, _transitionTime / 2f);
        }
    }

    private IEnumerator handleSmoothMove(Camera _current, Camera _target, float _transitionTime, bool _startPlaying)
    {
        //First we put lerp cam at our current cam pos and copy its' FOV
        LerpCam.transform.position = _current.transform.position;
        LerpCam.transform.rotation = _current.transform.rotation;
        LerpCam.fieldOfView = _current.fieldOfView;

        //Some variables to help with lerping
        Vector3 _basePos = _current.transform.position;
        Quaternion _baseRot = _current.transform.rotation;
        float _baseFOV = _current.fieldOfView;

        //Then we disable old camera and enable lerpcam
        _current.enabled = false;
        LerpCam.enabled = true;

        //We lerp lerp cam to target position and rotation and fov
        float _lerpRate = 1f / _transitionTime;
        float _lerpTime = 0f;

        while(_lerpTime <= 1f)
        {
            _lerpTime += Time.unscaledDeltaTime * _lerpRate;

            LerpCam.transform.position = Vector3.Lerp(_basePos, _target.transform.position, _lerpTime);
            LerpCam.transform.rotation = Quaternion.Lerp(_baseRot, _target.transform.rotation, _lerpTime);
            LerpCam.fieldOfView = Mathf.Lerp(_baseFOV, _target.fieldOfView, _lerpTime);

            yield return null;
        }

        //Disable lerp cam and enable target camera
        LerpCam.enabled = false;
        _target.enabled = true;


        //Start playback
        if (_startPlaying == true)
        {
            onStartPlayback();
        }
        else if(playerRef != null)
        {
            playerRef.InputController.ChangeInputMode(EInputMode.Full);
            playerRef = null;
        }
        if (_startPlaying == false)
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
                handleSharpCut(CutsceneCam, targetCamera, false);
                break;

            case ECameraTransitionType.FadeToBlack:     
                if (transitionCoroutine != null)
                {
                    StopCoroutine(transitionCoroutine);
                }

                fadeManager.FadeToBlack(true, EndTransitionTime / 2f, () => handleFadeToBlack(CutsceneCam, targetCamera, EndTransitionTime, false));
                break;

            case ECameraTransitionType.SmoothMove:
                if (transitionCoroutine != null)
                {
                    StopCoroutine(transitionCoroutine);
                }

                transitionCoroutine = StartCoroutine(handleSmoothMove(CutsceneCam, targetCamera, EndTransitionTime, false));
                break;

            case ECameraTransitionType.None:
                if (playerRef != null)
                {
                    playerRef.InputController.ChangeInputMode(EInputMode.Full);
                    playerRef = null;
                }
                endCutscene();
                break;

            default:
                Debug.LogError("CutsceneController " + name + " encountered an unknown EndTransitionType!");
                break;
        }
    }

    protected virtual void endCutscene()
    {
        if (TeleportsToDifferentScene == true)
        {
            sceneTeleporter.HandleTeleport();
        }

        foreach (FE_ActionContainer _action in afterCutsceneChanges)
        {
            FE_StateChanger.HandleObjectChange(_action);
        }
        GameManager.Instance.OnCutsceneEnd(EndTransitionTime / 2f);

        if (hasItsOwnScene == true)
        {
            SceneLoader.Instance.CloseCutscene(SceneName);
        }
        else
        {
            if (oneShotOnly == true)
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
        foreach(GameObject _go in objectsToActivateWithoutDirector)
        {
            _go.SetActive(true);
        }
    }
}

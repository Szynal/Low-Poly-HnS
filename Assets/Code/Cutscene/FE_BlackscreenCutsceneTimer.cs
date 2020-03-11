using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FE_BlackscreenCutsceneTimer : MonoBehaviour
{
    [SerializeField] FE_CutsceneController cutsceneController = null;
    [SerializeField] Text timerText = null;

    private float timeLeft = 0f;

    private void Awake()
    {
        cutsceneController.CountdownDelegate += handleCountdown;
    }

    private void handleCountdown(float _time)
    {
        timeLeft = _time;
        timerText.text = timeLeft.ToString();

        StartCoroutine(countDown());
    }

    private IEnumerator countDown()
    {
        while(timeLeft >= 0f)
        {
            timeLeft -= Time.unscaledDeltaTime;
            timerText.text = timeLeft.ToString("f");
            yield return null;
        }
    }
}

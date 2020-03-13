using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlackscreenCutsceneTimer : MonoBehaviour
{
    [SerializeField] private CutsceneController cutsceneController = null;
    [SerializeField] private Text timerText = null;

    private float timeLeft;

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
        while (timeLeft >= 0f)
        {
            timeLeft -= Time.unscaledDeltaTime;
            timerText.text = timeLeft.ToString("f");
            yield return null;
        }
    }
}
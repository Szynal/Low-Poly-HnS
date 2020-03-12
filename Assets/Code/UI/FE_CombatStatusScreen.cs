﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FE_CombatStatusScreen : MonoBehaviour
{
    [SerializeField] Image imgTargeting = null;
    [SerializeField] Image imgAmmoWarning = null;
    
    [SerializeField] float ammoWarningAnimSpeed = 0.4f;

    private bool needWarning = false;
    private Coroutine warningCoroutine;

    private void Awake()
    {
        if(imgTargeting == null || imgAmmoWarning == null)
        {
            Debug.LogError("There are unassigned field in CombatStatusScreen " + name);
        }

        imgTargeting.enabled = false;
        imgAmmoWarning.enabled = false;
    }

    public void ShowTargetingStatus(bool _hasTarget, bool _critical)
    {
        imgTargeting.enabled = _hasTarget;
        if(_critical == true)
        {
            imgTargeting.color = Color.red;
        }
        else
        {
            imgTargeting.color = Color.green;
        }
    }

    public void AmmoLevelWarning(bool _isWarning)
    {
        if(_isWarning == true)
        {
            imgAmmoWarning.enabled = true;
            needWarning = true;

            if(warningCoroutine != null)
            {
                StopCoroutine(warningCoroutine);
            }

            if (gameObject.activeSelf == true)
            {
                warningCoroutine = StartCoroutine(flashWarning());
            }
        }
        else
        {
            imgAmmoWarning.enabled = false;
            needWarning = false;

            if (warningCoroutine != null)
            {
                StopCoroutine(warningCoroutine);
            }
        }
    }

    private IEnumerator flashWarning()
    {
        float _warnTime = ammoWarningAnimSpeed;
        Color _visibleColor = new Color(imgAmmoWarning.color.r, imgAmmoWarning.color.g, imgAmmoWarning.color.b, 1f);
        Color _invisibleColor = new Color(imgAmmoWarning.color.r, imgAmmoWarning.color.g, imgAmmoWarning.color.b, 0f);

        imgAmmoWarning.color = _visibleColor;

        while (needWarning == true)
        {
            _warnTime -= Time.deltaTime;

            if(_warnTime <= 0f)
            {
                if (imgAmmoWarning.color.a > 0f)
                {
                    imgAmmoWarning.color = _invisibleColor;
                }
                else
                {
                    imgAmmoWarning.color = _visibleColor;
                }

                _warnTime = ammoWarningAnimSpeed;
            }

            yield return null;
        }

        yield break;
    }
}
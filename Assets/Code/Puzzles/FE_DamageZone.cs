using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FE_DamageZone : MonoBehaviour
{
    [System.Serializable]
    public enum EDamageType
    {
        FlatDamage,
        PercentageDamage,
        InstantKill
    };

    [System.Serializable]
    public enum EActivationType
    {
        SelfActivation,
        ExternalControl
    };
   
    [HideInInspector] public EDamageType DamageType;
    [HideInInspector] public float DamageAmount = 0f;

    [HideInInspector] public EActivationType ActivationType;

    [HideInInspector] public bool DamageOnTick = false;
    [HideInInspector] public float TickTime = 1f;

    [HideInInspector] public bool ShouldStartEnabled = false;

    [HideInInspector] public float ActiveTime = 1f;
    [HideInInspector] public float InactiveTime = 1f;

    [HideInInspector] public bool ShouldDelayStart = false;
    [HideInInspector] public float StartDelayTime = 1f;

    [HideInInspector] public GameObject RepresentationObj = null;
    [HideInInspector] public bool RepresentionReverseActivation = false;

    [HideInInspector] public bool ZoneActive = false;

    private Collider damageCollider = null;
    private List<FE_Health> objectsToDamage = new List<FE_Health>();

    protected virtual void Awake()
    {
        damageCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        if(ShouldStartEnabled == true)
        {
            SetState(true, true);

            if (ActivationType == EActivationType.SelfActivation)
            {
                StartCoroutine(timeOnOffState(false));
            }
        }
        else
        {
            SetState(false, true);

            if (ActivationType == EActivationType.SelfActivation)
            {
                StartCoroutine(timeOnOffState(true));
            }
        }
    }

    private void OnTriggerEnter(Collider _other)
    {
        FE_Health _foundHealth = _other.GetComponent<FE_Health>();

        if(_foundHealth != null && _foundHealth.IgnoreDamageZones == false)
        {
            if (DamageType == EDamageType.FlatDamage)
            {
                _foundHealth.TakeDamage((int)DamageAmount, transform);
            }
            else if(DamageType == EDamageType.PercentageDamage)
            {
                _foundHealth.TakeDamage((int)(_foundHealth.GetMaxHealth() * DamageAmount / 100f), transform);
            }
            else
            {
                _foundHealth.TakeDamage(_foundHealth.GetMaxHealth(), transform, false, true);
                Debug.Log("InstantKill");
            }

            if(DamageOnTick == true)
            {
                objectsToDamage.Add(_foundHealth);
                StartCoroutine(handleDamagingObject(_foundHealth));
            }
        }
    }

    private void OnTriggerExit(Collider _other)
    {
        FE_Health _otherHealth = _other.GetComponent<FE_Health>();
        if (objectsToDamage.Contains(_otherHealth))
        {
            objectsToDamage.Remove(_otherHealth);
        }
    }

    private IEnumerator timeOnOffState(bool _isChangingToActive)
    {
        if(ShouldDelayStart == true && StartDelayTime >= 0f)
        {
            ShouldDelayStart = false;
            yield return new WaitForSeconds(StartDelayTime);
        }

        float _timeToWait = _isChangingToActive ? InactiveTime : ActiveTime;
        if(ShouldDelayStart == true && StartDelayTime < 0f)
        {
            _timeToWait -= Mathf.Abs(StartDelayTime);
        }

        yield return new WaitForSeconds(_timeToWait);

        SetState(_isChangingToActive, true);

        StartCoroutine(timeOnOffState(!_isChangingToActive));
    }

    private IEnumerator handleDamagingObject(FE_Health _target)
    {
        while (objectsToDamage.Contains(_target) == true)
        {
            while (_target.IsInvulnerable == true)
            {
                yield return null;
            }

            yield return new WaitForSeconds(TickTime);

            if (objectsToDamage.Contains(_target) == true)
            {
                if (DamageType == EDamageType.FlatDamage)
                {
                    _target.TakeDamage((int)DamageAmount, transform);
                }

                else if (DamageType == EDamageType.PercentageDamage)
                {
                    _target.TakeDamage((int)(_target.GetMaxHealth() * DamageAmount / 100f), transform);
                }
            }
        }
    }

    public void SetState(bool _isActive, bool _forceChange = false)
    {
        if(_forceChange == true || ActivationType == EActivationType.ExternalControl)
        {
            if (RepresentationObj != null)
            {
                handleShowingRepresentation(_isActive);
            }

            if (damageCollider != null)
            {
                damageCollider.enabled = _isActive;
            }
            ZoneActive = _isActive;

            if(_isActive == false && objectsToDamage.Count > 0)
            {
                objectsToDamage.Clear();
            }
        }
    }

    protected virtual void handleShowingRepresentation(bool _show)
    {
        if(RepresentionReverseActivation == true)
        {
            _show = !_show;
        }

        RepresentationObj.SetActive(_show);
    }
}

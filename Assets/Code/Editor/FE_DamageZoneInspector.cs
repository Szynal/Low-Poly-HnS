using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(FE_DamageZone), true)]
public class FE_DamageZoneInspector : Editor
{
    FE_DamageZone targetZone;

    private void OnEnable()
    {
        targetZone = target as FE_DamageZone;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();     
        EditorGUILayout.LabelField("Properties for DamageZone", EditorStyles.boldLabel);

        bool _startOnEnable, _perTickDmg, _reverseRepresentation;
        bool _delayStart = targetZone.ShouldDelayStart;
        float _newDmg = targetZone.DamageAmount, _tickTime = targetZone.TickTime, _activeTime = targetZone.ActiveTime, _inactiveTime = targetZone.InactiveTime, _startDelayTime = targetZone.StartDelayTime;
        FE_DamageZone.EActivationType _activationType;
        FE_DamageZone.EDamageType _dmgType;
        GameObject _representation;

        EditorGUI.BeginChangeCheck();

        _startOnEnable = EditorGUILayout.Toggle(label: "Should Start Enabled", targetZone.ShouldStartEnabled);

        _representation = (GameObject)EditorGUILayout.ObjectField(label: "Representation Object", targetZone.RepresentationObj, typeof(GameObject), true);
        _reverseRepresentation = EditorGUILayout.Toggle(label: "Reverse representation showing", targetZone.RepresentionReverseActivation);

        _activationType = (FE_DamageZone.EActivationType)EditorGUILayout.EnumPopup(label: "Activation Type", targetZone.ActivationType);

        if (_activationType == FE_DamageZone.EActivationType.SelfActivation)
        {
            _activeTime = EditorGUILayout.FloatField(label: "Active Time", targetZone.ActiveTime);
            _inactiveTime = EditorGUILayout.FloatField(label: "Inactive Time", targetZone.InactiveTime);

            _delayStart = EditorGUILayout.Toggle(label: "Should Delay First Cycle", targetZone.ShouldDelayStart);
            if(_delayStart == true)
            {
                _startDelayTime = EditorGUILayout.FloatField(label: "First Cycle Delay Time", targetZone.StartDelayTime);
            }
        }

        _dmgType = (FE_DamageZone.EDamageType) EditorGUILayout.EnumPopup(label: "Damage Type", targetZone.DamageType);
        if (_dmgType == FE_DamageZone.EDamageType.FlatDamage)
        {
            _newDmg = EditorGUILayout.FloatField(label: "Damage Amount", targetZone.DamageAmount);
        }
        else if (_dmgType == FE_DamageZone.EDamageType.PercentageDamage)
        {
            _newDmg = EditorGUILayout.Slider(label: "Health % to damage", targetZone.DamageAmount, 0f, 100f);
        }


        _perTickDmg = EditorGUILayout.Toggle(label: "Should Damage Per Tick", targetZone.DamageOnTick);

        if(_perTickDmg == true)
        {
            _tickTime = EditorGUILayout.FloatField(label: "Time Between Ticks", targetZone.TickTime);
        }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(targetZone, "Change zone's properties");
            targetZone.ShouldStartEnabled = _startOnEnable;
            targetZone.RepresentationObj = _representation;
            targetZone.RepresentionReverseActivation = _reverseRepresentation;

            targetZone.ActivationType = _activationType;

            targetZone.ActiveTime = _activeTime;
            targetZone.InactiveTime = _inactiveTime;

            targetZone.DamageType = _dmgType;
            targetZone.DamageAmount = _newDmg;

            targetZone.DamageOnTick = _perTickDmg;
            targetZone.TickTime = _tickTime;

            targetZone.ShouldDelayStart = _delayStart;
            targetZone.StartDelayTime = _startDelayTime;

            SceneView.RepaintAll();
        }
        GUI.enabled = true;
    }
}

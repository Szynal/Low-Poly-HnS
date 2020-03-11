using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FE_ActionContainer))]
public class FE_ActionContainerDrawer : PropertyDrawer
{
    private SerializedProperty  stateTypeProperty,
                                targetObjectProperty,
                                positionProperty,
                                msoNameProperty,
                                msoNewIDProperty,
                                itemToGiveProperty,
                                bossToStartProperty,
                                sceneTeleporterProperty,
                                msgRecieverProperty,
                                msgStringProperty,
                                cutsceneProperty;

    private SerializedProperty rotationProperty;

    private Transform tmpTrans;

    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        EditorGUI.BeginProperty(_position, _label, _property);

        Rect _baseRect = new Rect(_position.x, _position.y, _position.width, EditorGUIUtility.singleLineHeight);

        EditorGUI.PropertyField(_baseRect, stateTypeProperty);
        EActionType _stateType = (EActionType) stateTypeProperty.enumValueIndex;

        if (_stateType == EActionType.KillObject)
        {
            _baseRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(_baseRect, targetObjectProperty);
        }
        else if (_stateType == EActionType.MoveObjectToPos)
        {
            _baseRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(_baseRect, targetObjectProperty);

            _baseRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(_baseRect, positionProperty);

            _baseRect.y += EditorGUIUtility.singleLineHeight * 2f;
            tmpTrans = (Transform) EditorGUI.ObjectField(_baseRect, tmpTrans, typeof(Transform), true);

            _baseRect.y += EditorGUIUtility.singleLineHeight;
            if (tmpTrans != null)
            {
                if (GUI.Button(_baseRect, "Use this object's pos"))
                {
                    positionProperty.vector3Value = tmpTrans.position;
                }
            }
            else
            {
                EditorGUI.LabelField(_baseRect, "Assign a transform to copy its' position");
            }
        }
        else if (_stateType == EActionType.ChangeMSOStateByName)
        {
            _baseRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(_baseRect, msoNameProperty);

            _baseRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(_baseRect, msoNewIDProperty);
        }
        else if (_stateType == EActionType.GivePlayerItem)
        {
            _baseRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(_baseRect, itemToGiveProperty);
        }
        else if (_stateType == EActionType.StartBossFight)
        {
            _baseRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(_baseRect, bossToStartProperty);
        }
        else if (_stateType == EActionType.UseSceneTeleporter)
        {
            _baseRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(_baseRect, sceneTeleporterProperty);
        }
        else if (_stateType == EActionType.SendMessage)
        {
            _baseRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(_baseRect, msgRecieverProperty);

            _baseRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(_baseRect, msgStringProperty);
        }
        else if (_stateType == EActionType.StartInsceneCutscene)
        {
            _baseRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(_baseRect, cutsceneProperty);
        }
        else if (_stateType == EActionType.RotateObjectToAngle)
        {
            _baseRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(_baseRect, targetObjectProperty);

            _baseRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(_baseRect, rotationProperty);

            _baseRect.y += EditorGUIUtility.singleLineHeight * 2f;
            tmpTrans = (Transform) EditorGUI.ObjectField(_baseRect, tmpTrans, typeof(Transform), true);

            _baseRect.y += EditorGUIUtility.singleLineHeight;
            if (tmpTrans != null)
            {
                if (GUI.Button(_baseRect, "Use this object's rot"))
                {
                    rotationProperty.quaternionValue = tmpTrans.rotation;
                }
            }
            else
            {
                EditorGUI.LabelField(_baseRect, "Assign a transform to copy its' rotation");
            }
        }
        else if(_stateType == EActionType.RemoveItemFromPlayer)
        {
            _baseRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(_baseRect, itemToGiveProperty, new GUIContent("Item ID To Remove"));
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
    {
        stateTypeProperty = _property.FindPropertyRelative("StateType");
        targetObjectProperty = _property.FindPropertyRelative("TargetObject");
        positionProperty = _property.FindPropertyRelative("Pos");
        msoNameProperty = _property.FindPropertyRelative("MSOName");
        msoNewIDProperty = _property.FindPropertyRelative("NewMSOState");
        itemToGiveProperty = _property.FindPropertyRelative("ItemIDToGive");
        bossToStartProperty = _property.FindPropertyRelative("FightToStart");
        sceneTeleporterProperty = _property.FindPropertyRelative("SceneTeleporterToUse");
        msgRecieverProperty = _property.FindPropertyRelative("MessageReciever");
        msgStringProperty = _property.FindPropertyRelative("MessageToSend");
        cutsceneProperty = _property.FindPropertyRelative("CutsceneToStart");
        rotationProperty = _property.FindPropertyRelative("Rotation");


        int _numberOfFieldsToShow = 0;

        if ((EActionType) stateTypeProperty.enumValueIndex == EActionType.ChangeMSOStateByName || (EActionType) stateTypeProperty.enumValueIndex == EActionType.SendMessage)
        {
            _numberOfFieldsToShow = 4;
        }
        else if ((EActionType) stateTypeProperty.enumValueIndex == EActionType.KillObject
                 || (EActionType) stateTypeProperty.enumValueIndex == EActionType.StartBossFight
                 || (EActionType) stateTypeProperty.enumValueIndex == EActionType.GivePlayerItem
                 || (EActionType) stateTypeProperty.enumValueIndex == EActionType.UseSceneTeleporter
                 || (EActionType) stateTypeProperty.enumValueIndex == EActionType.StartInsceneCutscene
                 || (EActionType)stateTypeProperty.enumValueIndex == EActionType.RemoveItemFromPlayer)
        {
            _numberOfFieldsToShow = 3;
        }
        else if ((EActionType) stateTypeProperty.enumValueIndex == EActionType.MoveObjectToPos || (EActionType) stateTypeProperty.enumValueIndex == EActionType.RotateObjectToAngle)
        {
            _numberOfFieldsToShow = 7;
        }
        else
        {
            _numberOfFieldsToShow = 2;
        }

        return _numberOfFieldsToShow * base.GetPropertyHeight(_property, _label);
    }
}
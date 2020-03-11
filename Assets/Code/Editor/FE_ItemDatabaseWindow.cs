using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FE_ItemDatabaseWindow : EditorWindow
{
    private FE_ItemDatabase databaseObject = null;
    private SerializedObject m_serializedObject;
    private SerializedProperty m_serializedWeapons;
    private SerializedProperty m_serializedUseables;

    private Vector2 m_scrollPos = Vector2.zero;

    private void Init()
    {
        if (databaseObject == null)
        {
            var _assetGUIDs = AssetDatabase.FindAssets("t:" + typeof(FE_ItemDatabase).Name);
            if (_assetGUIDs.Length > 0)
            {
                if (_assetGUIDs.Length > 1)
                {
                    Debug.LogWarning("There are more than 1 item databases in the assets. Only 1 item database is supported. Opening the database at path " + AssetDatabase.GUIDToAssetPath(_assetGUIDs[0]));
                }

                databaseObject = (FE_ItemDatabase)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(_assetGUIDs[0]), typeof(FE_ItemDatabase));
                databaseObject.SortLists();
            }
            else
            {
                return;
            }
        }

        if (m_serializedObject == null)
        {
            m_serializedObject = new SerializedObject(databaseObject);
            m_serializedWeapons = m_serializedObject.FindProperty("Weapons");
            m_serializedUseables = m_serializedObject.FindProperty("Useables");
        }
    }

    private void OnGUI()
    {
        Init();
    
        m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos, GUILayout.Width(EditorGUIUtility.currentViewWidth), GUILayout.Height(position.height));
        if (m_serializedObject != null)
        {
            m_serializedObject.Update();

            showList(m_serializedWeapons);
            showList(m_serializedUseables);

            m_serializedObject.ApplyModifiedProperties();
        }
        else
        {
            EditorGUI.LabelField(new Rect(Vector2.zero, new Vector2(EditorGUIUtility.currentViewWidth, position.height)), "Item database could not be found. \n Create a new item database asset before trying to edit it.");
        }

        EditorGUILayout.EndScrollView();
    }

    private void showList(SerializedProperty _list)
    {  
       // EditorGUILayout.PropertyField(_list);

       // if (_list.isExpanded == true)
       // {
            EditorGUILayout.PropertyField(_list.FindPropertyRelative("Array.size"));

            for (int i = 0; i < _list.arraySize; i++)
            {
                if (_list.GetArrayElementAtIndex(i).objectReferenceValue != null)
                {
                    int _itemID = ((FE_Item)_list.GetArrayElementAtIndex(i).objectReferenceValue).itemID;
                    EditorGUILayout.PropertyField(_list.GetArrayElementAtIndex(i), new GUIContent("Item ID: " + _itemID.ToString()));
                }
                else
                {
                    EditorGUILayout.PropertyField(_list.GetArrayElementAtIndex(i), new GUIContent("Item ID: ---"));
                }
            }
      //  }
    }

    [MenuItem("FearEffect/Show item database")]
    static void showWindow()
    {
        FE_ItemDatabaseWindow _window = GetWindow<FE_ItemDatabaseWindow>("Item database");
        _window.Init();
    }
}

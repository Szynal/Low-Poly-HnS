using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PD_BezierPoint))]
public class BezierPointEditor : Editor {

	private PD_BezierSpline spline;
	private PD_BezierPoint point;

	private Quaternion precedingPointRotation = Quaternion.identity;
	private Quaternion followingPointRotation = Quaternion.identity;
	private bool controlPointRotationsInitialized = false;

	private Color RESET_POINT_BUTTON_COLOR = Color.yellow;
    private Color REMOVE_POINT_BUTTON_COLOR = Color.red;

	void OnEnable() {

		point = target as PD_BezierPoint;
		spline = point.GetComponentInParent<PD_BezierSpline>();

		if( spline != null && !spline.Equals(null)){
        
            spline.Refresh();
        }
	}

	void OnSceneGUI() {
    
		if(Tools.current != Tool.Move){

			controlPointRotationsInitialized = false;
			return;
		}

		if(controlPointRotationsInitialized == false) {

			precedingPointRotation = Quaternion.LookRotation(point.precedingControlPointPosition - point.position);
			followingPointRotation = Quaternion.LookRotation(point.followingControlPointPosition - point.position);

			controlPointRotationsInitialized = true;
		}

        EditorGUI.BeginChangeCheck();
        Vector3 _position = Handles.PositionHandle(point.precedingControlPointPosition, Tools.pivotRotation == PivotRotation.Local ? precedingPointRotation : Quaternion.identity);

        if (EditorGUI.EndChangeCheck()) {

            Undo.RecordObject(point, "Move Control Point");
            point.precedingControlPointPosition = _position;
        }

        EditorGUI.BeginChangeCheck();
        _position = Handles.PositionHandle(point.followingControlPointPosition, Tools.pivotRotation == PivotRotation.Local ? followingPointRotation : Quaternion.identity);

        if (EditorGUI.EndChangeCheck()) {

            Undo.RecordObject(point, "Move Control Point");
            point.followingControlPointPosition = _position;
        }
	}

	public override void OnInspectorGUI(){

		Color _color = GUI.color;

		if(spline != null && !spline.Equals(null)){

			PD_BezierUtils.DrawSplineInspectorGUI(spline);

			if(point == null || point.Equals(null)){

                return;
            }
			
			if( GUILayout.Button("Insert Point Before")){
            
                InsertNewPointAt(spline.IndexOf(point));
            }
            
			if( GUILayout.Button("Insert Point After")){
            
                InsertNewPointAt(spline.IndexOf(point) + 1);
            }
		}
		
        EditorGUILayout.Space();
		EditorGUI.BeginChangeCheck();

		HandleMode handleMode = (HandleMode) EditorGUILayout.EnumPopup("Handle Mode", point.handleMode);

		if(EditorGUI.EndChangeCheck()) {

			Undo.RecordObject( point, "Change Point Handle Mode" );
			point.handleMode = handleMode;

			SceneView.RepaintAll();
		}

        EditorGUILayout.Space();

		if(spline != null && !spline.Equals(null)) {

			GUI.color = RESET_POINT_BUTTON_COLOR;

			if(GUILayout.Button("Reset Point")) {

				ResetEndPointAt(spline.IndexOf(point));
				SceneView.RepaintAll();
			}
            
			GUI.color = REMOVE_POINT_BUTTON_COLOR;

			if(spline.Count <= 2) {
            
                GUI.enabled = false;
            }
				
			if(GUILayout.Button("Remove Point")){
            
                RemovePointAt(spline.IndexOf(point));
            }

			GUI.enabled = true;
		}

		GUI.color = _color;
	}
    
	private void InsertNewPointAt(int _index) {

		Vector3 _position;

		if(spline.Count >= 2) {

			if(_index > 0 && _index < spline.Count) {

				_position = (spline[_index - 1].localPosition + spline[_index].localPosition) * 0.5f;

			} else if(_index == 0) {

				if( spline.loop == true) {

					_position = (spline[0].localPosition + spline[spline.Count - 1].localPosition) * 0.5f;

                } else {

					_position = spline[0].localPosition - (spline[1].localPosition - spline[0].localPosition) * 0.5f;
                }

			} else {

				if(spline.loop == true) {

					_position = (spline[0].localPosition + spline[spline.Count - 1].localPosition) * 0.5f;

                } else {

					_position = spline[_index - 1].localPosition + (spline[_index - 1].localPosition - spline[_index - 2].localPosition) * 0.5f;
                }
			}

		} else if(spline.Count == 1) {
        
            if(_index == 0){

                _position = spline[0].localPosition - Vector3.forward;

            } else {
            
                _position = spline[0].localPosition + Vector3.forward;
            }

        } else {
        
            _position = Vector3.zero;
        }
			
		PD_BezierPoint _point = spline.InsertNewPointAt(_index);
		_point.localPosition = _position;

		Undo.IncrementCurrentGroup();
		Undo.RegisterCreatedObjectUndo(_point.gameObject, "Insert Point");
		Undo.RegisterCompleteObjectUndo(_point.transform.parent, "Insert Point");
		
		Selection.activeTransform = _point.transform;
		SceneView.RepaintAll();
	}
    
	private void RemovePointAt(int _index) {

		if(spline.Count <= 2){

            return;
        }

		Undo.IncrementCurrentGroup();
		Undo.DestroyObjectImmediate(spline[_index].gameObject);
		
		if(_index >= spline.Count){
        
            _index--;
        }
			
		Selection.activeTransform = spline[_index].transform;
		SceneView.RepaintAll();
	}

	private void ResetEndPointAt(int _index) {

		Undo.RecordObject(spline[_index].transform, "Reset Point");
		Undo.RecordObject(spline[_index], "Reset Point");

		spline[_index].Reset();
	}

	private void OnUndoRedo() {

		controlPointRotationsInitialized = false;

		if(spline != null && !spline.Equals(null)) {
        
            spline.Refresh();
        }
			
		Repaint();
	}

	private bool HasFrameBounds() {

		return true;
	}

	private Bounds OnGetFrameBounds(){

		return new Bounds(point.position, Vector3.one);
	}
}
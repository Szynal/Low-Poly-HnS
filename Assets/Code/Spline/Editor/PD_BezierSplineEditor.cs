using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PD_BezierSpline))]
public class PD_BezierSplineEditor : Editor {

	private PD_BezierSpline spline;

	void OnEnable() {

		spline = target as PD_BezierSpline;
		spline.Refresh();

		Undo.undoRedoPerformed -= OnUndoRedo;
		Undo.undoRedoPerformed += OnUndoRedo;
	}

	void OnDisable() {

		Undo.undoRedoPerformed -= OnUndoRedo;
	}

	void OnSceneGUI() {

		PD_BezierUtils.DrawSplineDetailed(spline);

		for(int i = 0; i < spline.Count; i++){
        
            PD_BezierUtils.DrawBezierPoint(spline[i], i + 1, false);
        }
        
	}

	public override void OnInspectorGUI() {
    
		PD_BezierUtils.DrawSplineInspectorGUI(spline);
	}

	private void OnUndoRedo() {

		if(spline != null && !spline.Equals(null)) {
        
            spline.Refresh();
        }
			
		Repaint();
	}

	private bool HasFrameBounds() {

		return true;
	}

	private Bounds OnGetFrameBounds() {

		return new Bounds(spline.transform.position, Vector3.one);
	}
}

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public enum SplineColor {

    green,
    red,
    blue,
    pink,
    purple,
    orange,
    cyan,
    black,
    white,
    yellow,
    gray,
    darkGreen,
    darkBlue,
    brown,
    lightBlue,
}

[ExecuteInEditMode]
public class PD_BezierSpline : MonoBehaviour {

	private static Material gizmoMaterial;
	
	private Color gizmoColor = Color.white;
	private float gizmoStep = 0.05f;

	[HideInInspector] public List<PD_BezierPoint> endPoints = new List<PD_BezierPoint>();
	
	public bool loop = false;
	public bool drawGizmos = false;
    public bool drawFullColor = false;
    public SplineColor splineColorFromEditor;

    [SerializeField] public Color splineColorEditor;
    public Color splineColor;

	public int Count { 

        get { 

            return endPoints.Count; 
        } 
    }

	public float Length { 

        get { 
        
            return GetLengthApproximately(0f, 1f); 
        } 
    }
	
	public PD_BezierPoint this[int _index] {

		get {

			if(_index < Count) {
            
                return endPoints[_index];
            }
				
			return null;
		}
	}

	private void Awake() {

		Refresh();
	}

    private void Update() {

        setColor();
    }

    private void setColor() {
    
        switch (splineColorFromEditor) {

            case SplineColor.green:
            splineColor = Color.green;
            break;

            case SplineColor.blue:
            splineColor = Color.blue;
            break;

            case SplineColor.red:
            splineColor = Color.red;
            break;

            case SplineColor.pink:
            splineColor = new Color(1, 0, 0.745f);
            break;

            case SplineColor.purple:
            splineColor = new Color(0.2641509f, 0.02118192f, 2022177);
            break;

            case SplineColor.orange:
            splineColor = new Color(1f, 0.75f, 0f);
            break;

            case SplineColor.cyan:
            splineColor = Color.cyan;
            break;

            case SplineColor.black:
            splineColor = new Color(0f, 0f, 0f);
            break;

            case SplineColor.white:
            splineColor = new Color(1f, 1f, 1f);
            break;

            case SplineColor.yellow:
            splineColor = new Color(1f, 0.97f, 0f);
            break;

            case SplineColor.gray:
            splineColor = new Color(0.45f, 0.45f, 0.45f);
            break;

            case SplineColor.darkBlue:
            splineColor = new Color(0.0f, 0.0f, 0.8f);
            break;

            case SplineColor.darkGreen:
            splineColor = new Color(0.0f, 0.38f, 0.0f);
            break;

            case SplineColor.brown:
            splineColor = new Color(0.44f, 0.22f, 0.18f);
            break;

            case SplineColor.lightBlue:
            splineColor = new Color(0.3f, 0.95f, 1f);
            break;
        }
    }

#if UNITY_EDITOR

    private void OnTransformChildrenChanged() {

		Refresh();
	}
    
    #endif

    void OnDrawGizmos() {

        if (drawFullColor) {

            DrawSplineGizmo(this);
        }    
    }

	public void Initialize(int _endPointsCount) {

		if(_endPointsCount < 2) {

			Debug.LogError("Not enought bezier points!");
			return;
		}

		Refresh();

		for(int i = endPoints.Count - 1; i >= 0; i--) {
        
            DestroyImmediate(endPoints[i].gameObject);
        }
			
		endPoints.Clear();

		for(int i = 0; i < _endPointsCount; i++) {
        
            InsertNewPointAt(i);
        }
			
		Refresh();
	}

	public void Refresh() {

		endPoints.Clear();
		GetComponentsInChildren(endPoints);
	}

    public void DrawSplineGizmo(PD_BezierSpline _spline) {
    
		if(_spline.Count < 2) {

            return;
        }

		PD_BezierPoint _endPoint0 = null, endPoint1 = null;

		for(int i = 0; i < _spline.Count - 1; i++) {

			_endPoint0 = _spline[i];
			endPoint1 = _spline[i + 1];

			DrawBezier(_endPoint0, endPoint1, splineColor);
		}
	}

    private static void DrawBezier( PD_BezierPoint endPoint0, PD_BezierPoint endPoint1, Color _color) {

        #if UNITY_EDITOR

		Handles.DrawBezier( endPoint0.position, endPoint1.position, endPoint0.followingControlPointPosition, endPoint1.precedingControlPointPosition, _color, null, 8f);

        #endif
	}

	public PD_BezierPoint InsertNewPointAt(int _index) {

		if(_index < 0 || _index > endPoints.Count) {

			Debug.LogError("Index " + _index + " is out of range: [0," + endPoints.Count + "]");
			return null;
		}

		int _prevCount = endPoints.Count;

		PD_BezierPoint _point = new GameObject( "Point" ).AddComponent<PD_BezierPoint>();
		_point.transform.SetParent( endPoints.Count == 0 ? transform : ( _index == 0 ? endPoints[0].transform.parent : endPoints[_index - 1].transform.parent ), false );
		_point.transform.SetSiblingIndex( _index == 0 ? 0 : endPoints[_index - 1].transform.GetSiblingIndex() + 1 );

		if( endPoints.Count == _prevCount) {
        
            endPoints.Insert(_index, _point);
        }
			

		return _point;
	}

	public PD_BezierPoint DuplicatePointAt(int _index) {

		if(_index < 0 || _index >= endPoints.Count) {

			Debug.LogError("Index " + _index + " is out of range: [0," + ( endPoints.Count - 1 ) + "]");
			return null;
		}

		PD_BezierPoint _newPoint = InsertNewPointAt(_index + 1);
		endPoints[_index].CopyTo(_newPoint);

		return _newPoint;
	}

	public void RemovePointAt(int _index) {

		if(endPoints.Count <= 2) {

			Debug.LogError("Can't remove point: spline must consist of at least two points!");
			return;
		}

		if(_index < 0 || _index >= endPoints.Count) {

			Debug.LogError("Index " + _index + " is out of range: [0," + endPoints.Count + ")");
			return;
		}

		PD_BezierPoint _point = endPoints[_index];
		endPoints.RemoveAt(_index);

		DestroyImmediate(_point.gameObject);
	}

	public void SwapPointsAt(int _index1, int _index2) {

		if(_index1 == _index2) {

			Debug.LogError("It's the same point!");
			return;
		}

		if(_index1 < 0 || _index1 >= endPoints.Count || _index2 < 0 || _index2 >= endPoints.Count) {

			Debug.LogError("Points must be in range [0," + (endPoints.Count - 1) + "]");
			return;
		}

		PD_BezierPoint _point1 = endPoints[_index1];
		int _point1SiblingIndex = _point1.transform.GetSiblingIndex();

		endPoints[_index1] = endPoints[_index2];
		endPoints[_index2] = _point1;

		_point1.transform.SetSiblingIndex( endPoints[_index1].transform.GetSiblingIndex());
		endPoints[_index1].transform.SetSiblingIndex(_point1SiblingIndex);
	}

	public int IndexOf(PD_BezierPoint _point) {

		return endPoints.IndexOf(_point);
	}

	public void DrawGizmos(Color color, int smoothness = 4) {

		drawGizmos = true;
		gizmoColor = color;
		gizmoStep = 1f / (endPoints.Count * Mathf.Clamp(smoothness, 1, 30));
	}

	public void HideGizmos() {

		drawGizmos = false;
	}

	public Vector3 GetPoint(float _normalizedT) {

        if (_normalizedT <= 0f) {

            return endPoints[0].position;
        } else if (_normalizedT >= 1f) {

            if (loop == true) {

                return endPoints[0].position;
            }

            return endPoints[endPoints.Count - 1].position;
        }

        float _t = _normalizedT * getCount();

        PD_BezierPoint _startPoint, _endPoint;

        int _startIndex = (int) _t;
        int _endIndex = _startIndex + 1;

        if (_endIndex == endPoints.Count){
        
            _endIndex = 0;
        }
            
        _startPoint = endPoints[_startIndex];
        _endPoint = endPoints[_endIndex];

        float _localT = _t - _startIndex;
        float _oneMinusLocalT = 1f - _localT;

        return (_oneMinusLocalT * _oneMinusLocalT * _oneMinusLocalT * _startPoint.position) +
               (3f * _oneMinusLocalT * _oneMinusLocalT * _localT * _startPoint.followingControlPointPosition) +
               (3f * _oneMinusLocalT * _localT * _localT * _endPoint.precedingControlPointPosition) +
               (_localT * _localT * _localT * _endPoint.position);
    }

    private int getCount() {

        int _count = 0;

        if (loop == true){

            _count = endPoints.Count;

        } else {
        
            _count = (endPoints.Count - 1);
        }
        
        return _count;
    }

    public Vector3 GetTangent(float _normalizedT) {

		if(_normalizedT <= 0f) {
        
            return 3f * (endPoints[0].followingControlPointPosition - endPoints[0].position);

        } else if(_normalizedT >= 1f) {

			if(loop == true) {

				return 3f * (endPoints[0].position - endPoints[0].precedingControlPointPosition);

			} else {

				int _index = endPoints.Count - 1;
				return 3f * (endPoints[_index].position - endPoints[_index].precedingControlPointPosition);
			}
        }
			
		float _t = _normalizedT * getCount();

		PD_BezierPoint _startPoint, _endPoint;

		int _startIndex = (int) _t;
		int _endIndex = _startIndex + 1;

		if(_endIndex == endPoints.Count){
        
            _endIndex = 0;
        }
			
		_startPoint = endPoints[_startIndex];
		_endPoint = endPoints[_endIndex];

		float _localT = _t - _startIndex;
		float _oneMinusLocalT = 1f - _localT;

		return (3f * _oneMinusLocalT * _oneMinusLocalT * (_startPoint.followingControlPointPosition - _startPoint.position)) +
			   (6f * _oneMinusLocalT * _localT * (_endPoint.precedingControlPointPosition - _startPoint.followingControlPointPosition)) +
			   (3f * _localT * _localT * (_endPoint.position - _endPoint.precedingControlPointPosition));
	}

	public float GetLengthApproximately(float _startNormalizedT, float _endNormalizedT, float _accuracy = 50f) {

		if(_endNormalizedT < _startNormalizedT) {

			float _temp = _startNormalizedT;
			_startNormalizedT = _endNormalizedT;
			_endNormalizedT = _temp;
		}

		if(_startNormalizedT < 0f){
        
            _startNormalizedT = 0f;
        }
			
		if(_endNormalizedT > 1f){
        
            _endNormalizedT = 1f;
        }
			
		float _step = AccuracyToStepSize(_accuracy) * (_endNormalizedT - _startNormalizedT);

		float _length = 0f;
		Vector3 _lastPoint = GetPoint(_startNormalizedT);

		for(float i = _startNormalizedT + _step; i < _endNormalizedT; i += _step) {

			Vector3 _thisPoint = GetPoint(i);
			_length += Vector3.Distance(_thisPoint, _lastPoint);
			_lastPoint = _thisPoint;
		}

		_length += Vector3.Distance(_lastPoint, GetPoint(_endNormalizedT));

		return _length;
	}

	public Vector3 FindNearestPointTo(Vector3 _worldPos, float _accuracy = 100f) {

		float _normalizedT;
		return FindNearestPointTo(_worldPos, out _normalizedT, _accuracy);
	}

	public Vector3 FindNearestPointTo(Vector3 _worldPos, out float _normalizedT, float _accuracy = 100f) {

		Vector3 _result = Vector3.zero;
		_normalizedT = -1f;

		float _step = AccuracyToStepSize(_accuracy);

		float _minDistance = Mathf.Infinity;

		for(float i = 0f; i < 1f; i += _step) {

			Vector3 _thisPoint = GetPoint(i);
			float _thisDistance = (_worldPos - _thisPoint).sqrMagnitude;

			if(_thisDistance < _minDistance) {

				_minDistance = _thisDistance;
				_result = _thisPoint;
				_normalizedT = i;
			}
		}

		return _result;
	}

	public Vector3 MoveAlongSpline(ref float _normalizedT, float _deltaMovement, int _accuracy = 3) {
    
		float _1OverCount = 1f / endPoints.Count;

		for(int i = 0; i < _accuracy; i++) {
        
            _normalizedT += _deltaMovement * _1OverCount / (_accuracy * GetTangent(_normalizedT).magnitude);
        }
			
		return GetPoint(_normalizedT);
	}

	public void AutoConstructSpline() {

		for(int i = 0; i < endPoints.Count; i++) {
        
            endPoints[i].handleMode = HandleMode.Mirrored;
        }
			
		int _count = endPoints.Count - 1;

		if(_count == 1) {

			endPoints[0].followingControlPointPosition = ((2 * endPoints[0].position + endPoints[1].position ) / 3f);
			endPoints[1].precedingControlPointPosition = (2 * endPoints[0].followingControlPointPosition - endPoints[0].position);

			return;
		}

		Vector3[] _tempPoints;

		if(loop == true){
        
            _tempPoints = new Vector3[_count + 1];

        } else {
        
            _tempPoints = new Vector3[_count];
        }
			
		for(int i = 1; i < _count - 1; i++) {

			_tempPoints[i] = 4 * endPoints[i].position + 2 * endPoints[i + 1].position;
		}

		_tempPoints[0] = (endPoints[0].position + 2 * endPoints[1].position);

		if(loop == false) {
        
            _tempPoints[_count - 1] = ( 8 * endPoints[_count - 1].position + endPoints[_count].position ) * 0.5f;

        } else {
        
            _tempPoints[_count - 1] = 4 * endPoints[_count - 1].position + 2 * endPoints[_count].position;
			_tempPoints[_count] = ( 8 * endPoints[_count].position + endPoints[0].position ) * 0.5f;
        }

		Vector3[] _controlPoints = GetFirstControlPoints(_tempPoints);

		for(int i = 0; i < _count; i++) {
			
			endPoints[i].followingControlPointPosition = _controlPoints[i];

			if(loop == true) {

				endPoints[i + 1].precedingControlPointPosition = 2 * endPoints[i + 1].position - _controlPoints[i + 1];

			} else {
				
				if( i < _count - 1 ){
                
                    endPoints[i + 1].precedingControlPointPosition = 2 * endPoints[i + 1].position - _controlPoints[i + 1];

                } else {
                
                    endPoints[i + 1].precedingControlPointPosition = ( endPoints[_count].position + _controlPoints[_count - 1] ) * 0.5f;
                }
			}
		}

		if(loop == true) {

			float _controlPointDistance = Vector3.Distance( endPoints[0].followingControlPointPosition, endPoints[0].position );
			Vector3 _direction = Vector3.Normalize( endPoints[_count].position - endPoints[1].position );

			endPoints[0].precedingControlPointPosition = endPoints[0].position + _direction * _controlPointDistance * 0.5f;
			endPoints[0].followingControlPointLocalPosition = -endPoints[0].precedingControlPointLocalPosition;
		}
	}

	private static Vector3[] GetFirstControlPoints(Vector3[] _tempPoints) {
    
		int _count = _tempPoints.Length;
		Vector3[] _x = new Vector3[_count];
		float[] _temp = new float[_count];

		float _divider = 2f;
		_x[0] = _tempPoints[0] / _divider;

		for(int i = 1; i < _count; i++) {

			float _val = 1f / _divider;
			_temp[i] = _val;
			_divider = ( i < _count - 1 ? 4f : 3.5f ) - _val;
			_x[i] = ( _tempPoints[i] - _x[i - 1] ) / _divider;
		}

		for(int i = 1; i < _count; i++) {

			_x[_count - i - 1] -= _temp[_count - i] * _x[_count - i];
		}

		return _x;
	}

	private float AccuracyToStepSize(float _accuracy){

		if(_accuracy <= 0f){
        
            return 0.2f;
        }
			
		return Mathf.Clamp(1f / _accuracy, 0.001f, 0.2f);
	}
    
	private void OnRenderObject() {

		if(drawGizmos == false || endPoints.Count < 2){

            return;
        }

		if( !gizmoMaterial ) {

			Shader shader = Shader.Find( "Hidden/Internal-Colored" );
			gizmoMaterial = new Material( shader ) { hideFlags = HideFlags.HideAndDontSave };
			gizmoMaterial.SetInt( "_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha );
			gizmoMaterial.SetInt( "_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha );
			gizmoMaterial.SetInt( "_Cull", (int) UnityEngine.Rendering.CullMode.Off );
			gizmoMaterial.SetInt( "_ZWrite", 0 );
		}
		
		gizmoMaterial.SetPass(0);
		
		GL.Begin(GL.LINES);
		GL.Color(gizmoColor);

		Vector3 _lastPosition = endPoints[0].position;

		for(float i = gizmoStep; i < 1f; i += gizmoStep) {

			GL.Vertex3(_lastPosition.x, _lastPosition.y, _lastPosition.z);
			_lastPosition = GetPoint(i);
			GL.Vertex3( _lastPosition.x, _lastPosition.y, _lastPosition.z);
		}

		GL.Vertex3(_lastPosition.x, _lastPosition.y, _lastPosition.z);
		_lastPosition = GetPoint(1f);
		GL.Vertex3(_lastPosition.x, _lastPosition.y, _lastPosition.z);
		GL.End();
    }

#if UNITY_EDITOR

	public void Reset() {

		for(int i = endPoints.Count - 1; i >= 0; i--){
        
            UnityEditor.Undo.DestroyObjectImmediate(endPoints[i].gameObject);
        }
			
		Initialize(2);

		endPoints[0].localPosition = Vector3.back;
		endPoints[1].localPosition = Vector3.forward;

		UnityEditor.Undo.RegisterCreatedObjectUndo(endPoints[0].gameObject, "Initialize Spline");
		UnityEditor.Undo.RegisterCreatedObjectUndo(endPoints[1].gameObject, "Initialize Spline");

		UnityEditor.Selection.activeTransform = endPoints[0].transform;
	}

#endif
}

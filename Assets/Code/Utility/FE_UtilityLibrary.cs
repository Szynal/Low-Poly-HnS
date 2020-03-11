using UnityEngine;
using UnityEngine.AI;

public class FE_UtilityLibrary
{
    /// <summary>
    /// Returns a distance between Vector 1 and Vector 2 in X and Z coordinates.
    /// </summary>
    /// <param name="_vector1">First vector to calutate distance</param>
    /// <param name="_vector2">Second vector to calculate distance</param>
    /// <returns></returns>
    public static float FlatDistance(Vector3 _vector1, Vector3 _vector2)
    {
        _vector1 = new Vector3(_vector1.x, 0f, _vector1.z);
        _vector2 = new Vector3(_vector2.x, 0f, _vector2.z);

        return Vector3.Distance(_vector1, _vector2);
    }

    /// <summary>
    /// Returns a vector that's only taking X and Z coordinates into account. It's based either on Y = 0 or Y taken from another vector.
    /// </summary>
    /// <param name="_originalVector"> Original vector we wish to level.</param>
    /// <param name="_vectorToLevelWith"> Optional vector we want to take the Y parameter from.</param>
    /// <returns></returns>
    public static Vector3 FlatVector(Vector3 _originalVector, Vector3 _vectorToLevelWith = default(Vector3))
    {
        return new Vector3(_originalVector.x, _vectorToLevelWith.y, _originalVector.z);
    }


    /// <summary>
    /// Returns a position on navmesh that's somewhere inside a circle of given distance around the original pos.
    /// </summary>
    /// <param name="_originalPos"> Original position we wish to randomize.</param>
    /// <param name="_maxDistOut"> Radius of the search circle.</param>
    /// <param name="_areaMask"> Area mask we use to search for position.</param>
    /// <returns></returns>
    public static Vector3 NavmeshPosAround(Vector3 _originalPos, float _maxDistOut, int _areaMask)
    {
        Vector3 _changedPos = _originalPos + new Vector3(Random.Range(-_maxDistOut, _maxDistOut), 0f, Random.Range(-_maxDistOut, _maxDistOut));

        int _numOfTries = 5;

        NavMeshHit _navHit = new NavMeshHit();
        while(NavMesh.SamplePosition(_changedPos, out _navHit, 2f, _areaMask) == false)
        {
            _numOfTries--;

            if(_numOfTries <= 0)
            {
                return _originalPos;
            }
        }

        return _navHit.position;
    }

    // MPS:dpienkowska Angle methods 31/10/2019
    /// <summary>
    /// Returns an angle in degrees between two vectors taking only X and Z coordinates into account (Y = 0).
    /// </summary>
    /// <param name="_from">Vector to calculate angle from</param>
    /// <param name="_to">Vector to calculate angle to</param>
    /// <returns></returns>
    public static float FlatAngle(Vector3 _from, Vector3 _to)
    {
        _from = new Vector3(_from.x, 0f, _from.z);
        _to = new Vector3(_to.x, 0f, _to.z);

        return Vector3.Angle(_from, _to);
    }

    // MPS:dpienkowska Angle methods 31/10/2019
    /// <summary>
    /// Returns a signed angle in degrees between two vectors relative to Y-axis taking only X and Z coordinates into account (Y = 0).
    /// </summary>
    /// <param name="_from">Vector to calculate angle from</param>
    /// <param name="_to">Vector to calculate angle to</param>
    /// <returns></returns>
    public static float FlatSignedAngle(Vector3 _from, Vector3 _to)
    {
        _from = new Vector3(_from.x, 0f, _from.z);
        _to = new Vector3(_to.x, 0f, _to.z);

        return Vector3.SignedAngle(_from, _to, Vector3.up);
    }

    // MPS:dpienkowska Movement helpers 06/11/2019
    /// <summary>
    /// Returns position on NavMesh that is closes to given position in given range. If not found then Vector3(Infinity, Infinity, Infinity).
    /// </summary>
    /// <param name="_position">Position the closest should result be</param>
    /// <param name="_maxDistance">NavMesh position max distance (deafult = 2f)</param>
    /// <param name="_areaMask">Sample NavMesh area mask (default = NavMesh.AllAreas = -1) </param>
    /// <returns></returns>
    public static Vector3 ClosestNavMeshPosition(Vector3 _position, float _maxDistance = 2f, int _areaMask = NavMesh.AllAreas)
    {
        NavMesh.SamplePosition(_position, out NavMeshHit _navHit, _maxDistance, _areaMask);
        return _navHit.position;
    }

    /// <summary>
    /// Returns a sample position on NavMesh near the position, on the side of the target.
    /// </summary>
    /// <param name="_position">Position around which we're looking</param>
    /// <param name="_target">Target towards which we're facing our position</param>
    /// <param name="_directionalBias">How far from the _position are we facing towards _target</param>
    /// <param name="_areaMask">Areas we're looking in</param>
    /// <returns></returns>
    public static Vector3 ClosestNavMeshPositionTowardsTarget(Vector3 _position, Vector3 _target, float _directionalBias = 1f, int _areaMask = NavMesh.AllAreas)
    {
        Vector3 _changedPos = _position + (_target - _position).normalized * _directionalBias;
        NavMesh.SamplePosition(_changedPos, out NavMeshHit _navHit, 2f, _areaMask);
        return _navHit.position;
    }

    // MPS:dpienkowska Movement helpers 06/11/2019
    /// <summary>
    /// Returns -1 (left), 1 (right) or 0 (middle) value depending on which side is the target relative to given source forward direction.
    /// </summary>
    /// <param name="_forward">Source forward direction</param>
    /// <param name="_targetDirection">Direction to target from source position</param>
    /// <returns></returns>
    public static int GetSide(Vector3 _forward, Vector3 _targetDirection)
    {
        Vector3 _flatForward = FE_UtilityLibrary.FlatVector(_forward);
        Vector3 _flatTargetDirection = FE_UtilityLibrary.FlatVector(_targetDirection);

        float _angle = Vector3.SignedAngle(_flatForward, _flatTargetDirection, Vector3.up);

        if(_angle == 0f)
        {
            return 0;
        }

        return _angle < 0f ? -1 : 1;
    }

    // MPS:dpienkowska Detection layer mask check helper 06/11/2019
    /// <summary>
    /// Returns true if given mask contains given layer. False if otherwise.
    /// </summary>
    /// <param name="_layer">Layer to check</param>
    /// <param name="_mask">Mask to check layer against</param>
    /// <returns></returns>
    public static bool IsLayerInMask(int _layer, LayerMask _mask)
    {
        return _mask == (_mask | (1 << _layer));
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;

[RequireComponent(typeof(IViewable))]
public class FieldOfView : MonoBehaviour {

	public float viewRadius;
	[Range(0,360)]
	public float viewAngle;

	public LayerMask targetMask;
	public LayerMask obstacleMask;

	[HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>();
	private readonly List<Transform> _noneLostTargets = new List<Transform>();

	public float meshResolution;
	public int edgeResolveIterations;
	public float edgeDstThreshold;

	public MeshFilter viewMeshFilter;
	private Mesh _viewMesh;
	private LineRenderer _line;

	public Transform eye;

	private Func<float> _viewCenter;

	private readonly bool _onDev;

	private FieldOfView()
    {
#if UNITY_EDITOR
	    _onDev = true;
#endif
		_viewCenter = LocalHorizontal;
	}

	private void Start() {
		_viewMesh = new Mesh
		{
			name = "View Mesh"
		};
		viewMeshFilter.mesh = _viewMesh;
		
		_line = GetComponentInChildren<LineRenderer>();
		StartCoroutine(FindTargetsWithDelay(.2f));
	}


	private IEnumerator FindTargetsWithDelay(float delay) {
		while (true) {
			yield return new WaitForSeconds (delay);
			FindVisibleTargets ();
		}
	}

	private void LateUpdate() {
		if (!_onDev) return;

		if (visibleTargets.Count > 0)
		{
			DrawLocked();
		}
		else
		{
			DrawFieldOfView();
		}
	}

	private void FindVisibleTargets() {
		var controller = GetComponent<IViewable>();
		var newVisibleTargets = new List<Transform>(_noneLostTargets);
		var targetsInViewRadius = Physics.OverlapSphere (eye.position, viewRadius, targetMask);
		foreach (var c in targetsInViewRadius)
		{
			var target = c.transform;

			var dirToTarget = (target.position - eye.position).normalized;
			if (Vector3.Angle (DirFromAngle(_viewCenter(), true), dirToTarget) < viewAngle / 2) {
				var dstToTarget = Vector3.Distance (eye.position, target.position);
				if (Physics.Raycast(eye.position, dirToTarget, dstToTarget, obstacleMask)) continue;
				newVisibleTargets.Add(target);
			}
		}
		
		foreach (var t in newVisibleTargets.Where(t => !visibleTargets.Contains(t)))
		{
			controller.OnFoundVisibleTarget(t);
		}

        visibleTargets = new List<Transform>(newVisibleTargets);

		if (visibleTargets.Count == 0)
        {
			controller.OnLostAllTargets();
        }
	}

	private void DrawFieldOfView() {
		_line.positionCount = 0;
		var stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
		var stepAngleSize = viewAngle / stepCount;
		var viewPoints = new List<Vector3> ();
		var oldViewCast = new ViewCastInfo ();
		for (var i = 0; i <= stepCount; i++) {
			var angle = _viewCenter() - viewAngle / 2 + stepAngleSize * i;
			var newViewCast = ViewCast (angle);

			if (i > 0) {
				var edgeDstThresholdExceeded = Mathf.Abs (oldViewCast.Dst - newViewCast.Dst) > edgeDstThreshold;
				if (oldViewCast.Hit != newViewCast.Hit || (oldViewCast.Hit && newViewCast.Hit && edgeDstThresholdExceeded)) {
					var edge = FindEdge (oldViewCast, newViewCast);
					if (edge.PointA != Vector3.zero) {
						viewPoints.Add (edge.PointA);
					}
					if (edge.PointB != Vector3.zero) {
						viewPoints.Add (edge.PointB);
					}
				}

			}


			viewPoints.Add (newViewCast.Point);
			oldViewCast = newViewCast;
		}

		var vertexCount = viewPoints.Count + 1;
		var vertices = new Vector3[vertexCount];
		var triangles = new int[(vertexCount-2) * 3];

		vertices [0] = eye.localPosition;
		for (var i = 0; i < vertexCount - 1; i++) {
			vertices [i + 1] = transform.InverseTransformPoint(viewPoints [i]);

			if (i < vertexCount - 2) {
				triangles [i * 3] = 0;
				triangles [i * 3 + 1] = i + 1;
				triangles [i * 3 + 2] = i + 2;
			}
		}

		_viewMesh.Clear ();

		_viewMesh.vertices = vertices;
		_viewMesh.triangles = triangles;
		_viewMesh.RecalculateNormals ();
	}

	private void DrawLocked()
	{
		_viewMesh.Clear ();
		if (_line == null) return;
		_line.positionCount = 2;
		_line.SetPositions(new [] {eye.transform.position, visibleTargets[0].position});
	}

	private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast) {
		var minAngle = minViewCast.Angle;
		var maxAngle = maxViewCast.Angle;
		var minPoint = Vector3.zero;
		var maxPoint = Vector3.zero;

		for (var i = 0; i < edgeResolveIterations; i++) {
			var angle = (minAngle + maxAngle) / 2;
			var newViewCast = ViewCast (angle);

			var edgeDstThresholdExceeded = Mathf.Abs (minViewCast.Dst - newViewCast.Dst) > edgeDstThreshold;
			if (newViewCast.Hit == minViewCast.Hit && !edgeDstThresholdExceeded) {
				minAngle = angle;
				minPoint = newViewCast.Point;
			} else {
				maxAngle = angle;
				maxPoint = newViewCast.Point;
			}
		}

		return new EdgeInfo (minPoint, maxPoint);
	}


	private ViewCastInfo ViewCast(float globalAngle) {
		var dir = DirFromAngle (globalAngle, true);
		RaycastHit hit;

		if (Physics.Raycast (eye.position, dir, out hit, viewRadius, obstacleMask)) {
			return new ViewCastInfo (true, hit.point, hit.distance, globalAngle);
		}

		return new ViewCastInfo (false, eye.position + dir * viewRadius, viewRadius, globalAngle);
	}

	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
		if (!angleIsGlobal) {
			angleInDegrees += _viewCenter();
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),Mathf.Cos(angleInDegrees * Mathf.Deg2Rad),0);
	}

	public struct ViewCastInfo {
		public bool Hit;
		public Vector3 Point;
		public float Dst;
		public float Angle;

		public ViewCastInfo(bool hit, Vector3 point, float dst, float angle) {
			Hit = hit;
			Point = point;
			Dst = dst;
			Angle = angle;
		}
	}

	public struct EdgeInfo {
		public Vector3 PointA;
		public Vector3 PointB;

		public EdgeInfo(Vector3 pointA, Vector3 pointB) {
			PointA = pointA;
			PointB = pointB;
		}
	}

	private float LocalHorizontal() => eye.eulerAngles.y;

	private Func<float> ToTarget(Transform target)
    {
		var dirToTarget = (target.position - eye.position).normalized;
		var angleSign = dirToTarget.y > 0 ? 1 : -1;
		return () => angleSign * Vector3.Angle(dirToTarget, Vector3.left) -90;
    }

	public IEnumerator LockOn(Transform target)
    {
		while (true)
        {
			_viewCenter = ToTarget(target);
			yield return new WaitForEndOfFrame();
		}
	}

	public void LockOff()
    {
		StopCoroutine("LockOn");
		_viewCenter = LocalHorizontal;
    }

	public void AddNoneLost(Transform target)
	{
		if (_noneLostTargets.Contains(target)) return;
		_noneLostTargets.Add(target);
	}
}
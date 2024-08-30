using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace FromStore.Arrow_WayPointer.Scripts
{
	[ExecuteInEditMode]
	public class WaypointController : MonoBehaviour
	{

		public enum Switch { Off, On }

		[Serializable]
		public class WaypointComponents
		{
			public string waypointName = "Waypoint Name";
			public Waypoint waypoint;
			public UnityEvent waypointEvent;
		}


		public Transform player;
		public Switch configureMode;
		[Range(0.0001f, 20)] public float arrowTargetSmooth; // controls how fast the arrow should smoothly target the next waypoint
		[FormerlySerializedAs("TotalWaypoints")][Range(1, 100)] public int totalWaypoints; // controls how many Waypoints should be used
		public WaypointComponents[] waypointList;
		private GameObject _newWaypoint;
		private string _newWaypointName;
		private int _nextWp;
		private Transform _waypointArrow; //Transform used to reference the Waypoint Arrow
		private Transform _currentWaypoint; //Transforms used to identify the Waypoint Arrow's target
		private Transform _arrowTarget;

		private void Start()
		{
			if (Application.isPlaying)
			{
				var newObject = new GameObject();
				newObject.name = "Arrow Target";
				newObject.transform.parent = gameObject.transform;
				_arrowTarget = newObject.transform;
				newObject = null;
			}
			_nextWp = 0;
			ChangeTarget();
		}

		[ContextMenu("Reset")]
		public void Reset()
		{
			_nextWp = 0;
			ChangeTarget();
		}

		private void Update()
		{
			if (configureMode == Switch.Off)
			{
				totalWaypoints = waypointList.Length;
			}
			//Check if the script is being executed in the Unity Editor
#if UNITY_EDITOR
			if (configureMode == Switch.On)
			{
				CalculateWaypoints();
			}
#endif
			//Keep the Waypoint Arrow pointed at the Current Waypoint
			if (_arrowTarget != null)
			{
				_arrowTarget.localPosition = Vector3.Lerp(_arrowTarget.localPosition, _currentWaypoint.localPosition, arrowTargetSmooth * Time.deltaTime);
				_arrowTarget.localRotation = Quaternion.Lerp(_arrowTarget.localRotation, _currentWaypoint.localRotation, arrowTargetSmooth * Time.deltaTime);
			}
			else
			{
				_arrowTarget = _currentWaypoint;
			}
			if (_waypointArrow == null)
				FindArrow();
			_waypointArrow.LookAt(_arrowTarget);
		}

		public void WaypointEvent(int waypointEvent)
		{
			waypointList[waypointEvent - 1].waypointEvent.Invoke();
		}

		public void ChangeTarget()
		{
			var check = _nextWp;
			if (check < totalWaypoints)
			{
				if (_currentWaypoint == null)
					_currentWaypoint = waypointList[0].waypoint.transform;
				_currentWaypoint.gameObject.SetActive(false);
				_currentWaypoint = waypointList[_nextWp].waypoint.transform;
				_currentWaypoint.gameObject.SetActive(true);
				_nextWp += 1;
			}
			if (check == totalWaypoints)
			{
				Destroy(_waypointArrow.gameObject);
				Destroy(gameObject);
			}
		}

		public void CreateArrow()
		{
			var instance = Instantiate(Resources.Load("Waypoint Arrow", typeof(GameObject))) as GameObject;
			instance.name = "Waypoint Arrow";
			instance = null;
		}

		public void FindArrow()
		{
			var arrow = GameObject.Find("Waypoint Arrow");
			if (arrow == null)
			{
				CreateArrow();
				_waypointArrow = GameObject.Find("Waypoint Arrow").transform;
			}
			else
			{
				_waypointArrow = arrow.transform;
			}
		}

		public void CalculateWaypoints()
		{
			if (configureMode == Switch.On)
			{
				Array.Resize(ref waypointList, totalWaypoints);
				if (_waypointArrow == null) FindArrow();
				for (var i = 0; i < totalWaypoints; i++)
				{
					if (waypointList[i] != null && waypointList[i].waypoint == null)
					{
						_newWaypointName = "Waypoint " + (i + 1);
						waypointList[i].waypointName = _newWaypointName;
						//setup waypoint reference
						foreach (Transform child in transform)
						{
							if (child.name == _newWaypointName) { waypointList[i].waypoint = child.GetComponent<Waypoint>(); }
						}
						if (waypointList[i].waypoint == null)
						{
							_newWaypoint = Instantiate(Resources.Load<GameObject>("Waypoint"));
							_newWaypoint.name = _newWaypointName;
							_newWaypoint.GetComponent<Waypoint>().waypointNumber = i + 1;
							_newWaypoint.transform.parent = gameObject.transform;
							waypointList[i].waypoint = _newWaypoint.GetComponent<Waypoint>();
							waypointList[i].waypoint.waypointController = this;
							Debug.Log("Waypoint Controller created a new Waypoint: " + _newWaypointName);
						}
						_currentWaypoint = waypointList[0].waypoint.transform;
					}
				}
				CleanUpWaypoints();
			}
		}

		public void CleanUpWaypoints()
		{
			if (configureMode == Switch.On)
			{
				if (Application.isPlaying)
				{
					Debug.LogWarning("ARROW WAYPOINTER: Turn Off 'Configure Mode' on the Waypoint Controller");
				}
				if (transform.childCount > waypointList.Length)
				{
					foreach (Transform oldChild in transform)
					{
						if (oldChild.GetComponent<Waypoint>().waypointNumber > waypointList.Length)
						{
							DestroyImmediate(oldChild.gameObject);
						}
					}
				}
			}
		}

#if UNITY_EDITOR
		//Draws a Gizmo in the scene view window to show the Waypoints
		public void OnDrawGizmosSelected()
		{
			for (var i = 0; i < waypointList.Length; i++)
			{
				if (waypointList[i] != null)
				{
					if (waypointList[i].waypoint != null)
					{
						Gizmos.DrawWireSphere(waypointList[i].waypoint.transform.position, waypointList[i].waypoint.radius);
					}
				}
			}
		}
#endif

	}
}
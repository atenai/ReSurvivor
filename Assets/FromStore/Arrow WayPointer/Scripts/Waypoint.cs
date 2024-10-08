﻿using UnityEngine;

namespace FromStore.Arrow_WayPointer.Scripts
{
	public class Waypoint : MonoBehaviour
	{

		public int radius;
		[HideInInspector] public WaypointController waypointController;
		[HideInInspector] public int waypointNumber;

		private void Update()
		{
			if (waypointController.player)
			{
				if (Vector3.Distance(transform.position, waypointController.player.position) < radius)
				{
					waypointController.ChangeTarget();
				}
			}
		}

		private void OnTriggerEnter(Collider col)
		{
			if (col.gameObject.tag == "Player")
			{
				waypointController.WaypointEvent(waypointNumber);
				waypointController.ChangeTarget();
			}
		}

#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			if (waypointController != null) waypointController.OnDrawGizmosSelected();
		}
#endif
	}
}
using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor (typeof (FieldOfView))]
	public class FieldOfViewEditor : UnityEditor.Editor {
		private void OnSceneGUI() {
			var fow = (FieldOfView)target;
			Handles.color = Color.white;
			Handles.DrawWireArc (fow.eye.position, Vector3.forward, Vector3.up, 360, fow.viewRadius);
			var viewAngleA = fow.DirFromAngle (-fow.viewAngle / 2, false);
			var viewAngleB = fow.DirFromAngle (fow.viewAngle / 2, false);

			Handles.DrawLine (fow.eye.position, fow.eye.position + viewAngleA * fow.viewRadius);
			Handles.DrawLine (fow.eye.position, fow.eye.position + viewAngleB * fow.viewRadius);

			Handles.color = Color.red;
			foreach (var visibleTarget in fow.visibleTargets) {
				Handles.DrawLine (fow.eye.position, visibleTarget.position);
			}
		}

	}
}

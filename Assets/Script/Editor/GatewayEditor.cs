using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(Gateway))]
	public class GatewayEditor : UnityEditor.Editor
	{
		private void OnSceneGUI()
		{
			var gateway = (Gateway)target;
			Handles.color = Color.green;
			var p1 = gateway.transform.position + new Vector3(gateway.accessRangeLower, 0, 0);
			var p2 = gateway.transform.position + new Vector3(gateway.accessRangeUpper, 0, 0);

			Handles.DrawLine(p1, p2);
		}

	}
}

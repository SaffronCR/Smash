using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Smash
{
	public class PlatformCollision : MonoBehaviour
	{
		public float maxAngle = 45.0f;

		private void Start()
		{
			float cos = Mathf.Cos(maxAngle);
			MeshFilter MF = GetComponent<MeshFilter>();
			MeshCollider MC = GetComponent<MeshCollider>();
			if (MF == null || MC == null || MF.sharedMesh == null)
			{
				Debug.LogError("PlatformCollision needs a MeshFilter and a MeshCollider");
				return;
			}
			Mesh M = new Mesh();
			Vector3[] verts = MF.sharedMesh.vertices;
			List<int> triangles = new List<int>(MF.sharedMesh.triangles);
			for (int i = triangles.Count - 1; i >= 0; i -= 3)
			{
				Vector3 P1 = transform.TransformPoint(verts[triangles[i - 2]]);
				Vector3 P2 = transform.TransformPoint(verts[triangles[i - 1]]);
				Vector3 P3 = transform.TransformPoint(verts[triangles[i]]);
				Vector3 faceNormal = Vector3.Cross(P3 - P2, P1 - P2).normalized;
				if (Vector3.Dot(faceNormal, Vector3.up) <= cos)
				{
					triangles.RemoveAt(i);
					triangles.RemoveAt(i - 1);
					triangles.RemoveAt(i - 2);
				}
			}
			M.vertices = verts;
			M.triangles = triangles.ToArray();
			MC.sharedMesh = M;
		}
	}
}

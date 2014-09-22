using UnityEngine;
using System.Collections;

public class FloorScript : MonoBehaviour {
	Mesh floorMesh;
	public GameObject pelletPrefab;
	// Use this for initialization
	void Start () {
		floorMesh = gameObject.GetComponent<MeshFilter> ().mesh;

		for (int i = 0; i < floorMesh.triangles.Length; i += 3)
		{
			Vector3 v1 = floorMesh.vertices[floorMesh.triangles[i + 0]];
			Vector3 v2 = floorMesh.vertices[floorMesh.triangles[i + 1]];
			Vector3 v3 = floorMesh.vertices[floorMesh.triangles[i + 2]];
			Vector3 pos;
			if(Vector3.Dot ((v2-v1),(v3-v1)) != 0)
			{
				if((v2-v1).magnitude > (v3-v1).magnitude)
				{
					pos = v1 +((v2-v1)/2);
				}
				else
				{
				 	pos = v1 +((v3-v1)/2);
				}


			}
			else
			{
				pos = v1 +((v3-v1)/2) + ((v2-v1)/2);
			}
			pos.x *= transform.localScale.x;
			pos.y *= transform.localScale.y;
			pos.z = pos.y;
			pos.y = 1;
			Instantiate(pelletPrefab, pos, Quaternion.identity);

		}
	}
	
}

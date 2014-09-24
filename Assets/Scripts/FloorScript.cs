using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FloorScript : MonoBehaviour {
	Mesh floorMesh;
	public GameObject pelletPrefab;
    private GameController gameController;
    private Dictionary<Vector3, bool> positions;
	// Use this for initialization
	void Start () {
        positions = new Dictionary<Vector3, bool>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        SetupPellets();

	}
   
    int ResetMap()
    {
        GameObject[] pellets = GameObject.FindGameObjectsWithTag("Pellet");
        foreach (GameObject p in pellets)
        {
            Destroy(p);
        }
        return SetupPellets();
    }
    int SetupPellets()
    {
        floorMesh = gameObject.GetComponent<MeshFilter> ().mesh;
        int numPellets = 0;
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
            pos.x += transform.localPosition.x;
            pos.y *= transform.localScale.y;
            pos.y += transform.localPosition.y;
            pos.z = pos.y;
            pos.y = 1;

            // check if we have already created a pellet at this position.
            if (positions.ContainsKey(pos))
            {
                // a pellet has has been instatiated aldready on this position, so skip this
                continue;
            }
            positions.Add(pos, true);

            Instantiate(pelletPrefab, pos, Quaternion.identity);
            if(gameController != null)
            {
                gameController.IncrementPelletCounter();
            }
            numPellets++;
            
        }
        return numPellets;
    }

}

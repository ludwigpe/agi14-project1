using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class is responsible for instatiating pellets on each square on
/// the floor.
/// </summary>
public class FloorScript : MonoBehaviour {

	Mesh floorMesh;
	public GameObject pelletPrefab;
    private GameController gameController;
    private Dictionary<Vector3, bool> positions;
	
    /// <summary>
    /// Use this for initialization
    /// </summary>
	void Start () 
    {
        positions = new Dictionary<Vector3, bool>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        SetupPellets();
	}
   
    /// <summary>
    /// Retrieve all pellet objects instatiated in the scene and then
    /// destroy them. Once all are destroyed we instatiate new pellets 
    /// with setupPellets.
    /// </summary>
    /// <returns>Number of pellets created </returns>
    int ResetMap()
    {
        GameObject[] pellets = GameObject.FindGameObjectsWithTag("Pellet");
        foreach (GameObject p in pellets)
        {
            Destroy(p);
        }
        return SetupPellets();
    }

    /// <summary>
    /// This function goes trough all triangles in the floor mesh and finds the hypotenuse
    /// where it instatiates a new pellet if there does not already exist one there. After
    /// a pellet has been instatiated the position is saved for future reference.
    /// </summary>
    /// <returns> number of pellets instatiated </returns>
    int SetupPellets()
    {
        floorMesh = gameObject.GetComponent<MeshFilter> ().mesh;
        int numPellets = 0;

        // loop through all triangles in the floor mesh
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
                // v1 is the vertex opposite the hypotenuse
                // calculate the middle position of the hypotenuse.
                pos = v1 +((v3-v1)/2) + ((v2-v1)/2);
            }

            // scale the position according to the parents scale since the floor is in local space.
            pos.x *= transform.parent.localScale.x;
            pos.x += transform.localPosition.x;
            pos.y *= transform.parent.localScale.y;
            pos.y += transform.localPosition.y;
            pos.z = pos.y;
            pos.y = 1;

            // check if we have already created a pellet at this position.
            if (positions.ContainsKey(pos))
            {
                // a pellet has has been instatiated aldready on this position, so skip this
                continue;
            }

            // save the position calculated for future reference.
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

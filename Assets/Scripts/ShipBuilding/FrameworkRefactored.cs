using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameworkRefactored : MonoBehaviour
{
    [Header("Framework")]
    int cubeCount = 1;
    public int gridDepth;
    public int gridHeight;
    public int gridWidth;
    public List<Vector3> Vertices;
    public Transform[] ControlPoints;
    public GameObject ControlPointPrefab;
    GameObject container;

    [Space()]
    public bool autoUpdate = false;



    void ClearAllData() {
        Vertices.Clear();
        for(int a = 0; a < ControlPoints.Length; a++) {
            DestroyImmediate(ControlPoints[a].gameObject);
        }
    }

    public void UpdateInEditor() {
        ClearAllData();
        //Create container for ControlPoints
        if(container == null) {
            container = new GameObject("ROOT || ControlPoints");
            container.transform.parent = this.transform;
        }
        //Create 3D Vertex list where TransformPoints are places.
        for(int x = 0; x < gridDepth; x++) {
            for(int y = 0; y < gridHeight; y++) {
                for(int z = 0; z < gridWidth; z++) {
                    Vertices.Add(new Vector3(x, y, z));
                }
            }
        }
        //Instatiate and Index the TransformPoints.
        ControlPoints = new Transform[Vertices.Count];
        for(int i = 0; i < Vertices.Count; i++) {
            if(ControlPoints[i] == null) {
                ControlPoints[i] = (Instantiate(ControlPointPrefab, Vertices[i], Quaternion.identity, container.transform)).transform;
                if(i >= 8){
                    ControlPoints[i].gameObject.SetActive(false);
                }
                ControlPoints[i].GetComponent<ControlPoint>().Index = i;
            }
        }
    }
    
    // void OnDrawGizmos() {
    //     Gizmos.color = Color.white;
    //     for(int i = 0; i < Vertices.Count; i++) {
    //         Gizmos.DrawWireSphere(Vertices[i], 0.02f);
    //     }
    // }


    Vector3Int[] CornerTable = new Vector3Int[8] {
        new Vector3Int(0, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(1, 0, 1),
        new Vector3Int(1, 1, 1),
        new Vector3Int(0, 1, 1)
    };

    int[,] EdgeIndexes = new int[12, 2] {
        {0,1}, {1,2}, {3,2}, {0,3}, {4,5}, {5,6}, {7,6}, {4,7}, {0,4}, {1,5}, {2,6}, {3,7}
    };

}

    
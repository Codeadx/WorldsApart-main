using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewFramework : MonoBehaviour
{
    public Vector3[] TransformPoint;
    public Vector3[] RoundingPoints;
    
    void Start() {
        for(int i = 0; i < TPTable.Length; i++) {
            TransformPoint[i] = TPTable[i];
            RoundingPoints[i] = RPTable[i];
        }
    }

    #region Transform Point positions
    Vector3Int[] TPTable = new Vector3Int[8]
    {
        new Vector3Int(0, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(0, 1, 1),
        new Vector3Int(1, 0, 1),
        new Vector3Int(1, 1, 1)
    };
    #endregion
    
    #region Rounding Point positions
    Vector3[] RPTable = new Vector3[8]
    {
        new Vector3(0, 1/3f, 0),
        new Vector3(0, 1/3f * 2, 0),
        new Vector3(1, 1/3f, 0),
        new Vector3(1, 1/3f * 2, 0),
        new Vector3(0, 1/3f, 1),
        new Vector3(0, 1/3f * 2, 1),
        new Vector3(1, 1/3f, 1),
        new Vector3(1, 1/3f * 2, 1)
    };
    #endregion

    public void OnDrawGizmos() {
        for(int i = 0; i < TransformPoint.Length; i++) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(TransformPoint[i], 0.05f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(RoundingPoints[i], 0.03f);
        }
    }
}

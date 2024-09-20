using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Framework : MonoBehaviour
{
    public delegate CalculatePlus Func<in T1, out CalculatePlus>(T1 CalculatePlus11);
    public Transform[] ControlPoints;
    public Vector3[] InitialPositions;
    public ControlPoint[] InitialArray;
    public Vector3[] newRPos;

    void Awake() {
        InitializeControlPoints();
        InitializeInitialPositions();
    }
#region Control Point initialization
    void InitializeControlPoints() {
        InitialArray = GetComponentsInChildren<ControlPoint>();
        ControlPoints = new Transform[InitialArray.Length];
        GameObject container = new GameObject("ROOT || ControlPoints");
        container.transform.parent = this.transform;

        for (int i = 0; i < InitialArray.Length; i++) {
            ControlPoints[i] = InitialArray[i].transform;
            ControlPoints[i].parent = container.transform;

            if (!InitialArray[i].transform.CompareTag("DuplicationPoint")) {
                InitialArray[i].Index = i;
            }

            if (InitialArray[i].ActiveOnStart == ActiveOnStart.No) {
                InitialArray[i].gameObject.SetActive(false);
            }
        }
    }

    void InitializeInitialPositions() {
        InitialPositions = new Vector3[ControlPoints.Length];
        for (int i = 0; i < ControlPoints.Length; i++) {
            InitialPositions[i] = ControlPoints[i].position;
        }
    }
#endregion

    void UpdateRoundingPointOffsets(int index) {
        if (index >= 18) {  // RoundingPoints indices are assumed to be >= 16
            newRPos[index - 18] = ControlPoints[index].transform.position - FindInitialPosition(index);
        }
    }

    public void MoveSelectedAndAssociated(int index, Axis axis, Vector3 delta) {
        ControlPoint point = ControlPoints[index].GetComponent<ControlPoint>();
        if (point == null) return;

        delta = ClampDelta(delta, index);

        if (point.PointType == PointType.TransformPoint) {
            HandleTransformPoint(index, axis, delta);
        } else if (point.PointType == PointType.RoundingPoint) {
            HandleRoundingPoint(index, axis, delta);
        }
    }

#region Transform and Rounding point logic
    void HandleTransformPoint(int index, Axis axis, Vector3 delta) {
        MoveSingleVertex(index, axis, delta);
        if(LikeTPoints.TryGetKey(index, out var rKey)) {
            //Input Value to return Key.
            Locate1stCubePoints(OpposingPoints, LikeTPoints, rKey);
            if(index == 1 || index == 3) {
                int i = CalculatePlus11(index);
                LocateFurtherCubePoints(OpposingPoints, LikeTPoints, i);
            } else if (index == 5 || index == 7) {
                int i = CalculatePlus8(index);
                LocateFurtherCubePoints(OpposingPoints, LikeTPoints, i);
            }
        } else { 
            //If the Key cannot be found, we put in the Value instead.
            Locate1stCubePoints(OpposingPoints, LikeTPoints, index); 
        }

        //Calculates PartnerIndex and moves the TransformPoint opposite
        int partnerIndex = HorizontalOpposite(index);
        if(index >= 16) {
            partnerIndex = VerticalOpposite(index);
        }

        if(axis == Axis.X) {
            delta = -delta;
        } MoveSingleVertex(partnerIndex, axis, delta);

        if(LikeTPoints.TryGetKey(partnerIndex, out var returnedKey)) {
            //Input Value to return Key
            Locate1stCubePoints(OpposingPoints, LikeTPoints, returnedKey);
            if(index == 1 || index == 3) {
                int i = CalculatePlus11(partnerIndex);
                LocateFurtherCubePoints(OpposingPoints, LikeTPoints, i);
            } else if (index == 5 || index == 7) {
                int i = CalculatePlus8(partnerIndex);
                LocateFurtherCubePoints(OpposingPoints, LikeTPoints, i);
            } else if (index >= 16) {
                LocateFurtherCubePoints(OpposingPoints, LikeTPoints, partnerIndex);
            }
        } else {
            //If the Key cannot be found, we put in the Value instead.
            Locate1stCubePoints(OpposingPoints, LikeTPoints, partnerIndex); 
        }
    }

    void HandleRoundingPoint(int index, Axis axis, Vector3 delta) {
        MoveSingleVertex(index, axis, delta);
        //Move Vertex
        UpdateRoundingPointOffsets(index);
        //Update Offset position
        int partnerIndex = HorizontalOppositePositiveVal(index);
        
        if(index >= 30 && index < 34) {
            partnerIndex = index + 4;
        } else if (index >= 34 && index < 38) {
            partnerIndex = index - 4;
        }
        if(axis == Axis.X) {
            delta = -delta;
        } MoveSingleVertex(partnerIndex, axis, delta); 
        UpdateRoundingPointOffsets(partnerIndex); 
    }

#endregion

    Vector3 ClampDelta(Vector3 delta, int index) {
        Vector3 initialPos = FindInitialPosition(index);
        return new Vector3(
            Mathf.Clamp(delta.x, initialPos.x - 0.45f, initialPos.x + 0.45f),
            Mathf.Clamp(delta.y, initialPos.y - 0.45f, initialPos.y + 0.45f),
            Mathf.Clamp(delta.z, initialPos.z - 0.45f, initialPos.z + 0.45f)
        );
    }
#region Resetting position
    void InitTransformAndRoundingPoint(int index) {
        ControlPoints[index].position = FindInitialPosition(index);
        ControlPoints[OpposingPoints[index]].position = FindInitialPosition(OpposingPoints[index]);
    }

    void InitSingleRoundingPoint(int index) {
        ControlPoints[index].position = FindInitialPosition(index);
    }

    void InitOppositeTransformPoints(int index) {
        // Helper to initialize rounding and transform points for an index and its opposite points
        InitTransformAndRoundingPoint(index);
        InitTransformAndRoundingPoint(HorizontalOpposite(index));
        InitTransformAndRoundingPoint(VerticalOpposite(index));
        int partnerIndex = HorizontalOpposite(VerticalOpposite(index));
        InitTransformAndRoundingPoint(partnerIndex);
    }

    void InitSpecialCase(int index, Func<int, int> calculatePlus) {
        // Generalized helper to handle special cases for plus11 and plus8
        int partnerIndex = HorizontalOpposite(index);
        InitTransformAndRoundingPoint(partnerIndex);
        InitSingleRoundingPoint(OpposingPoints[calculatePlus(partnerIndex)]);
        InitSingleRoundingPoint(OpposingPoints[calculatePlus(partnerIndex)] + 1);

        partnerIndex = VerticalOpposite(partnerIndex);
        InitTransformAndRoundingPoint(partnerIndex);
        InitSingleRoundingPoint(OpposingPoints[calculatePlus(index)]);
        InitSingleRoundingPoint(OpposingPoints[calculatePlus(index)] + 1);
    }

    public void ResetAllPoints(int index, Vector3 initialPos) {
        // Resets all points based on index
        if (index >= 12) {
            InitTransformAndRoundingPoint(index);
            InitTransformAndRoundingPoint(HorizontalOpposite(index));
            InitSingleRoundingPoint(OpposingPoints[index] + 1);
            InitSingleRoundingPoint(OpposingPoints[HorizontalOpposite(index)] + 1);
        } else if (index == 1 || index == 3 || index == 5 || index == 7) {
            InitTransformAndRoundingPoint(index);
            InitOppositeTransformPoints(index);

            if (index == 1 || index == 3) {
                InitSpecialCase(index, CalculatePlus11);
            } else if (index == 5 || index == 7) {
                InitSpecialCase(index, CalculatePlus8);
            }
        } else {
            InitOppositeTransformPoints(index);
        }
    }
#endregion
    private void LocateCubePointsHelper(Dictionary<int, int> rpoint, Dictionary<int, int> tpoint, int index, bool adjustOffset) {
        // Generalized method to locate cube points (shared between Locate1stCubePoints and LocateFurtherCubePoints)
        Vector3 t1 = ControlPoints[index].transform.localPosition;
        Vector3 t2 = ControlPoints[tpoint[index]].transform.localPosition;
        Vector3 pos1 = t1 + (t2 - t1) / 3;
        Vector3 pos2 = t1 + (t2 - t1) * 2 / 3;

        // Calculate and clamp position for the first rounding point
        if (adjustOffset) {
            Vector3 r1offset = newRPos[rpoint[index] - 18];
            ControlPoints[rpoint[index]].transform.position = ClampDelta(r1offset + pos1, rpoint[index]);
        } else {
            ControlPoints[rpoint[index]].transform.position = pos1;
        }

        // Calculate and clamp position for the second rounding point
        if (adjustOffset) {
            Vector3 r2offset = newRPos[rpoint[index] - 18];
            ControlPoints[rpoint[index] + 1].transform.position = ClampDelta(r2offset + pos2, rpoint[index] + 1);
        } else {
            ControlPoints[rpoint[index] + 1].transform.position = pos2;
        }
    }

#region Point location
    public void Locate1stCubePoints(Dictionary<int, int> rpoint, Dictionary<int, int> tpoint, int index) {
        LocateCubePointsHelper(rpoint, tpoint, index, adjustOffset: true);
    }

    public void LocateFurtherCubePoints(Dictionary<int, int> rpoint, Dictionary<int, int> tpoint, int index) {
        LocateCubePointsHelper(rpoint, tpoint, index, adjustOffset: true);
    }
#endregion

    public void MoveSingleVertex(int index, Axis axis, Vector3 delta) {
        Vector3 pos = ControlPoints[index].transform.position;

        if (Input.GetKey(KeyCode.LeftShift)) {
            // Round the delta to 1 decimal place
            delta.x = Mathf.Round(delta.x * 10f) / 10f;
            delta.y = Mathf.Round(delta.y * 10f) / 10f;
            delta.z = Mathf.Round(delta.z * 10f) / 10f;
        }

        switch (axis) {
            case Axis.X: pos.x = delta.x; break;
            case Axis.Y: pos.y = delta.y; break;
            case Axis.Z: pos.z = delta.z; break;
        }

        ControlPoints[index].transform.position = pos;
    }

    public Vector3 FindInitialPosition(int index) {
        return InitialPositions[index];
    }
        
    public int VerticalOpposite(int index) => index + 1 * ((index / 1) % 2 == 0 ? 1 : -1);
    public int HorizontalOpposite(int index) => index + 2 * ((index / 2) % 2 == 0 ? 1 : -1);    
    public int HorizontalOppositePositiveVal(int index) => index + 2 * ((index / 2) % 2 == 0 ? -1 : 1);
    public int CalculatePlus8(int index) => index + 8 * ((index / 8) % 2 == 0 ? 1 : -1);
    public int CalculatePlus11(int index) => index + 11 * ((index / 11) % 2 == 0 ? 1 : -1);


#region Dictionaries
    public Dictionary<int, int> LikeRPoints = new Dictionary<int, int> {
        {18, 19}, {20, 21}, {22, 23}, {24, 25}, {26, 27}, {28, 29}, {30, 31}, {32, 33}, {34, 35}, {36, 37}, {38, 39}, {40, 41}
    };

    public Dictionary<int, int> LikeTPoints = new Dictionary<int, int> {
        {0, 1}, {2, 3}, {4, 5}, {6, 7}, {8, 9}, {10, 11}, {12, 1}, {13, 5}, {14, 3}, {15, 7}, {16, 9}, {17, 11}
    };

    public Dictionary<int, int> OpposingPoints = new Dictionary<int, int> {
        {0, 18}, {1, 19}, {2, 20}, {3, 21}, {4, 22}, {5, 23}, {6, 24}, {7, 25}, {8, 26}, {9, 27}, {10, 28}, {11, 29}, 
        {12, 30}, {31, 1}, {13, 32}, {33, 5}, {14, 34}, {35, 3}, {15, 36}, {37, 7}, {16, 38}, {17, 40}, 
    };
    #endregion
}

    public enum Axis {
        X,
        Y,
        Z
    }

    public enum ActiveOnStart {
        Yes,
        No
    }

    public enum PointType {
        TransformPoint,
        RoundingPoint
    }

    
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoxColliders : MonoBehaviour
{
    Framework Framework;

    public BoxCollider[] AllColliders = new BoxCollider[20];
    BoxCollider BoxCollider;
    public GameObject[] ColliderContainer = new GameObject[20];
    public Material material;



    [Header("Parameters")]
    public Mesh mesh;
    public Vector3 size;

    // Start is called before the first frame update
    void Awake() {
        Framework = GetComponent<Framework>();
        GameObject container = new GameObject("ROOT || InitialColliders");
        container.gameObject.transform.parent = Framework.transform;
        for(int i = 0; i < 20; i++) {
            ColliderContainer[i] = new GameObject("ColliderContainer"+i, typeof(BoxCollider), typeof(MeshFilter), typeof(MeshRenderer));
            ColliderContainer[i].GetComponent<MeshFilter>().mesh = mesh;
            ColliderContainer[i].GetComponent<MeshRenderer>().material = material;
            ColliderContainer[i].gameObject.transform.parent = container.transform;
            AllColliders[i] = ColliderContainer[i].GetComponent<BoxCollider>();
        }
        UpdateCube();
    }

    // Update is called once per frame
    void LateUpdate() {
        UpdateCube();
    }

    public void UpdateCube() {
        for(int i = 0; i < 4; i++) {
        LikeRPointColliders(i, ColliderContainer);
        } for(int j = 4; j < 8; j++) {
            XAxisPairColliders(j, ColliderContainer);
        } for(int k = 8; k < 12; k++) {
            ZAxisPairColliders(k, ColliderContainer);
        } for(int l = 12; l < 20; l++) {
            OpposingPointColliders(l, ColliderContainer);
        }
    }

    private GameObject[] LikeRPointColliders(int index, GameObject[] Colliders) {
        Colliders[index] = ColliderContainer[index];
        //ElementAt(index) gets the Key/Value at the for loop index
        Vector3 positions = (Framework.ControlPoints[Framework.LikeRPoints.ElementAt(index).Key].transform.position - Framework.ControlPoints[Framework.LikeRPoints.ElementAt(index).Value].transform.position) / 2 + Framework.ControlPoints[Framework.LikeRPoints.ElementAt(index).Value].transform.position;
        ColliderContainer[index].transform.position = positions;
        Vector3 rotations = Framework.ControlPoints[Framework.LikeRPoints.ElementAt(index).Key].transform.position - Framework.ControlPoints[Framework.LikeRPoints.ElementAt(index).Value].transform.position;
        ColliderContainer[index].transform.rotation = Quaternion.LookRotation(rotations);
        size = new Vector3(0.03f, 0.03f, rotations.magnitude);
        ColliderContainer[index].transform.localScale = size;
        return Colliders;
    }

    private GameObject[] XAxisPairColliders(int index, GameObject[] Colliders) {
        Colliders[index] = ColliderContainer[index];
        //ElementAt(index) gets the Key/Value at the for loop index
        Vector3 positions = (Framework.ControlPoints[XAxisPairs.ElementAt(index-4).Key].transform.position - Framework.ControlPoints[XAxisPairs.ElementAt(index-4).Value].transform.position) / 2 + Framework.ControlPoints[XAxisPairs.ElementAt(index-4).Value].transform.position;
        ColliderContainer[index].transform.position = positions;
        Vector3 rotations = Framework.ControlPoints[XAxisPairs.ElementAt(index-4).Key].transform.position - Framework.ControlPoints[XAxisPairs.ElementAt(index-4).Value].transform.position;
        ColliderContainer[index].transform.rotation = Quaternion.LookRotation(rotations);
        size = new Vector3(0.03f, 0.03f, rotations.magnitude);
        ColliderContainer[index].transform.localScale = size;
        return Colliders;
    }

    private GameObject[] ZAxisPairColliders(int index, GameObject[] Colliders) {
        Colliders[index] = ColliderContainer[index];
        //ElementAt(index) gets the Key/Value at the for loop index
        Vector3 positions = (Framework.ControlPoints[ZAxisPairs.ElementAt(index-8).Key].transform.position - Framework.ControlPoints[ZAxisPairs.ElementAt(index-8).Value].transform.position) / 2 + Framework.ControlPoints[ZAxisPairs.ElementAt(index-8).Value].transform.position;
        ColliderContainer[index].transform.position = positions;
        Vector3 rotations = Framework.ControlPoints[ZAxisPairs.ElementAt(index-8).Key].transform.position - Framework.ControlPoints[ZAxisPairs.ElementAt(index-8).Value].transform.position;
        ColliderContainer[index].transform.rotation = Quaternion.LookRotation(rotations);
        size = new Vector3(0.03f, 0.03f, rotations.magnitude);
        ColliderContainer[index].transform.localScale = size;
        return Colliders;
    }

    private GameObject[] OpposingPointColliders(int index, GameObject[] Colliders) {
        Colliders[index] = ColliderContainer[index];
        //ElementAt(index) gets the Key/Value at the for loop index
        Vector3 positions = (Framework.ControlPoints[Framework.OpposingPoints.ElementAt(index-12).Key].transform.position - Framework.ControlPoints[Framework.OpposingPoints.ElementAt(index-12).Value].transform.position) / 2 + Framework.ControlPoints[Framework.OpposingPoints.ElementAt(index-12).Value].transform.position;
        ColliderContainer[index].transform.position = positions;
        Vector3 rotations = Framework.ControlPoints[Framework.OpposingPoints.ElementAt(index-12).Key].transform.position - Framework.ControlPoints[Framework.OpposingPoints.ElementAt(index-12).Value].transform.position;
        ColliderContainer[index].transform.rotation = Quaternion.LookRotation(rotations);
        size = new Vector3(0.03f, 0.03f, rotations.magnitude);
        ColliderContainer[index].transform.localScale = size;
        return Colliders;
    }

    private Dictionary<int, int> XAxisPairs = new Dictionary<int, int>() {
        {0, 2},
        {1, 3},
        {4, 6},
        {5, 7},
        {8, 10},
        {9, 11},
    };

    private Dictionary<int, int> ZAxisPairs = new Dictionary<int, int>() {
        {0, 4},
        {1, 5},
        {2, 6},
        {3, 7},
        {8, 4},
        {9, 5},
        {10, 6},
        {11, 7}, 
    };
    
}

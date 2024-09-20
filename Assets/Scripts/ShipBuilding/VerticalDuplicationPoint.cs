using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VerticalDuplicationPoint : MonoBehaviour
{
    public int Index;
    
    public ControlPoint[] AssociatedPoints;

    public Framework Framework;
    public BoxCollider[] AllColliders;
    BoxCollider BoxCollider;
    public GameObject[] ColliderContainer;
    public RaycastToControlPoint Raycaster;
    public GameObject AttachedPoint;
    
    [Header("Parameters")]
    public Vector3 size;
    public Mesh mesh;
    
    void Awake() {
        Framework = GetComponentInParent<Framework>();

        AllColliders = new BoxCollider[AssociatedPoints.Length + 4];
        ColliderContainer = new GameObject[AssociatedPoints.Length + 4];
        GameObject container = new GameObject("ROOT || AdditionalColliders");
        container.gameObject.transform.parent = Framework.transform;
        for(int i = 0; i < AssociatedPoints.Length + 4; i++) {
            ColliderContainer[i] = new GameObject("ColliderContainer"+i, typeof(BoxCollider), typeof(MeshFilter), typeof(MeshRenderer));
            ColliderContainer[i].gameObject.transform.parent = container.transform;
            AllColliders[i] = ColliderContainer[i].GetComponent<BoxCollider>();
            AllColliders[i].GetComponent<MeshFilter>().mesh = mesh;
            AllColliders[i].GetComponent<MeshRenderer>().enabled = false;
            AllColliders[i].enabled = !AllColliders[i].enabled; 
        }
    }

    void Update() {
        UpdateCube();
        if(this.GetComponent<MeshRenderer>().enabled == false) {
            for(int m = 0; m < AssociatedPoints.Length + 4; m++) {
                AllColliders[m].enabled = true;
                AllColliders[m].GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }

    private void UpdateCube() {
        for(int i = 0; i < 4; i++) {
            LikeRPointColliders(i, ColliderContainer);
        } for(int j = 4; j < 6; j++) {
            XAxisPairColliders(j, ColliderContainer);
        } for(int k = 0; k < 2; k++) {
            ZAxisPairColliders(k, ColliderContainer);
        } for(int l = 0; l < 8; l++) {
             OpposingPointColliders(l, ColliderContainer);
        }
    }

    private GameObject[] LikeRPointColliders(int index, GameObject[] Colliders) {
        Colliders[index] = ColliderContainer[index];
        int partnerIndex = Framework.VerticalOpposite(index * 2);
        //ElementAt(index) gets the Key/Value at the for loop index
        Vector3 positions = (AssociatedPoints[index * 2].transform.position - AssociatedPoints[partnerIndex].transform.position) / 2 + AssociatedPoints[partnerIndex].transform.position;
        ColliderContainer[index].transform.position = positions;
        Vector3 rotations = AssociatedPoints[index * 2].transform.position - AssociatedPoints[partnerIndex].transform.position;
        ColliderContainer[index].transform.rotation = Quaternion.LookRotation(rotations);
        size = new Vector3(0.03f, 0.03f, rotations.magnitude);
        ColliderContainer[index].transform.localScale = size;
        return Colliders;
    }

    private GameObject[] XAxisPairColliders(int index, GameObject[] Colliders) {
        Colliders[index] = ColliderContainer[index];
        int partnerIndex = Framework.HorizontalOpposite(index + 4);
        //ElementAt(index) gets the Key/Value at the for loop index
        Vector3 positions = (AssociatedPoints[index + 4].transform.position - AssociatedPoints[partnerIndex].transform.position) / 2 + AssociatedPoints[partnerIndex].transform.position;
        ColliderContainer[index].transform.position = positions;
        Vector3 rotations = AssociatedPoints[index + 4].transform.position - AssociatedPoints[partnerIndex].transform.position;
        ColliderContainer[index].transform.rotation = Quaternion.LookRotation(rotations);
        size = new Vector3(0.03f, 0.03f, rotations.magnitude);
        ColliderContainer[index].transform.localScale = size;
        return Colliders;
    }

    private GameObject[] ZAxisPairColliders(int index, GameObject[] Colliders) {
        Colliders[index + 6] = ColliderContainer[index + 6];
        int partnerIndex = Framework.VerticalOpposite(index * 2 + 8);
        //ElementAt(index) gets the Key/Value at the for loop index
        Vector3 positions = (AssociatedPoints[index * 2 + 8].transform.position - AssociatedPoints[partnerIndex].transform.position) / 2 + AssociatedPoints[partnerIndex].transform.position;
        ColliderContainer[index + 6].transform.position = positions;
        Vector3 rotations = AssociatedPoints[index * 2 + 8].transform.position - AssociatedPoints[partnerIndex].transform.position;
        ColliderContainer[index + 6].transform.rotation = Quaternion.LookRotation(rotations);
        size = new Vector3(0.03f, 0.03f, rotations.magnitude);
        ColliderContainer[index + 6].transform.localScale = size;
        return Colliders;
    }

    private GameObject[] OpposingPointColliders(int index, GameObject[] Colliders) {
        Colliders[index + 8] = ColliderContainer[index + 8];
        //ElementAt(index) gets the Key/Value at the for loop index
        Vector3 positions = (Framework.ControlPoints[OpposingPoints.ElementAt(index).Key].transform.position - Framework.ControlPoints[OpposingPoints.ElementAt(index).Value].transform.position) / 2 + Framework.ControlPoints[OpposingPoints.ElementAt(index).Value].transform.position;
        ColliderContainer[index + 8].transform.position = positions;
        Vector3 rotations = Framework.ControlPoints[OpposingPoints.ElementAt(index).Key].transform.position - Framework.ControlPoints[OpposingPoints.ElementAt(index).Value].transform.position;
        ColliderContainer[index + 8].transform.rotation = Quaternion.LookRotation(rotations);
        size = new Vector3(0.03f, 0.03f, rotations.magnitude);
        ColliderContainer[index + 8].transform.localScale = size;
        return Colliders;
    }

    Dictionary<int, int> OpposingPoints = new Dictionary<int, int>() {
        {1, 31},
        {3, 35},
        {5, 33},
        {7, 37},
        {12, 30},
        {13, 32},
        {14, 34},
        {15, 36},
    };
}

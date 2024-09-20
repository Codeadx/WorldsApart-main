using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FinalDuplicationPoint : MonoBehaviour
{
    public int Index;
    
    public ControlPoint[] AssociatedPoints;
    public Framework Framework;
    public BoxCollider[] AllColliders;
    BoxCollider BoxCollider;
    public GameObject[] ColliderContainer;
    public RaycastToControlPoint Raycaster;
    
    [Header("Parameters")]
    public Vector3 size;
    public Mesh mesh;
    
    void Awake() {
        Framework = GetComponentInParent<Framework>();
        
        AllColliders = new BoxCollider[AssociatedPoints.Length + 3];
        ColliderContainer = new GameObject[AssociatedPoints.Length + 3];
        GameObject container = new GameObject("ROOT || AdditionalColliders");
        container.gameObject.transform.parent = Framework.transform;
        for(int i = 0; i < AssociatedPoints.Length + 3; i++) {
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
            for(int m = 0; m < AssociatedPoints.Length + 3; m++) {
                AllColliders[m].enabled = true;
                AllColliders[m].GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }

    private void UpdateCube() {
        for(int i = 0; i < 2; i++) {
            LikeRPointColliders(i, ColliderContainer);
        } for(int j = 2; j < 3; j++) {
            XAxisPairColliders(j, ColliderContainer);
        } for(int k = 3; k < 4; k++) {
            XAxisPairCollider2(k, ColliderContainer);
        } for(int l = 4; l < 5; l++) {
            ZAxisPairCollider(l, ColliderContainer);
        } 
        
        for(int m = 0; m < 4; m++) {
            OpposingPointColliders(m, ColliderContainer);
        }
    }

    private GameObject[] LikeRPointColliders(int index, GameObject[] Colliders) {
        Colliders[index] = ColliderContainer[index];
        int partnerIndex = Framework.VerticalOpposite(index * 2);

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
        int partnerIndex = Framework.VerticalOpposite(index);

        Vector3 positions = (AssociatedPoints[index + 2].transform.position - AssociatedPoints[index + 3].transform.position) / 2 + AssociatedPoints[index + 3].transform.position;
        ColliderContainer[index].transform.position = positions;
        Vector3 rotations = AssociatedPoints[index + 2].transform.position - AssociatedPoints[index + 3].transform.position;
        ColliderContainer[index].transform.rotation = Quaternion.LookRotation(rotations);
        size = new Vector3(0.03f, 0.03f, rotations.magnitude);
        ColliderContainer[index].transform.localScale = size;
        return Colliders;
    }
    
    private GameObject[] XAxisPairCollider2(int index, GameObject[] Colliders) {
        Colliders[index] = ColliderContainer[index];
        int partnerIndex = 15;

        Vector3 positions = (AssociatedPoints[index + 2].transform.position - Framework.ControlPoints[partnerIndex].transform.position) / 2 + Framework.ControlPoints[partnerIndex].transform.position;
        ColliderContainer[index].transform.position = positions;
        Vector3 rotations = AssociatedPoints[index + 2].transform.position - Framework.ControlPoints[partnerIndex].transform.position;
        ColliderContainer[index].transform.rotation = Quaternion.LookRotation(rotations);
        size = new Vector3(0.03f, 0.03f, rotations.magnitude);
        ColliderContainer[index].transform.localScale = size;
        return Colliders;
    }

    private GameObject[] ZAxisPairCollider(int index, GameObject[] Colliders) {
        Colliders[index] = ColliderContainer[index];
        int partnerIndex = 13;

        Vector3 positions = (AssociatedPoints[index].transform.position - Framework.ControlPoints[partnerIndex].transform.position) / 2 + Framework.ControlPoints[partnerIndex].transform.position;
        ColliderContainer[index].transform.position = positions;
        Vector3 rotations = AssociatedPoints[index].transform.position - Framework.ControlPoints[partnerIndex].transform.position;
        ColliderContainer[index].transform.rotation = Quaternion.LookRotation(rotations);
        size = new Vector3(0.03f, 0.03f, rotations.magnitude);
        ColliderContainer[index].transform.localScale = size;
        return Colliders;
    }

    private GameObject[] OpposingPointColliders(int index, GameObject[] Colliders) {
        Colliders[index + 5] = ColliderContainer[index + 5];
        //ElementAt(index) gets the Key/Value at the for loop index
        Vector3 positions = (Framework.ControlPoints[OpposingPoints.ElementAt(index).Key].transform.position - Framework.ControlPoints[OpposingPoints.ElementAt(index).Value].transform.position) / 2 + Framework.ControlPoints[OpposingPoints.ElementAt(index).Value].transform.position;
        ColliderContainer[index + 5].transform.position = positions;
        Vector3 rotations = Framework.ControlPoints[OpposingPoints.ElementAt(index).Key].transform.position - Framework.ControlPoints[OpposingPoints.ElementAt(index).Value].transform.position;
        ColliderContainer[index + 5].transform.rotation = Quaternion.LookRotation(rotations);
        size = new Vector3(0.03f, 0.03f, rotations.magnitude);
        ColliderContainer[index + 5].transform.localScale = size;
        return Colliders;
    }

    Dictionary<int, int> OpposingPoints = new Dictionary<int, int>() {
        {16, 38},
        {17, 40},
        {9, 39},
        {11, 41}
    };
}

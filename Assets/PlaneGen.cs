using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
    
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class PlaneGen : MonoBehaviour
{
    public Framework Framework;
    Mesh planeMesh;
    MeshFilter MeshFilter;
    MeshCollider MeshCollider;
    
    //Plane settings
    [SerializeField] float size = 1;
    [SerializeField] int resolution = 3;

    //Mesh settings
    public List<Vector3> Vertices;
    List<int> Triangles;

    //Deformation settings
    bool done = false;
    Vector3 vertPos;
    List<Vector3> newVertices;
    public CombineInstance[] combine = new CombineInstance[2];

    //Transform
    public Face face;
    public Placement placement;
    public Transform[] FaceControlPoints;

    void Awake() {
        planeMesh = new Mesh();
        MeshFilter = GetComponent<MeshFilter>();
        MeshFilter.mesh = planeMesh;
        MeshCollider = GetComponent<MeshCollider>();
        done = false;
    }
    

    // Update is called once per frame
    void LateUpdate() {
        FaceControlPoints = new Transform[4];

        switch (face) {
            case Face.One:
                switch (placement) {
                    case Placement.Bottom:
                        FaceControlPoints[0] = Framework.ControlPoints[16];
                        FaceControlPoints[1] = Framework.ControlPoints[17];
                        FaceControlPoints[2] = Framework.ControlPoints[18];
                        FaceControlPoints[3] = Framework.ControlPoints[19];
                        //Create Vertices
                        Vertices = new List<Vector3>();
                        foreach(Transform ControlPoint in FaceControlPoints) {
                            Vertices.Add(new Vector3(ControlPoint.transform.position.x, ControlPoint.transform.position.y, ControlPoint.transform.position.z));
                        } break;

                    case Placement.Middle:
                        FaceControlPoints[0] = Framework.ControlPoints[8];
                        FaceControlPoints[1] = Framework.ControlPoints[12];
                        FaceControlPoints[2] = Framework.ControlPoints[10];
                        FaceControlPoints[3] = Framework.ControlPoints[14];

                        Vertices = new List<Vector3>();
                        foreach(Transform ControlPoint in FaceControlPoints) {
                            Vertices.Add(new Vector3(ControlPoint.transform.position.x, ControlPoint.transform.position.y, ControlPoint.transform.position.z));
                        } break;
                    
                    case Placement.Top:
                        FaceControlPoints[0] = Framework.ControlPoints[10];
                        FaceControlPoints[1] = Framework.ControlPoints[14];
                        FaceControlPoints[2] = Framework.ControlPoints[2];
                        FaceControlPoints[3] = Framework.ControlPoints[6];

                        Vertices = new List<Vector3>();
                        foreach(Transform ControlPoint in FaceControlPoints) {
                            Vertices.Add(new Vector3(ControlPoint.transform.position.x, ControlPoint.transform.position.y, ControlPoint.transform.position.z));
                        } break;

                } break;

                case Face.Six:
                    FaceControlPoints[0] = Framework.ControlPoints[2];
                    FaceControlPoints[1] = Framework.ControlPoints[6];
                    FaceControlPoints[2] = Framework.ControlPoints[3];
                    FaceControlPoints[3] = Framework.ControlPoints[7];
                    
                    Vertices = new List<Vector3>();
                    foreach(Transform ControlPoint in FaceControlPoints) {
                        Vertices.Add(new Vector3(ControlPoint.transform.position.x, ControlPoint.transform.position.y, ControlPoint.transform.position.z));
                    }
                break;
        }
        GeneratePlane(size, resolution);
        AssignMesh(planeMesh);
    }

    Mesh GeneratePlane(float size, int resolution) {
        //Create Triangles
        Triangles = new List<int>();
        for(int row = 0; row < resolution; row++) {
            for(int column = 0; column < resolution; column++) {
                int i = (row * resolution) + row + column;
                //First Triangle
                Triangles.Add(i);
                Triangles.Add(i+(resolution)+1);
                Triangles.Add(i+(resolution)+2);

                // //Second Triangle
                Triangles.Add(i);
                Triangles.Add(i+resolution+2);
                Triangles.Add(i+1);
            }
        }

        planeMesh.Clear();
        planeMesh.vertices = Vertices.ToArray();
        planeMesh.triangles = Triangles.ToArray();
        planeMesh.RecalculateNormals();
        return planeMesh;
    }

    void AssignMesh(Mesh mesh) {
        Mesh filterMesh = MeshFilter.mesh;
        filterMesh.Clear();
        filterMesh.vertices = mesh.vertices;
        filterMesh.triangles = mesh.triangles;
        MeshCollider.sharedMesh = planeMesh;
    }
}

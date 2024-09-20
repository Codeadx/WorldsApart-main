using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
    
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class CubeMesh : MonoBehaviour
{
    Mesh planeMesh;
    Mesh cubeMesh;
    Mesh combinedMesh;
    MeshFilter MeshFilter;
    MeshCollider MeshCollider;
    
    //Plane settings
    [SerializeField] float size = 1;
    [SerializeField] int resolution = 1;

    //Mesh settings
    public List<Vector3> Vertices;
    List<int> Triangles;

    public CombineInstance[] combine = new CombineInstance[2];

    void Awake() {
        planeMesh = new Mesh();
        cubeMesh = new Mesh();
        combinedMesh = new Mesh();
        MeshFilter = GetComponent<MeshFilter>();
        MeshFilter.mesh = combinedMesh;
        MeshFilter.mesh.CombineMeshes(combine);
        MeshCollider = GetComponent<MeshCollider>();
    }

    // Update is called once per frame
    void Start() {
        GenerateCube(size, resolution, Vector3.zero);
        AssignMesh(cubeMesh);
    }

    Mesh GeneratePlane(float size, int resolution) {
        //Create Vertices
        Vertices = new List<Vector3>();
        float sizePerStep = size / resolution;

        Vector3 shiftValue = ((size / 2) * (Vector3.left + Vector3.down));
        for(int y = 0; y < resolution + 1; y++) {
            for(int x = 0; x < resolution + 1; x++) {
                Vertices.Add(new Vector3(x * sizePerStep, y * sizePerStep, 0) + shiftValue);
            }
        }

        //Create Triangles
        Triangles = new List<int>();
        for(int row = 0; row < resolution; row++) {
            for(int column = 0; column < resolution; column++) {
                int i = (row * resolution) + row + column;
                //First Triangle
                Triangles.Add(i);
                Triangles.Add(i+(resolution)+1);
                Triangles.Add(i+(resolution)+2);

                //Second Triangle
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

    Mesh GenerateCube(float size, int resolution, Vector3 origin) {
        Mesh planeMesh = GeneratePlane(size, resolution);
        Vertices.Clear();
        Triangles.Clear();
        //Front face
        List<Vector3> frontVertices = ShiftVertices(origin, planeMesh.vertices, -Vector3.forward * size / 2);
        List<int> frontTriangles = new List<int>(planeMesh.triangles);
        Vertices.AddRange(frontVertices);
        Triangles.AddRange(frontTriangles);

        //Back face
        List<Vector3> backVertices = ShiftVertices(origin, planeMesh.vertices, Vector3.forward * size / 2);
        List<int> backTriangles = ShiftTriangleIndexes(ReverseTriangles(planeMesh.triangles), Vertices.Count);
        Vertices.AddRange(backVertices);
        Triangles.AddRange(backTriangles);

        //Switch dimensions
        Mesh rotatedPlane = new Mesh();
        rotatedPlane.vertices = SwitchXandZ(planeMesh.vertices);
        rotatedPlane.triangles = planeMesh.triangles;

        //Right face
        List<Vector3> rightVertices = ShiftVertices(origin, rotatedPlane.vertices, Vector3.right * size / 2);
        List<int> rightTriangles = ShiftTriangleIndexes(rotatedPlane.triangles, Vertices.Count);
        Vertices.AddRange(rightVertices);
        Triangles.AddRange(rightTriangles);

        //Left face
        List<Vector3> leftVertices = ShiftVertices(origin, rotatedPlane.vertices, Vector3.left * size / 2);
        List<int> leftTriangles = ShiftTriangleIndexes(ReverseTriangles(rotatedPlane.triangles), Vertices.Count);
        Vertices.AddRange(leftVertices);
        Triangles.AddRange(leftTriangles);

        //Dimension switch
        rotatedPlane.vertices = SwitchYandZ(planeMesh.vertices);
        rotatedPlane.triangles = planeMesh.triangles;

        //Top face
        List<Vector3> topVertices = ShiftVertices(origin, rotatedPlane.vertices, Vector3.up * size / 2);
        List<int> topTriangles = ShiftTriangleIndexes(rotatedPlane.triangles, Vertices.Count);
        Vertices.AddRange(topVertices);
        Triangles.AddRange(topTriangles);

        //Bottom face
        List<Vector3> bottomVertices = ShiftVertices(origin, rotatedPlane.vertices, Vector3.down * size / 2);
        List<int> bottomTriangles = ShiftTriangleIndexes(ReverseTriangles(rotatedPlane.triangles), Vertices.Count);
        Vertices.AddRange(bottomVertices);
        Triangles.AddRange(bottomTriangles);

        cubeMesh.Clear();
        cubeMesh.vertices = Vertices.ToArray();
        cubeMesh.triangles = Triangles.ToArray();
        cubeMesh.RecalculateNormals();
        
        return cubeMesh;
    }

    List<Vector3> ShiftVertices(Vector3 origin, Vector3[] Vertices, Vector3 shiftValue) {
        List<Vector3> shiftedVertices = new List<Vector3>();
        foreach(Vector3 Vertex in Vertices) {
            shiftedVertices.Add(origin + Vertex + shiftValue);
        }
        return shiftedVertices;
    }

    List<int> ShiftTriangleIndexes(int[] triangles, int shiftValue) {
        List<int> newTriangles = new List<int>();
        foreach(int triangleIndex in triangles) {
            newTriangles.Add(triangleIndex + shiftValue);
        }
        return newTriangles;
    }

    Vector3[] SwitchXandZ(Vector3[] values)
    {
        for(int i = 0; i < values.Length; i++)
        {
            Vector3 value = values[i];
            float storedValue = value.x;
            value.x = value.z;
            value.z = storedValue;
            values[i] = value;
        }
        return values;
    }
    Vector3[] SwitchYandZ(Vector3[] values) {
        for(int i = 0; i < values.Length; i++) {
            Vector3 value = values[i];
            float storedValue = value.y;
            value.y = value.z;
            value.z = storedValue;
            values[i] = value;
        }
        return values;
    }

    int[] ReverseTriangles(int[] triangles) {
        System.Array.Reverse(triangles);
        return triangles;
    }

    void AssignMesh(Mesh mesh) {
        Mesh filterMesh = MeshFilter.mesh;
        filterMesh.Clear();
        filterMesh.vertices = mesh.vertices;
        filterMesh.triangles = mesh.triangles;
        MeshCollider.sharedMesh = cubeMesh;
    }
}

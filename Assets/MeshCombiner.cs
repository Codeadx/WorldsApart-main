using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    public Framework Framework;
    [SerializeField] MeshFilter[] meshFilters;

    void Awake() {
        for(int i = 0; i < 20; i++) {
            meshFilters[i] = Framework.GetComponent<BoxColliders>().ColliderContainer[i].GetComponent<MeshFilter>();
        }
    }

    public IEnumerator CombineMeshes(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }
        var mesh = new Mesh();
        mesh.CombineMeshes(combine);
        transform.GetComponent<MeshFilter>().mesh = mesh;
        //transform.GetComponent<MeshCollider>().sharedMesh = mesh;
        
        for(int j = 0; j < meshFilters.Length; j++) {
            //Destroy(meshFilters[j].GetComponent<CubeMesh>());
            Destroy(meshFilters[j]);
            Framework.ControlPoints[j].transform.gameObject.SetActive(false);
        }
        yield return null;
    }
}

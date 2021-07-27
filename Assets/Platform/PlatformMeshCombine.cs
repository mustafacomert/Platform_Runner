using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PlatformMeshCombine : MonoBehaviour
{
    private void Awake()
    {

        CombineMeshes();
    }

    private void CombineMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }
        Mesh m = transform.GetComponent<MeshFilter>().mesh;
        m = new Mesh();
        m.CombineMeshes(combine);
        GetComponent<MeshCollider>().sharedMesh = m;
        transform.gameObject.SetActive(true);
    }
}

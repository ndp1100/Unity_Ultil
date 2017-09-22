using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MeshCombiner : MonoBehaviour
{
#if UNITY_EDITOR
    // Use this for initialization
    void Start()
    {
        //MeshRenderer mesh = this.GetComponent<MeshRenderer>();
        //if (SceneManager.sceneCount > 1)
        //{
        //    if (mesh != null) mesh.lightmapIndex = 3;
        //}

    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject FinalCombineObject;

    public void StartCombine()
    {
        MeshFilter thisMesh = GetComponent<MeshFilter>();
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        List<MeshFilter> meshFilterList = new List<MeshFilter>(meshFilters);

        int i = 0;
        while (i < meshFilterList.Count) //get all meshrenderer in child
        {
            if (meshFilterList[i] == thisMesh || meshFilterList[i].sharedMesh == null)
            {
                meshFilterList.RemoveAt(i);
            }
            else
            {
                if (meshFilterList[i].GetComponent<MeshRenderer>().sharedMaterial.name.Contains("Instance"))
                {
                    Debug.Log(meshFilterList[i]);
                }
                i++;
            }
        }


        if (meshFilterList.Count == 0) return;

        //khong can phai sort ca list, chi loop qua list, check co 2 phan tu co InstanceID khac nhau hay ko
        /*meshFilterList.Sort((a, b) =>
        {
            Material aM = a.GetComponent<MeshRenderer>().sharedMaterial;
            Material bM = b.GetComponent<MeshRenderer>().sharedMaterial;
            return aM.GetInstanceID().CompareTo(bM.GetInstanceID());
        });
        
         if (meshFilterList[0].GetComponent<MeshRenderer>().sharedMaterial == meshFilterList[meshFilterList.Count - 1].GetComponent<MeshRenderer>().sharedMaterial)
        {
            GenerateMeshSingleMaterial(meshFilterList);
        }
        else
        {
            GenerateMeshMultiMaterial(meshFilterList);
        }
         
         */

        bool isMultilMaterial = false;
        for (int j = 0; j < meshFilterList.Count - 1; j++)
        {
            if (meshFilterList[j].GetComponent<MeshRenderer>().sharedMaterial !=
                meshFilterList[j + 1].GetComponent<MeshRenderer>().sharedMaterial)
            {
                isMultilMaterial = true;
                break;
            }
        }

        if (isMultilMaterial)
        {
            GenerateMeshMultiMaterial(meshFilterList);
        }
        else
        {
            GenerateMeshSingleMaterial(meshFilterList);
        }

    }

    public void GenerateMeshSingleMaterial(List<MeshFilter> meshFilters)
    {
        MeshFilter thisMesh = GetComponent<MeshFilter>();
        List<Material> materials = new List<Material>();
//        List<Vector2> combie_uv2 = new List<Vector2>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Count];

        int i = 0;
        while (i < meshFilters.Count)
        {
            if (meshFilters[i] == thisMesh || meshFilters[i].sharedMesh == null)
            {
                i++;
                continue;
            }
            GameObject child = meshFilters[i].gameObject;
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
            Material[] mats = meshRenderer.sharedMaterials;
            foreach (Material mat in mats)
            {
                if (!materials.Contains(mat)) materials.Add(mat);
            }
            //Debug.Log(i);

            combine[i].mesh = child.GetComponent<MeshFilter>().sharedMesh;
            combine[i].transform = this.transform.worldToLocalMatrix * child.transform.localToWorldMatrix;
            //combine[i].subMeshIndex = materials.IndexOf(material);
            /*Vector2[] uv2s = meshFilters[i].sharedMesh.uv2;
            //check if uv2 exist else use uv 
            if (uv2s.Length == 0) uv2s = meshFilters[i].sharedMesh.uv;
            float lm_scale_x = meshRenderer.lightmapScaleOffset.x;
            float lm_scale_y = meshRenderer.lightmapScaleOffset.y;
            float lm_offset_x = meshRenderer.lightmapScaleOffset.z;
            float lm_offset_y = meshRenderer.lightmapScaleOffset.w;

            for (int j = 0; j < uv2s.Length; j++)
            {
                Vector2 mod_uv2;
                mod_uv2.x = uv2s[j].x * lm_scale_x + lm_offset_x;
                mod_uv2.y = uv2s[j].y * lm_scale_y + lm_offset_y;

                combie_uv2.Add(mod_uv2);
            }*/

            i++;
        }


        if (transform.GetComponent<MeshFilter>() == null)
        {
            transform.gameObject.AddComponent<MeshFilter>();
        }

        transform.GetComponent<MeshFilter>().sharedMesh = new Mesh();
        transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine, true, true);

        if (transform.GetComponent<MeshRenderer>() == null)
        {
            transform.gameObject.AddComponent<MeshRenderer>();
        }

        MeshRenderer renderer = transform.GetComponent<MeshRenderer>();
        renderer.sharedMaterials = materials.ToArray();

        //transform.GetComponent<MeshFilter>().sharedMesh.uv2 = combie_uv2.ToArray();
        //renderer.lightmapIndex = 0;

        Unwrapping.GenerateSecondaryUVSet(transform.GetComponent<MeshFilter>().sharedMesh);
    }

    public void GenerateUV2()
    {
        Unwrapping.GenerateSecondaryUVSet(transform.GetComponent<MeshFilter>().sharedMesh);
    }


    public void GenerateMeshMultiMaterial(List<MeshFilter> meshFilters)
    {
        List<Material> materials = new List<Material>();
//        List<Vector2> combie_uv2 = new List<Vector2>();
        Dictionary<Material, ArrayList> combineDic = new Dictionary<Material, ArrayList>();
//        CombineInstance[] test = new CombineInstance[meshFilters.Count];

        int i = 0;
        while (i < meshFilters.Count)
        {
            if (meshFilters[i].sharedMesh == null)
            {
                i++;
                continue;
            }
            GameObject child = meshFilters[i].gameObject;
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();

            Material[] all_mat = meshRenderer.sharedMaterials;

            for (int ma = 0; ma < all_mat.Length; ma++)
            {
                Material mat = all_mat[ma];
                if (!materials.Contains(mat))
                {

                    materials.Add(mat);
                    combineDic.Add(mat, new ArrayList());
                }
                CombineInstance combineIns = new CombineInstance();
                // combineDic[mat].Add(combine);
                combineIns.mesh = child.GetComponent<MeshFilter>().sharedMesh;
                if (FinalCombineObject)
                    combineIns.transform = FinalCombineObject.transform.worldToLocalMatrix *
                                           child.transform.localToWorldMatrix;
                else
                    combineIns.transform = this.transform.worldToLocalMatrix * child.transform.localToWorldMatrix;

                int submesh_idx = Mathf.Min(ma, combineIns.mesh.subMeshCount - 1);

                combineIns.subMeshIndex = submesh_idx;


                combineDic[mat].Add(combineIns);

                /*Vector2[] uv2s = meshFilters[i].sharedMesh.uv2;
                //check if uv2 exist else use uv 
                if (uv2s.Length == 0) uv2s = meshFilters[i].sharedMesh.uv;
                float lm_scale_x = meshRenderer.lightmapScaleOffset.x;
                float lm_scale_y = meshRenderer.lightmapScaleOffset.y;
                float lm_offset_x = meshRenderer.lightmapScaleOffset.z;
                float lm_offset_y = meshRenderer.lightmapScaleOffset.w;

                for (int j = 0; j < uv2s.Length; j++)
                {
                    Vector2 mod_uv2;
                    mod_uv2.x = uv2s[j].x * lm_scale_x + lm_offset_x;
                    mod_uv2.y = uv2s[j].y * lm_scale_y + lm_offset_y;

                    combie_uv2.Add(mod_uv2);
                }*/

            }

            i++;


        }

        Mesh[] meshes = new Mesh[materials.Count];
        CombineInstance[] combineInstances = new CombineInstance[materials.Count];

        for (int m = 0; m < materials.Count; m++)
        {
            Material mat = materials[m];
            meshes[m] = new Mesh();
            CombineInstance[] combines = (combineDic[mat].ToArray(typeof(CombineInstance)) as CombineInstance[]);

            meshes[m].CombineMeshes(combines, true, true);
            combineInstances[m] = new CombineInstance();
            combineInstances[m].mesh = meshes[m];
            combineInstances[m].subMeshIndex = 0;
        }
        // Combine into one


        //[ 09/22/2017 1:59:15 PM: PhuongND] - combine into a new object (not push mesh in parent) if finalCombineObject != null
        Transform tCombinedMesm = FinalCombineObject != null ? FinalCombineObject.transform : transform;

        if (tCombinedMesm.GetComponent<MeshFilter>() == null)
        {
            tCombinedMesm.gameObject.AddComponent<MeshFilter>();
        }

        tCombinedMesm.GetComponent<MeshFilter>().sharedMesh = new Mesh();
        Mesh mesh = tCombinedMesm.transform.GetComponent<MeshFilter>().sharedMesh;
        mesh.name = this.name + "Mesh";
        mesh.subMeshCount = materials.Count;


        mesh.CombineMeshes(combineInstances, false, false);
        if (tCombinedMesm.GetComponent<MeshRenderer>() == null)
        {
            tCombinedMesm.gameObject.AddComponent<MeshRenderer>();
        }
        MeshRenderer renderer = tCombinedMesm.GetComponent<MeshRenderer>();
        renderer.sharedMaterials = materials.ToArray();

        Unwrapping.GenerateSecondaryUVSet(mesh);

        //End PhuongND 
    }
#endif
}

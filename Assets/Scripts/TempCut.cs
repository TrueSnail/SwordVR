using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class TempCut : MonoBehaviour
{
    private SkinnedMeshRenderer meshRenderer;
    private bool CutHappened = false;

    void Start()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    void Update()
    {
        if (Keyboard.current[Key.Space].isPressed) Cut();
        else CutHappened = false;
    }

    void Cut()
    {
        if (CutHappened) return;
        CutHappened = true;

        Mesh objMesh = new Mesh();
        meshRenderer.BakeMesh(objMesh, true);
        //meshRenderer.BakeMesh(objMesh, true);
        objMesh.boneWeights = meshRenderer.sharedMesh.boneWeights;
        objMesh.bindposes = meshRenderer.sharedMesh.bindposes;

        objMesh = DivideMesh(objMesh);

        //List<Vector3> vertex = new List<Vector3>();
        //objMesh.GetVertices(vertex);
        //vertex = vertex.Where(v => v.z > 0).ToList();
        //objMesh.vertices = vertex.ToArray();

        GameObject copy = new GameObject("Copy");
        SkinnedMeshRenderer newMeshRenderer = copy.AddComponent(meshRenderer);
        newMeshRenderer.rootBone = Instantiate(meshRenderer.rootBone.gameObject, copy.transform).transform;
        newMeshRenderer.rootBone.gameObject.name = meshRenderer.rootBone.gameObject.name;

        List<Transform> newBones = new(copy.GetComponentsInChildren<Transform>());
        List<Transform> reorderedBones = new();
        List<Matrix4x4> bindPoses = new();
        for (int i = 0; i < newMeshRenderer.bones.Length; i++)
        {
            Transform Bone = newBones.Find(T => T.name == newMeshRenderer.bones[i].name);
            reorderedBones.Add(Bone);
            bindPoses.Add(Bone.worldToLocalMatrix * copy.transform.localToWorldMatrix);
        }
        newMeshRenderer.bones = reorderedBones.ToArray();

        objMesh.bindposes = bindPoses.ToArray();
        newMeshRenderer.sharedMesh = objMesh;
        newMeshRenderer.materials = meshRenderer.materials;
        copy.transform.localScale = transform.localScale;
    }


    Mesh DivideMesh (Mesh mesh)
    {
        List<Vector3> vertex = new();
        List<BoneWeight> BoneWeights = new(mesh.boneWeights);
        List<Vector3> normals = new(mesh.normals);
        List<Vector2> uv = new(mesh.uv);
        List<List<int>> submeshes = new();
        
        mesh.GetVertices(vertex);
        List<int> map = Enumerable.Repeat(0, vertex.Count).ToList();
        for (int i = 0; i < mesh.subMeshCount; i++) submeshes.Add(new(mesh.GetTriangles(i)));

        for (int oldi = 0, i = 0; i < vertex.Count; i++, oldi++)
        {
            if (vertex[i].z < 0)
            {
                vertex.RemoveAt(i);
                BoneWeights.RemoveAt(i);
                uv.RemoveAt(i);
                normals.RemoveAt(i);
                map[oldi] = -1;
                i--;
            }
            else map[oldi] = i;
        }

        for (int submeshIndex = 0; submeshIndex < mesh.subMeshCount; submeshIndex++)
        {
            List<int> triangles = submeshes[submeshIndex];
            int startCOunt = triangles.Count;
            for (int i = 0; i < triangles.Count; i += 3)
            {
                int map0 = map[triangles[i]];
                int map1 = map[triangles[i + 1]];
                int map2 = map[triangles[i + 2]];

                if (map0 == -1 || map1 == -1 || map2 == -1)
                {
                    int startstartcount = triangles.Count;
                    triangles.RemoveAt(i + 2);
                    triangles.RemoveAt(i + 1);
                    triangles.RemoveAt(i);
                    i -= 3;
                }
                else
                {
                    triangles[i] = map0;
                    triangles[i + 1] = map1;
                    triangles[i + 2] = map2;
                }
            }
        }

        mesh.Clear();
        mesh.SetVertices(vertex);
        mesh.boneWeights = BoneWeights.ToArray();
        mesh.uv = uv.ToArray();
        mesh.normals = normals.ToArray();
        //mesh.triangles = triangles.ToArray();
        mesh.subMeshCount = 2;
        for (int i = 0; i < submeshes.Count; i++) mesh.SetTriangles(submeshes[i], i);
        mesh.Optimize();
        mesh.RecalculateBounds();

        return mesh;
    }

}

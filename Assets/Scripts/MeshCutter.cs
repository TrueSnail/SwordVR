using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class MeshCutter : MonoBehaviour
{
    private SkinnedMeshRenderer meshRenderer;
    private bool CutHappened = false;

    void Start()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    void Update()
    {
        if (Keyboard.current[Key.Space].isPressed) Cut(new Vector3(0,0,0), new Vector3(0, 1, 0), new Vector3(0, 0, 1));
        else CutHappened = false;
    }

    public void Cut(Vector3 point1, Vector3 point2, Vector3 point3)
    {
        if (CutHappened) return;
        CutHappened = true;

        Mesh objMesh = new Mesh();
        meshRenderer.BakeMesh(objMesh, true);
        objMesh.boneWeights = meshRenderer.sharedMesh.boneWeights;
        objMesh.bindposes = meshRenderer.sharedMesh.bindposes;

        DivideMesh(objMesh, new Plane(transform.InverseTransformPoint(point1), transform.InverseTransformPoint(point2), transform.InverseTransformPoint(point3)));

        //GameObject copy = new GameObject("Copy");
        //SkinnedMeshRenderer newMeshRenderer = copy.AddComponent(meshRenderer);
        //newMeshRenderer.rootBone = Instantiate(meshRenderer.rootBone.gameObject, copy.transform).transform;
        //newMeshRenderer.rootBone.gameObject.name = meshRenderer.rootBone.gameObject.name;

        //List<Transform> newBones = new(copy.GetComponentsInChildren<Transform>());
        //List<Transform> reorderedBones = new();
        //List<Matrix4x4> bindPoses = new();
        //for (int i = 0; i < newMeshRenderer.bones.Length; i++)
        //{
        //    Transform Bone = newBones.Find(T => T.name == newMeshRenderer.bones[i].name);
        //    reorderedBones.Add(Bone);
        //    bindPoses.Add(Bone.worldToLocalMatrix * copy.transform.localToWorldMatrix);
        //}
        //newMeshRenderer.bones = reorderedBones.ToArray();

        //objMesh.bindposes = bindPoses.ToArray();
        //newMeshRenderer.sharedMesh = objMesh;
        //newMeshRenderer.materials = meshRenderer.materials;
        //copy.transform.localScale = transform.localScale;
        //copy.transform.SetPositionAndRotation(transform.position, transform.rotation);
    }


    void DivideMesh (Mesh mesh, Plane dividePlane)
    {
        List<Vector3> vertex = new();
        //List<BoneWeight> BoneWeights = new(mesh.boneWeights);
        //List<Vector3> normals = new(mesh.normals);
        //List<Vector2> uv = new(mesh.uv);
        //List<List<int>> submeshes = new();

        mesh.GetVertices(vertex);
        //List<int> vertexShiftMap = Enumerable.Repeat(0, vertex.Count).ToList();
        //for (int i = 0; i < mesh.subMeshCount; i++) submeshes.Add(new(mesh.GetTriangles(i)));
        MeshCutterPart part1 = new MeshCutterPart(Instantiate(mesh));
        MeshCutterPart part2 = new MeshCutterPart(mesh);

        //for (int oldi = 0, i = 0; i < vertex.Count; i++, oldi++)
        //{
        //    if (vertex[i].z < 0)
        //    {
        //        vertex.RemoveAt(i);
        //        BoneWeights.RemoveAt(i);
        //        uv.RemoveAt(i);
        //        normals.RemoveAt(i);
        //        vertexShiftMap[oldi] = -1;
        //        i--;
        //    }
        //    else vertexShiftMap[oldi] = i;
        //}
        int i = 0;
        int part1Index = 0;
        int part2Index = 0;
        while (i < vertex.Count)
        {
            if (dividePlane.GetSide(vertex[i]))
            {
                part1.Vertices.RemoveAt(part1Index);
                part1.BoneWeights.RemoveAt(part1Index);
                part1.UV.RemoveAt(part1Index);
                part1.Normals.RemoveAt(part1Index);
                part1.VerticesShiftMap[i] = -1;
                part2.VerticesShiftMap[i] = part2Index;
                part1Index--;
            }
            else
            {
                part2.Vertices.RemoveAt(part2Index);
                part2.BoneWeights.RemoveAt(part2Index);
                part2.UV.RemoveAt(part2Index);
                part2.Normals.RemoveAt(part2Index);
                part2.VerticesShiftMap[i] = -1;
                part1.VerticesShiftMap[i] = part1Index;
                part2Index--;
            }
            i++;
            part1Index++;
            part2Index++;
        }

        //for (int submeshIndex = 0; submeshIndex < mesh.subMeshCount; submeshIndex++)
        //{
        //    List<int> triangles = submeshes[submeshIndex];
        //    for (int i = 0; i < triangles.Count; i += 3)
        //    {
        //        int map0 = vertexShiftMap[triangles[i]];
        //        int map1 = vertexShiftMap[triangles[i + 1]];
        //        int map2 = vertexShiftMap[triangles[i + 2]];

        //        if (map0 == -1 || map1 == -1 || map2 == -1)
        //        {
        //            triangles.RemoveAt(i + 2);
        //            triangles.RemoveAt(i + 1);
        //            triangles.RemoveAt(i);
        //            i -= 3;
        //        }
        //        else
        //        {
        //            triangles[i] = map0;
        //            triangles[i + 1] = map1;
        //            triangles[i + 2] = map2;
        //        }
        //    }
        //}
        part1.UpdateTriangles();
        part2.UpdateTriangles();

        //mesh.Clear();
        //mesh.SetVertices(vertex);
        //mesh.boneWeights = BoneWeights.ToArray();
        //mesh.uv = uv.ToArray();
        //mesh.normals = normals.ToArray();
        //mesh.subMeshCount = 2;
        //for (int i = 0; i < submeshes.Count; i++) mesh.SetTriangles(submeshes[i], i);
        //mesh.Optimize();
        //mesh.RecalculateBounds();
        //return mesh;

        part1.GenerateMesh();
        part2.GenerateMesh();
        part1.CreateGameObj("Part1", meshRenderer, transform);
        part2.CreateGameObj("Part2", meshRenderer, transform);
    }

}

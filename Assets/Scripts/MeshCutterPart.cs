using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class MeshCutterPart : ScriptableObject
{
    public Mesh MeshPart { get; private set; }
    public List<Vector3> Vertices = new();
    public List<List<int>> Submeshes = new();
    public List<int> VerticesShiftMap;
    public List<BoneWeight> BoneWeights;
    public List<Vector3> Normals;
    public List<Vector2> UV;

    public MeshCutterPart(Mesh mesh)
    {
        MeshPart = mesh;
        BoneWeights = new(mesh.boneWeights);
        Normals = new(mesh.normals);
        UV = new(mesh.uv);

        mesh.GetVertices(Vertices);
        VerticesShiftMap = Enumerable.Repeat(0, Vertices.Count).ToList();
        for (int i = 0; i < mesh.subMeshCount; i++) Submeshes.Add(new(mesh.GetTriangles(i)));
    }

    public void UpdateTriangles()
    {
        for (int submeshIndex = 0; submeshIndex < MeshPart.subMeshCount; submeshIndex++)
        {
            List<int> triangles = Submeshes[submeshIndex];
            for (int i = 0; i < triangles.Count; i += 3)
            {
                int map0 = VerticesShiftMap[triangles[i]];
                int map1 = VerticesShiftMap[triangles[i + 1]];
                int map2 = VerticesShiftMap[triangles[i + 2]];

                if (map0 == -1 || map1 == -1 || map2 == -1)
                {
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
    }

    public Mesh GenerateMesh()
    {
        MeshPart.Clear();
        MeshPart.SetVertices(Vertices);
        MeshPart.boneWeights = BoneWeights.ToArray();
        MeshPart.uv = UV.ToArray();
        MeshPart.normals = Normals.ToArray();
        MeshPart.subMeshCount = 2;
        for (int i = 0; i < Submeshes.Count; i++) MeshPart.SetTriangles(Submeshes[i], i);
        MeshPart.Optimize();
        MeshPart.RecalculateBounds();
        return MeshPart;
    }

    public void CreateGameObj(string name, SkinnedMeshRenderer meshRenderer, Transform transform)
    {
        GameObject copy = new GameObject(name);
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

        MeshPart.bindposes = bindPoses.ToArray();
        newMeshRenderer.sharedMesh = MeshPart;
        newMeshRenderer.materials = meshRenderer.materials;
        copy.transform.localScale = transform.localScale;
        copy.transform.SetPositionAndRotation(transform.position, transform.rotation);

        var rigidbodies = copy.GetComponentsInChildren<Rigidbody>();
        var colliders = copy.GetComponentsInChildren<Collider>();
        foreach (var rigidbody in rigidbodies)
        {
            var joint = rigidbody.GetComponent<CharacterJoint>();
            if (joint != null) joint.autoConfigureConnectedAnchor = false;
            rigidbody.isKinematic = false;
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
    }
}

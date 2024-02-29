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
    }


    void DivideMesh (Mesh mesh, Plane dividePlane)
    {
        List<Vector3> vertex = new();

        mesh.GetVertices(vertex);
        MeshCutterPart part1 = new MeshCutterPart(Instantiate(mesh));
        MeshCutterPart part2 = new MeshCutterPart(mesh);

        int i = 0;
        int part1Index = 0;
        int part2Index = 0;
        while (i < vertex.Count)
        {
            if (dividePlane.GetSide(vertex[i]))
            {
                part1.Vertices[i] = Vector3.positiveInfinity;
                part1.BoneWeights[i] = new BoneWeight() { weight0 = float.PositiveInfinity };
                part1.UV[i] = Vector2.positiveInfinity;
                part1.Normals[i] = Vector3.positiveInfinity;
                //part1.Vertices.RemoveAt(part1Index);
                //part1.BoneWeights.RemoveAt(part1Index);
                //part1.UV.RemoveAt(part1Index);
                //part1.Normals.RemoveAt(part1Index);
                part1.VerticesShiftMap[i] = -1;
                part2.VerticesShiftMap[i] = part2Index;
                part1Index--;
            }
            else
            {
                part2.Vertices[i] = Vector3.positiveInfinity;
                part2.BoneWeights[i] = new BoneWeight() { weight0 = float.PositiveInfinity };
                part2.UV[i] = Vector2.positiveInfinity;
                part2.Normals[i] = Vector3.positiveInfinity;
                //part2.Vertices.RemoveAt(part2Index);
                //part2.BoneWeights.RemoveAt(part2Index);
                //part2.UV.RemoveAt(part2Index);
                //part2.Normals.RemoveAt(part2Index);
                part2.VerticesShiftMap[i] = -1;
                part1.VerticesShiftMap[i] = part1Index;
                part2Index--;
            }
            i++;
            part1Index++;
            part2Index++;
        }
        part1.Vertices = part1.Vertices.Where(Vec => Vec.x < float.PositiveInfinity).ToList();
        part1.BoneWeights = part1.BoneWeights.Where(Bone => Bone.weight0 < float.PositiveInfinity).ToList();
        part1.UV = part1.UV.Where(Vec => Vec.x < float.PositiveInfinity).ToList();
        part1.Normals = part1.Normals.Where(Vec => Vec.x < float.PositiveInfinity).ToList();
        part2.Vertices = part2.Vertices.Where(Vec => Vec.x < float.PositiveInfinity).ToList();
        part2.BoneWeights = part2.BoneWeights.Where(Bone => Bone.weight0 < float.PositiveInfinity).ToList();
        part2.UV = part2.UV.Where(Vec => Vec.x < float.PositiveInfinity).ToList();
        part2.Normals = part2.Normals.Where(Vec => Vec.x < float.PositiveInfinity).ToList();

        part1.UpdateTriangles();
        part2.UpdateTriangles();

        part1.GenerateMesh();
        part2.GenerateMesh();
        part1.CreateGameObj("Part1", meshRenderer, transform);
        part2.CreateGameObj("Part2", meshRenderer, transform);
    }

}

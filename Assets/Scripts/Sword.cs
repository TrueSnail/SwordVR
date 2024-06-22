using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Sword : MonoBehaviour, ISword
{
    private bool _active;
    public bool Active
    {
        get => _active;
        set
        {
            EdgeMaterial.SetColor("_Glow", value ? DefaultEmissionColor : Color.black);
            _active = value;
        }
    }
    public bool StartActivation;
    public AudioClip cutSound;
    [ColorUsage(false, true)]
    public Color DefaultEmissionColor;

    public event Action OnCutBegin;
    public event Action<CutState> OnCutEnd;

    private Material EdgeMaterial;
    private SkinnedMeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        EdgeMaterial = meshRenderer.materials[1];
        //DefaultEmissionColor = EdgeMaterial.GetColor("_Glow");

        Active = StartActivation;
    }

    public void BeginCut(Vector3 position, HealthControler cutObject)
    {
        OnCutBegin?.Invoke();
    }

    public void EndCut(CutState state, List<Vector3> points, HealthControler cutObject)
    {
        OnCutEnd?.Invoke(state);
        if (state == CutState.Success)
        {
            GetComponent<AudioSource>().clip = cutSound;
            GetComponent<AudioSource>().Play();
        }
    }

    public float GetDamageValue() => Active ? 1 : 0;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Sword : MonoBehaviour, ISword
{
    public float CooldownTime = 1;
    public ActionBasedController controller;
    public InputActionReference grip;
    public AudioClip cutSound;

    private Material EdgeMaterial;
    private SkinnedMeshRenderer meshRenderer;
    private Color DefaultEmissionColor;
    private bool CanDamage = true;

    void Start()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        EdgeMaterial = meshRenderer.materials[1];
        //meshRenderer.materials[1] = EdgeMaterial;
        DefaultEmissionColor = EdgeMaterial.GetColor("_Glow");
        grip.action.performed += GripPress;
        grip.action.canceled += GripPress;
    }

    private void GripPress(InputAction.CallbackContext obj)
    {
        float pressAmount = obj.ReadValue<float>();
        meshRenderer.SetBlendShapeWeight(0, 100 * (1 - pressAmount));
    }

    private void OnCollisionEnter(Collision collision)
    {
        controller.SendHapticImpulse(0.03f, 0.1f);
    }

    public void OnCutBegin(Vector3 position, HealthControler cutObject)
    {
        controller.SendHapticImpulse(0.2f, 0.2f);
    }

    public void OnCutEnd(CutState state, List<Vector3> points, HealthControler cutObject)
    {
        if (state == CutState.Success)
        {
            controller.SendHapticImpulse(1f, 0.45f);
            GetComponent<AudioSource>().clip = cutSound;
            GetComponent<AudioSource>().Play();
            CanDamage = false;
            EdgeMaterial.SetColor("_Glow", Color.black);
            StartCoroutine(StartCooldown());
        }
    }

    IEnumerator StartCooldown()
    {
        yield return new WaitForSeconds(CooldownTime);
        CanDamage = true;
        EdgeMaterial.SetColor("_Glow", DefaultEmissionColor);
    }

    public float GetDamageValue() => CanDamage ? 1 : 0;
}

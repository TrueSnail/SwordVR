using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerSwordController : MonoBehaviour
{
    public float CooldownTime = 1;
    public ActionBasedController controller;
    public InputActionReference grip;
    public Sword Sword;

    private SkinnedMeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();

        Sword.OnCutBegin += CutBegun;
        Sword.OnCutEnd += CutEnded;
        grip.action.performed += GripPress;
        grip.action.canceled += GripPress;
    }

    private void OnCollisionEnter(Collision collision)
    {
        controller.SendHapticImpulse(0.03f, 0.1f);
    }

    private void GripPress(InputAction.CallbackContext obj)
    {
        float pressAmount = obj.ReadValue<float>();
        meshRenderer.SetBlendShapeWeight(0, 100 * (1 - pressAmount));
    }

    private void CutBegun()
    {
        controller.SendHapticImpulse(0.2f, 0.2f);
    }

    private void CutEnded(CutState state)
    {
        if (state == CutState.Success)
        {
            controller.SendHapticImpulse(1f, 0.45f);
            Sword.Active = false;
            Invoke(nameof(ResetCooldown), CooldownTime);
        }
    }

    private void ResetCooldown() => Sword.Active = true;
}

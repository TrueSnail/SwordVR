using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sword : MonoBehaviour
{
    private SkinnedMeshRenderer meshRenderer;
    public InputActionReference grip;

    void Start()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        grip.action.performed += GripPress;
        grip.action.canceled += GripPress;
    }

    private void GripPress(InputAction.CallbackContext obj)
    {
        float pressAmount = obj.ReadValue<float>();
        meshRenderer.SetBlendShapeWeight(0, 100 * (1 - pressAmount));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerDash : MonoBehaviour
{
    public GameObject Player;
    public GameObject CameraObject;
    public GameObject DashProgress;
    public ActionBasedController controller;
    public float DashDelay;
    public float DashDistance;
    public InputActionReference joystick;

    private bool CanDash = true;

    private void Start()
    {
        joystick.action.performed += JoystickMove;
        joystick.action.started += JoystickMove;
        joystick.action.canceled += (_) => DashProgress.transform.GetChild(0).transform.position = new Vector3(0, -1000, 0);
    }

    private void Update()
    {
        DashProgress.transform.LookAt(CameraObject.transform);
        DashProgress.transform.GetChild(0).rotation = Quaternion.LookRotation(Vector3.up);
    }

    private void JoystickMove(InputAction.CallbackContext obj)
    {
        Vector2 DashDirection = obj.ReadValue<Vector2>();
        Vector3 DashPosition = CameraObject.transform.TransformDirection(new Vector3(DashDirection.x, 0, DashDirection.y));
        DashPosition = new Vector3(DashPosition.x, 0, DashPosition.z).normalized;
        DashPosition = Player.transform.position + (DashPosition * DashDistance);
        DashProgress.transform.GetChild(0).transform.position = DashPosition + (Vector3.up * 0.1f);

        if (CanDash && DashDirection.magnitude > 0.8f)
        {
            Player.transform.position = DashPosition;
            CanDash = false;
            Invoke(nameof(ResetDash), DashDelay);
            DashProgress.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;
            DashProgress.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    private void ResetDash()
    {
        CanDash = true;
        DashProgress.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.green;
        DashProgress.GetComponent<SpriteRenderer>().color = Color.green;
        controller.SendHapticImpulse(0.1f, 0.1f);
    }
}

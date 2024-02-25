using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthControlerCollider : MonoBehaviour
{
    public event Action<Collision, HealthControlerCollider> OnObjectEnter;
    public event Action<Collision, HealthControlerCollider> OnObjectStay;
    public event Action<Collision, HealthControlerCollider> OnObjectExit;
    public bool DamageCollider = false;
    public ContactPoint LastContactPoint { get; private set; }

    private void OnCollisionEnter(Collision collision)
    {
        OnObjectEnter?.Invoke(collision, this);
    }
    private void OnCollisionExit(Collision collision)
    {
        OnObjectExit?.Invoke(collision, this);
    }
    private void OnCollisionStay(Collision collision)
    {
        OnObjectStay?.Invoke(collision, this);
        LastContactPoint = collision.GetContact(0);
    }
}

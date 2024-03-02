using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthControlerCollider : MonoBehaviour
{
    public event Action<Collision, HealthControlerCollider, ISword> OnObjectEnter;
    public event Action<Collision, HealthControlerCollider, ISword> OnObjectStay;
    public event Action<Collision, HealthControlerCollider, ISword> OnObjectExit;
    public bool DamageCollider = false;
    public ContactPoint LastContactPoint { get; private set; }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent(out ISword sword)) return;
        OnObjectEnter?.Invoke(collision, this, sword);
    }
    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent(out ISword sword)) return;
        OnObjectExit?.Invoke(collision, this, sword);
    }
    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent(out ISword sword)) return;
        OnObjectStay?.Invoke(collision, this, sword);
        LastContactPoint = collision.GetContact(0);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HealthControler : MonoBehaviour
{
    public MeshCutter meshCutter;
    public float HP;
    HealthControlerCollider[] colliders;
    List<HealthControlerColisionObject> ObjectsInside = new();

    private void Start()
    {
        colliders = GetComponentsInChildren<HealthControlerCollider>();
        foreach(var collider in colliders)
        {
            collider.OnObjectEnter += ObjectEntered;
            collider.OnObjectExit += ObjectExited;
        }
    }

    private void ObjectEntered(Collision arg1, HealthControlerCollider arg2, ISword sword)
    {
        var obj = ObjectsInside.FirstOrDefault(obj => obj.gameObject == arg1.gameObject);

        if (obj == null)
        {
            obj = new HealthControlerColisionObject() { gameObject = arg1.gameObject, collisionAmount = 1, sword = sword};
            sword.BeginCut(arg1.GetContact(0).point, this);
            obj.CutPoints.Add(arg1.GetContact(0).point);
            ObjectsInside.Add(obj);
        }
        else obj.collisionAmount++;

        if (arg2.DamageCollider)
        {
            obj.dealsDamage = true;
            obj.CutPoints.Add(arg1.GetContact(0).point);
        }
    }

    private void ObjectExited(Collision arg1, HealthControlerCollider arg2, ISword sword)
    {
        var obj = ObjectsInside.FirstOrDefault(obj => obj.gameObject == arg1.gameObject);
        if (obj == null) return;

        obj.collisionAmount--;
        obj.CutPoints.Add(arg2.LastContactPoint.point);
        if (obj.collisionAmount == 0)
        {
            ObjectsInside.Remove(obj);
            float DamageValue = sword.GetDamageValue();
            if (obj.dealsDamage && meshCutter != null && DamageValue > 0)
            {
                HP -= DamageValue;
                sword.EndCut(CutState.Success, obj.CutPoints, this);
                if (HP > 0) return;

                meshCutter.Cut(obj.CutPoints[0], obj.CutPoints[1], arg2.LastContactPoint.point);
                Destroy(gameObject);
            }
            else sword.EndCut(CutState.Failed, obj.CutPoints, this);
        }
    }

}

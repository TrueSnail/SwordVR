using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthControlerColisionObject
{
    public GameObject gameObject;
    public ISword sword;
    public int collisionAmount = 0;
    public bool dealsDamage = false;
    public List<Vector3> CutPoints = new();
}

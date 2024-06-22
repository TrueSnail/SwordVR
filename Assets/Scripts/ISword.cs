using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISword
{
    void BeginCut(Vector3 position, HealthControler cutObject);
    void EndCut(CutState state, List<Vector3> points, HealthControler cutObject);
    float GetDamageValue();
}

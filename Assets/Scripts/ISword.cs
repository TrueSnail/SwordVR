using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISword
{
    void OnCutBegin(Vector3 position, HealthControler cutObject);
    void OnCutEnd(CutState state, List<Vector3> points, HealthControler cutObject);
    float GetDamageValue();
}

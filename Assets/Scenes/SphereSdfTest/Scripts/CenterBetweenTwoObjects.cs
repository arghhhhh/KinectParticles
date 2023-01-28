using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CenterBetweenTwoObjects : MonoBehaviour
{
    public Transform transform1;
    public Transform transform2;

    void Update()
    {
        Vector3 goToPos = Vector3.LerpUnclamped(transform1.position, transform2.position, 0.5f);
        gameObject.transform.position = goToPos;
    }

    float EaseOutElastic(float x) {
        double c3 = (2 * Math.PI) / 3;
        float c4 = Convert.ToSingle(c3);

        if (x == 0) return 0;
        else if (x == 1) return 1;
        else return Convert.ToSingle(Math.Pow(2, -10 * x) * Math.Sin((x * 10 - 0.75) * c4) + 1);
    }
}

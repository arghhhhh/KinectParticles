using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VectorTesting : MonoBehaviour
{
    public Transform A;
    public Transform B;
    public Transform C;

    public GameObject point;

    [Range(0,1f)]
    public float lerpVal;
    [Range(0,1f)]
    public float slerpVal;

    void Start()
    {
        Instantiate(point, A);
        Instantiate(point, B);
        Instantiate(point, C);
    }
    void Update()
    {
        Vector3 BA = B.position - A.position;
        Vector3 BC = B.position - C.position;
        Vector3 lerpPoint = Vector3.Lerp(A.position, C.position, lerpVal);
        Vector3 slerpPoint = Vector3.Slerp(A.position, C.position, slerpVal);

        Debug.DrawLine(A.position, B.position, Color.green);
        Debug.DrawLine(B.position, C.position, Color.red);
        Debug.DrawLine(A.position, C.position, Color.blue);
        Debug.DrawLine(B.position, lerpPoint, Color.yellow);
        Debug.DrawLine(B.position, slerpPoint, Color.white);
    }
}

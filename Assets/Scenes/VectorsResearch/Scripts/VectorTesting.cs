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

    [Range(0,2f)]
    public float scalar;

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
        Vector3 AC = C.position - A.position;
        float ba = BA.magnitude;
        float bc = BC.magnitude;
        float ratio = ba/(ba+bc);
        
        Vector3 X = Vector3.Lerp(A.position, C.position, ratio);

        Vector3 BX = B.position - X;
        Vector3 D = C.position + BX;
        Vector3 DC = D - C.position;
        Vector3 N = DC * scalar + C.position;

        Debug.DrawLine(A.position, B.position, Color.green);
        Debug.DrawLine(B.position, C.position, Color.red);
        Debug.DrawLine(A.position, C.position, Color.blue);
        Debug.DrawLine(B.position, X, Color.yellow);
        Debug.DrawLine(C.position, D, Color.magenta);
        Debug.DrawLine(C.position, N, Color.white);
    }
}

using BezierSolution;
using DG.Tweening;
using UnityEngine;

public class rot : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody rb;
    void OnEnable()
    {
        walk.OnJoyStickMove += Move;
    }

    private void Move(BezierSpline.Segment s)
    {
        Quaternion targetRotation;
        Vector3 tandent = s.GetTangent();
        Vector3 normal = s.GetNormal();
        targetRotation = Quaternion.LookRotation(tandent, normal);
        
        
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.fixedTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

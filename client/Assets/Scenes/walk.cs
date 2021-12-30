using System;
using System.Collections;
using System.Collections.Generic;
using BezierSolution;
using UnityEngine;
using DG.Tweening;

public class walk : MonoBehaviour
{
    public delegate void JoystickEvent(BezierSpline.Segment s);

    public static event JoystickEvent OnJoyStickMove;
    public BezierSpline Spline;
    public float Normalt = 0;
    private Coroutine _walk;
    private float speed = 0.01f;
    private Rigidbody _rigidbody;

    public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
       
    }

    private void FixedUpdate()
    {
        Vector3 targetpos = Spline.MoveAlongSpline(ref Normalt, speed * Time.fixedTime, 10);
        targetpos *= -1;
        _rigidbody.MovePosition(targetpos);
       

        BezierSpline.Segment segment = Spline.GetSegmentAt(Normalt);
        OnJoyStickMove?.Invoke(segment);
        // Quaternion targetRotation;
        // Vector3 tandent = segment.GetTangent();
        // Vector3 normal = segment.GetNormal();
        // targetRotation = Quaternion.LookRotation(tandent, normal);
        //
        //
        // targetRotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.fixedTime);
        //transform.rotation = targetRotation;
    }
}
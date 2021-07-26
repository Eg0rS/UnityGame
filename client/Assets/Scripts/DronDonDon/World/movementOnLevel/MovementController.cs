using System;
using System.Collections;
using Adept.Logger;
using System.Collections.Generic;
using DronDonDon.Core.Filter;
using Unity.Mathematics;
using UnityEngine;

public class MovementController : MonoBehaviour 
{
    public Transform _pfTrack=null;
    private List<Vector3> _points=null;
    private BezierPath _bezierPath;

    private float _passOfGroup = 0;        //процент прохождения текущей группы 
    private int _currentGroup = 0;        //текущая группа 
    private int _countGroup = 0;        //кол-во групп
    private bool _moveOnCurve = true;
    
    public float _speed = 0.05f;
    
    private void Start()
    {
        if (_pfTrack == null || _pfTrack.childCount < 4) return;
        
        // получение всех точек с карты
        _points =new List<Vector3>();
        for (int i = 0; i<_pfTrack.childCount; i++)
        {
            _points.Add(_pfTrack.GetChild(i).transform.position);
        }

        _countGroup = _points.Count  / 4;
    }
    
    private void Update()
    {
        if (_points==null || _currentGroup >= _countGroup) return;

        //установка новых положения и поворота
        _passOfGroup += _speed * Time.deltaTime;

        if (_moveOnCurve)
        {
            transform.position = Beizer.GetPoint(_points[_currentGroup * 4],
                _points[_currentGroup * 4 + 1],
                _points[_currentGroup * 4 + 2],
                _points[_currentGroup * 4 + 3],
                _passOfGroup);
            /*
            transform.rotation = Quaternion.LookRotation(Beizer.GetDerivative(_points[_currentGroup * 4],
                _points[_currentGroup * 4 + 1],
                _points[_currentGroup * 4 + 2],
                _points[_currentGroup * 4 + 3],
                _passOfGroup));
                */
        }
        else
        {
            transform.position = Vector3.Lerp(_points[(_currentGroup-1)*4 +3],_points[(_currentGroup)*4], _passOfGroup);
            /* transform.rotation =
                Quaternion.LookRotation(_points[(_currentGroup + 1) * 4] - _points[(_currentGroup) * 4 + 3]); 
                */
        }

        if (_passOfGroup >= 1)
        {
            _passOfGroup = 0;
            _moveOnCurve = !_moveOnCurve;
            if (_moveOnCurve == false)
            {
                _currentGroup++;
            }
        }
        
    }
}

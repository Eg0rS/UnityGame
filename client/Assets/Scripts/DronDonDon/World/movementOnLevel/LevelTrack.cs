using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelTrack : MonoBehaviour
{
    private List<Vector3> _points;
    
    private void Start()
    {
        _points =new List<Vector3>();
    }

    private void OnDrawGizmos()        // отрисовка кривой 
    {
        // получение всех точек с карты
        _points.Clear();
        if ( _points==null || transform.childCount < 4) return;

        for (int i = 0; i<gameObject.transform.childCount; i++)
        {
            _points.Add(transform.GetChild(i).transform.position);
        }
        
        
        int _sigmentNumber = 20;          // качество отрисовки
        Vector3 preveousePoint = _points[0] ;
        
        for (int i = 0; i < _points.Count/4; i++)        // перебор всеx групп по 4 точки
        {
            for (int j = 0; j < _sigmentNumber; j++)        // отрисовка нескольких линий для имитации одной плавной 
            {
                float parameter =(float) j / _sigmentNumber; 
                
                Vector3 point = Beizer.GetPoint( _points[i*4],
                    _points[i*4 +1],
                    _points[i*4 +2],
                    _points[i*4 +3],
                    parameter);
                
                Gizmos.DrawLine(preveousePoint, point);        // отрисовка от предыдущей точки до следующей полученной 
                preveousePoint = point;
            }
         
        }
    }
 
}

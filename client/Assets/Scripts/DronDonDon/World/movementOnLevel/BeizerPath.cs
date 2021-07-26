
using UnityEngine;
using System.Collections.Generic;

/**
    Class for representing a Bezier path, and methods for getting suitable points to 
    draw the curve with line segments.
*/
public class BezierPath
{
    private const int SEGMENTS_PER_CURVE = 10;
    private const float MINIMUM_SQR_DISTANCE = 0.01f;

    // This corresponds to about 172 degrees, 8 degrees from a traight line
    private const float DIVISION_THRESHOLD = -0.99f; 

    private List<Vector3> controlPoints;

    private int curveCount; //how many bezier curves in this path?

    /**
        Constructs a new empty Bezier curve. Use one of these methods
        to add points: SetControlPoints, Interpolate, SamplePoints.
    */
    public BezierPath()
    {
        controlPoints = new List<Vector3>();
    }

    /**
        Sets the control points of this Bezier path.
        Points 0-3 forms the first Bezier curve, points 
        3-6 forms the second curve, etc.
    */
    public void SetControlPoints(List<Vector3> newControlPoints)
    {
        controlPoints.Clear();
        controlPoints.AddRange(newControlPoints);
        curveCount = (controlPoints.Count - 1) / 3;
    }

    /**
        Returns the control points for this Bezier curve.
    */
    public List<Vector3> GetControlPoints()
    {
        return controlPoints;
    }

    /**
        Calculates a Bezier interpolated path for the given points.
    */
    public void Interpolate(List<Vector3> segmentPoints, float scale)
    {
        controlPoints.Clear();

        if (segmentPoints.Count < 2)
        {
            return;
        }

        for (int i = 0; i < segmentPoints.Count; i++)
        {
            if (i == 0) // is first
            {
                Vector3 p1 = segmentPoints[i];
                Vector3 p2 = segmentPoints[i + 1];                

                Vector3 tangent = (p2 - p1);
                Vector3 q1 = p1 + scale * tangent;

                controlPoints.Add(p1);
                controlPoints.Add(q1);
            }
            else if (i == segmentPoints.Count - 1) //last
            {
                Vector3 p0 = segmentPoints[i - 1];
                Vector3 p1 = segmentPoints[i];
                Vector3 tangent = (p1 - p0);
                Vector3 q0 = p1 - scale * tangent;

                controlPoints.Add(q0);
                controlPoints.Add(p1);
            }
            else
            {
                Vector3 p0 = segmentPoints[i - 1];
                Vector3 p1 = segmentPoints[i];
                Vector3 p2 = segmentPoints[i + 1];
                Vector3 tangent = (p2 - p0).normalized;
                Vector3 q0 = p1 - scale * tangent * (p1 - p0).magnitude;
                Vector3 q1 = p1 + scale * tangent * (p2 - p1).magnitude;

                controlPoints.Add(q0);
                controlPoints.Add(p1);
                controlPoints.Add(q1);
            }
        }

        curveCount = (controlPoints.Count - 1) / 3;
    }   

    /**
        Sample the given points as a Bezier path.
    */
    public void SamplePoints(List<Vector3> sourcePoints, float minSqrDistance, float maxSqrDistance, float scale)
    {
        if(sourcePoints.Count < 2)
        {
            return;
        }

        Stack<Vector3> samplePoints = new Stack<Vector3>();
        
        samplePoints.Push(sourcePoints[0]);
        
        Vector3 potentialSamplePoint = sourcePoints[1];

        int i = 2;

        for (i = 2; i < sourcePoints.Count; i++ )
        {
            if(
                ((potentialSamplePoint - sourcePoints[i]).sqrMagnitude > minSqrDistance) &&
                ((samplePoints.Peek() - sourcePoints[i]).sqrMagnitude > maxSqrDistance))
            {
                samplePoints.Push(potentialSamplePoint);
            }

            potentialSamplePoint = sourcePoints[i];
        }

        //now handle last bit of curve
        Vector3 p1 = samplePoints.Pop(); //last sample point
        Vector3 p0 = samplePoints.Peek(); //second last sample point
        Vector3 tangent = (p0 - potentialSamplePoint).normalized;
        float d2 = (potentialSamplePoint - p1).magnitude;
        float d1 = (p1 - p0).magnitude;
        p1 = p1 + tangent * ((d1 - d2)/2);

        samplePoints.Push(p1);
        samplePoints.Push(potentialSamplePoint);


        Interpolate(new List<Vector3>(samplePoints), scale);
    }

    /**
        Caluclates a point on the path.
        
        @param curveIndex The index of the curve that the point is on. For example, 
        the second curve (index 1) is the curve with controlpoints 3, 4, 5, and 6.
        
        @param t The paramater indicating where on the curve the point is. 0 corresponds 
        to the "left" point, 1 corresponds to the "right" end point.
    */
    public Vector3 CalculateBezierPoint(int curveIndex, float t)
    {
        int nodeIndex = curveIndex * 3;

        Vector3 p0 = controlPoints[nodeIndex];
        Vector3 p1 = controlPoints[nodeIndex + 1];
        Vector3 p2 = controlPoints[nodeIndex + 2];
        Vector3 p3 = controlPoints[nodeIndex + 3];

        return CalculateBezierPoint(t, p0, p1, p2, p3);
    }

    /**
        Gets the drawing points. This implementation simply calculates a certain number
        of points per curve.
    */
    public List<Vector3> GetDrawingPoints0()
    {
        List<Vector3> drawingPoints = new List<Vector3>();

        for (int curveIndex = 0; curveIndex < curveCount; curveIndex++)
        {
            if (curveIndex == 0) //Only do this for the first end point. 
            //When i != 0, this coincides with the 
            //end point of the previous segment,
            {
                drawingPoints.Add(CalculateBezierPoint(curveIndex, 0));
            }

            for (int j = 1; j <= SEGMENTS_PER_CURVE; j++)
            {
                float t = j / (float)SEGMENTS_PER_CURVE;
                drawingPoints.Add(CalculateBezierPoint(curveIndex, t));
            }
        }

        return drawingPoints;
    }

    /**
        Gets the drawing points. This implementation simply calculates a certain number
        of points per curve.

        This is a lsightly different inplementation from the one above.
    */
    public List<Vector3> GetDrawingPoints1()
    {
        List<Vector3> drawingPoints = new List<Vector3>();

        for (int i = 0; i < controlPoints.Count - 3; i += 3)
        {
            Vector3 p0 = controlPoints[i];
            Vector3 p1 = controlPoints[i + 1];
            Vector3 p2 = controlPoints[i + 2];
            Vector3 p3 = controlPoints[i + 3];

            if (i == 0) //only do this for the first end point. When i != 0, this coincides with the end point of the previous segment,
            {
                drawingPoints.Add(CalculateBezierPoint(0, p0, p1, p2, p3));
            }

            for (int j = 1; j <= SEGMENTS_PER_CURVE; j++)
            {
                float t = j / (float)SEGMENTS_PER_CURVE;
                drawingPoints.Add(CalculateBezierPoint(t, p0, p1, p2, p3));
            }
        }

        return drawingPoints;
    }

    /**
        This gets the drawing points of a bezier curve, using recursive division,
        which results in less points for the same accuracy as the above implementation.
    */
    public List<Vector3> GetDrawingPoints2()
    {
        List<Vector3> drawingPoints = new List<Vector3>();

        for (int curveIndex = 0; curveIndex < curveCount; curveIndex++)
        {
            List<Vector3> bezierCurveDrawingPoints = FindDrawingPoints(curveIndex);

            if (curveIndex != 0)
            {
                //remove the fist point, as it coincides with the last point of the previous Bezier curve.
                bezierCurveDrawingPoints.RemoveAt(0);
            }

            drawingPoints.AddRange(bezierCurveDrawingPoints);
        }

        return drawingPoints;
    }

    List<Vector3> FindDrawingPoints(int curveIndex)
    {
        List<Vector3> pointList = new List<Vector3>();

        Vector3 left = CalculateBezierPoint(curveIndex, 0);
        Vector3 right = CalculateBezierPoint(curveIndex, 1);

        pointList.Add(left);
        pointList.Add(right);

        FindDrawingPoints(curveIndex, 0, 1, pointList, 1);

        return pointList;
    }
    
    
    /**
        @returns the number of points added.
    */
    int FindDrawingPoints(int curveIndex, float t0, float t1,
        List<Vector3> pointList, int insertionIndex)
    {
        Vector3 left = CalculateBezierPoint(curveIndex, t0);
        Vector3 right = CalculateBezierPoint(curveIndex, t1);

        if ((left - right).sqrMagnitude < MINIMUM_SQR_DISTANCE)
        {
            return 0;
        }

        float tMid = (t0 + t1) / 2;
        Vector3 mid = CalculateBezierPoint(curveIndex, tMid);

        Vector3 leftDirection = (left - mid).normalized;
        Vector3 rightDirection = (right - mid).normalized;

        if (Vector3.Dot(leftDirection, rightDirection) > DIVISION_THRESHOLD || Mathf.Abs(tMid - 0.5f) < 0.0001f)
        {
            int pointsAddedCount = 0;

            pointsAddedCount += FindDrawingPoints(curveIndex, t0, tMid, pointList, insertionIndex);
            pointList.Insert(insertionIndex + pointsAddedCount, mid);
            pointsAddedCount++;
            pointsAddedCount += FindDrawingPoints(curveIndex, tMid, t1, pointList, insertionIndex + pointsAddedCount);

            return pointsAddedCount;
        }

        return 0;
    }



    /**
        Caluclates a point on the Bezier curve represented with the four controlpoints given.
    */
    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0; //first term

        p += 3 * uu * t * p1; //second term
        p += 3 * u * tt * p2; //third term
        p += ttt * p3; //fourth term

        return p;

    }
}
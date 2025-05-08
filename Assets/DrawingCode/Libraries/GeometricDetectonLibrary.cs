/* 
 * mayra barrera, 2017
 * geometric relationships library
 * help find positions based on strokes, and identify strokes
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GeometricDetectionLibrary
{

    #region RelationshipIdentification

    public static float VectorValue(Vector3 position, Vector3 direction, float length, int axis)
    {
        Vector3 temp = position + direction * length;

        switch (axis)
        {
            case 1:
                return temp.x;
            case 2:
                return temp.y;
            case 3:
                return temp.z;
            default:
                return 0;
        }
    }

    //position in same line using direction
    public static Vector3 PointLine(Vector3 position, Vector3 direction, float length)
    {
        //equation: position + directionVector * t, where t is the change in position with respect to position
        return position + direction * length;
    }

    //position in same line using start - end of other line
    public static Vector3 PointLine(Vector3 position, Vector3 startPosition, Vector3 endPosition, float length)
    {
        //equation: position + directionVector * t, where t is the change in position with respect to position
        Vector3 directionVector = VectorMathsLibrary.DirectionBetweenPoints(startPosition, endPosition);
        return position + directionVector * length;
    }

    //position in perpendicular line using direction
    public static Vector3 PointPerpendicularLine(Vector3 position, Vector3 normal, Vector3 direction, float lenght)
    {
        //equation: cross between (end-start) and (end-control)
        Vector3 result = Vector3.Cross(normal, direction);

        if (result.magnitude == 0)
        {
            return Vector3.zero;
        }
        else
        {
            return position + result.normalized * lenght;
        }
    }

    //position in perpendicular line using start - end of other line
    public static Vector3 PointPerpendicularLine(Vector3 position, Vector3 startPosition, Vector3 endPosition)
    {
        //equation: cross between (end-start) and (end-control)
        Vector3 directionVector1 = VectorMathsLibrary.DirectionBetweenPoints(startPosition, endPosition);
        Vector3 directionVector2 = VectorMathsLibrary.DirectionBetweenPoints(position, endPosition);

        Vector3 result = Vector3.Cross(directionVector1, directionVector2);

        if(result.magnitude == 0)
        {
            return Vector3.zero;
        } else
        {
            return position + result;
        }
    }

    //tangent line to a circle
    public static Vector3 PointTangentLine(Vector3 circleCenter, Vector3 position, Vector3 normalSurface, float length)
    {
        //tangent perpendicular to radius
        //equation: get direction and normal, then do a cross product to get perpendicular to both vectors
        Vector3 directionVector = VectorMathsLibrary.DirectionBetweenPoints(circleCenter, position);
        Vector3 directionPerpendicular = Vector3.Cross(directionVector, normalSurface);

        return position + directionPerpendicular * length;
    }

    //point in circle offset from previous circle
    public static Vector3 PointCircleOffset(Vector3 center, float radius, float degrees, Vector3 surfaceVectorRight, Vector3 surfaceVectorUp)
    {
        float x = center.x
            + radius * Mathf.Sin(degrees * Mathf.Deg2Rad) * surfaceVectorRight.x
            + radius * Mathf.Cos(degrees * Mathf.Deg2Rad) * surfaceVectorUp.x;

        float y = center.y
            + radius * Mathf.Sin(degrees * Mathf.Deg2Rad) * surfaceVectorRight.y
            + radius * Mathf.Cos(degrees * Mathf.Deg2Rad) * surfaceVectorUp.y;

        float z = center.z
            + radius * Mathf.Sin(degrees * Mathf.Deg2Rad) * surfaceVectorRight.z
            + radius * Mathf.Cos(degrees * Mathf.Deg2Rad) * surfaceVectorUp.z;

        return new Vector3(x, y, z);
    }

    //perfect world direction from local directon
    public static Vector3 GetPerfectDirection(Vector3 position, Vector3 direction, Vector3 planePosition, Vector3 planeNormal)
    {
        Vector3 controllerRightPos = PointLine(position, direction, 1);

        Vector3 perfectRightPos = VectorMathsLibrary.ProjectPointToPlane(controllerRightPos, planeNormal, planePosition);

        Vector3 perfectDirection = VectorMathsLibrary.DirectionBetweenPoints(perfectRightPos, position);

        if(Vector3.Dot(direction, perfectDirection) < 0)
        {
            perfectDirection = perfectDirection * -1;
        }

        return perfectDirection;
    }

    #endregion

    #region shapeIdentification

    public static Vector4 IdentifyType(List<RibbonVertex> points)
    {
        //normal values
        List<float> curvatures = new List<float>();
        List<float> angles = new List<float>();
        List<Vector3> directions = new List<Vector3>();

        for (int i = 1; i < points.Count - 1; i++)
        {
            //Byungmoon processSingleInput code in c# and 3D

            // compute curvature vector by taking derivative of coord function at local frame; equivalent to FEM
            float dx01 = points[i].position.x - points[i - 1].position.x;
            float dy01 = points[i].position.y - points[i - 1].position.y;
            float dz01 = points[i].position.z - points[i - 1].position.z;
            float L01 = 1e-10f + Mathf.Sqrt(dx01 * dx01 + dy01 * dy01 + dz01 * dz01);

            float dx12 = points[i + 1].position.x - points[i - 1].position.x;
            float dy12 = points[i + 1].position.y - points[i - 1].position.y;
            float dz12 = points[i + 1].position.z - points[i - 1].position.z;
            float L12 = 1e-10f + Mathf.Sqrt(dx12 * dx12 + dy12 * dy12 +  dz12 * dz12);

            float kx = (dx12 / L12 - dx01 / L01) / ((L12 + L01) * 0.5f);
            float ky = (dy12 / L12 - dy01 / L01) / ((L12 + L01) * 0.5f);
            float kz = (dz12 / L12 - dz01 / L01) / ((L12 + L01) * 0.5f);
            curvatures.Add(Mathf.Sqrt(kx * kx + ky * ky + kz * kz));

            // compute steering angle
            Vector3 v01 =  new Vector3( dx01 / L01, dy01 / L01, dz01 / L01 );
            Vector3 v12 = new Vector3( dx12 / L12, dy12 / L12, dz12 / L12 );
            float sign = v01[0] * v12[1] - v01[1] * v12[0];
            angles.Add((sign > 0.0f ? 1f : -1f) * Mathf.Acos(Mathf.Min(1.0f, v01[0] * v12[0] + v01[1] * v12[1] + v01[2] * v12[2])));

            directions.Add(new Vector3(dx12,dy12,dz12));
        }

        float curvature_mean = 0;
        float angle_mean = 0;
        Vector3 direction_mean = new Vector3(0.0f, 0.0f, 0.0f );

        for (int i = 2; i < curvatures.Count; i++)
        {
            curvature_mean += curvatures[i];
            angle_mean += angles[i];
            direction_mean.x += directions[i].x;
            direction_mean.y += directions[i].y;
            direction_mean.z += directions[i].z;
        }
        curvature_mean /= curvatures.Count - 2f;
        angle_mean /= angles.Count - 2f;

        direction_mean.x /= directions.Count - 2f;
        direction_mean.y /= directions.Count - 2f;
        direction_mean.z /= directions.Count - 2f;
        float size_direction_mean = Mathf.Sqrt(direction_mean.x * direction_mean.x 
          + direction_mean.y * direction_mean.y + direction_mean.z *  direction_mean.z);

        float curvature_std = 0.0f;
        for (int i = 2; i < curvatures.Count; i++)
        {
            float d = (curvature_mean - curvatures[i]);
            curvature_std += d * d;
        }
        curvature_std = Mathf.Sqrt(curvature_std) / curvatures.Count - 2f;

        float angleZeroCrossRate = 0.0f;
        for (int i = 3; i < angles.Count; i++)
        {
            if (angles[i - 1] * angles[i] < 0.0f)
            {
                angleZeroCrossRate ++;
            }
        }
        angleZeroCrossRate /= angles.Count - 3f;

        Vector4 results = new Vector4(curvature_std, Mathf.Abs(angle_mean), angleZeroCrossRate,size_direction_mean);
        return results;
    }

    public static Vector3[] FindCircleValues(List<RibbonVertex> points)
    {
        List<Vector3> center = new List<Vector3>();

        for (int i = 0; i < points.Count - 2; i++)
        {
            Vector3 start = points[i].position;
            Vector3 middle = points[i + 1].position;
            Vector3 end = points[i + 2].position;

            Vector3 t = middle - start;
            Vector3 u = end - start;
            Vector3 v = end - middle;

            Vector3 w = Vector3.Cross(t, u);
            float wlength = w.sqrMagnitude;

            //if (wlength < 0.0001) return null;
            
            float iwsl2 = 1.0f / 2.0f * wlength;
            float tt = Vector3.Dot(t, t);
            float uu = Vector3.Dot(u, u);

            //center
            Vector3 centerPoint = start + (u * tt * Vector3.Dot(u, v) - t * uu * Vector3.Dot(t, v)) * iwsl2;
            center.Add(centerPoint);
        }

        Vector3[] results = new Vector3[3];

        //center
        Vector3 center_mean = Vector3.zero;
        for (int i = 0; i < center.Count; i++)
        {
            center_mean.x += center[i].x;
            center_mean.y += center[i].y;
            center_mean.z += center[i].z;
        }

        center_mean.x /= center.Count;
        center_mean.y /= center.Count;
        center_mean.z /= center.Count;
        results[0] = center_mean;
        //Debug.Log("center" + center_mean.x + "," + center_mean.y + "," + center_mean.z);

        //radius
        float radius_mean = 0;
        for (int i = 0; i < points.Count; i++)
        {
            radius_mean += VectorMathsLibrary.DistanceBetweenTwoPoints(points[i].position, center_mean);
        }

        radius_mean /= points.Count;
        results[1] = new Vector3(radius_mean, radius_mean, radius_mean);
        //Debug.Log("radius" + radius_mean);

        return results;
    }

    #endregion
}
 
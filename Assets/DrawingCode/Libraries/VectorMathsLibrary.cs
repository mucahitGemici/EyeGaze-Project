/* 
 * mayra barrera, 2017
 * vector math library
 * help identify the values between two vectors
 * 
 */


using UnityEngine;

public static class VectorMathsLibrary {

    //find middle point of two vectors
    public static Vector3 MiddlePoint(Vector3 start, Vector3 end)
    {
        float x = (start.x + end.x) / 2;
        float y = (start.y + end.y) / 2;
        float z = (start.z + end.z) / 2;

        return new Vector3(x, y, z);
    }

    //get normalized direction between two points
    public static Vector3 DirectionBetweenPoints(Vector3 pointFrom, Vector3 pointTo, bool signed = false)
    {
        Vector3 heading = pointFrom - pointTo;
        float magnitud = (signed) ? heading.magnitude : Mathf.Abs(heading.magnitude);

        return heading / magnitud;
    }

    //get distance between two points
    public static float DistanceBetweenTwoPoints(Vector3 pointFrom, Vector3 pointTo)
    {
        Vector3 heading = pointFrom - pointTo;
        return heading.magnitude;
    }

    //get distance between plane and point
    public static float DistacenBetweenPointAndPlane(Vector3 point, Vector3 planeCenter, Vector3 planeNormal)
    {
        //get v1 (vector between plane origin and point)
        Vector3 v1 = point - planeCenter;

        //distance between plane and v1
        float distance = Vector3.Dot(v1, planeNormal);
        return Mathf.Abs(distance);
    }

    //relationship between two planes (parallel, perpendicular, none)
    public static GlobalVars.geometricRelation RelationshipBetweenTwoPlanes(Vector3 planeANormal, Vector3 planeBNormal)
    {
        GlobalVars.geometricRelation relationship = GlobalVars.geometricRelation.none;

        //angles dots
        float angle = Mathf.Abs(Vector3.Dot(planeANormal, planeBNormal));

        if(angle > 0.90f)
        {
            relationship = GlobalVars.geometricRelation.parallelism;
        }

        if(angle < 0.15f)
        {
            relationship = GlobalVars.geometricRelation.perpendicularity;
        }

        if(angle >= 0.65 && angle < 0.75)
        {
            relationship = GlobalVars.geometricRelation.acute45;
        }

        return relationship;
    }

    //signed relationship between two planes (!!!this return a signed int that needs to be converted to unsigned before doing the parse to geometricRelationship!!!)
    public static  int SignedRelationshipBetweenTwoPlanes(Vector3 planeANormal, Vector3 planeBNormal)
    {
        int sign = 1;
        int relationship = 0;

        //get the separation between vectors
        float angle = Vector3.Dot(planeANormal, planeBNormal);

        //see if vectors are in opposite directions
        if (angle < 0){sign = -1;}

        //remove sign to calculate relationshp
        angle = Mathf.Abs(angle);

        if (angle > 0.90f)
        {
            relationship = (int) GlobalVars.geometricRelation.parallelism;
        }

        if (angle < 0.15f)
        {
            relationship = (int) GlobalVars.geometricRelation.perpendicularity;
        }

        if (angle >= 0.65 && angle < 0.75)
        {
            relationship = (int) GlobalVars.geometricRelation.acute45;
        }

        //multiply sign and relationship to get a signed relationship
        return relationship * sign;
    }

    //get angle between two planes
    public static float AngleBetweenTwoPlanes(Vector3 planeANormal, Vector3 planeBNormal)
    {

        float angle = Mathf.Acos(Vector3.Dot(planeANormal, planeBNormal) / planeANormal.sqrMagnitude * planeBNormal.sqrMagnitude);
        return angle * Mathf.Rad2Deg;
    }

    //angle from triangle (degrees)
    public static float AngleFromRightTriangle(int lookedAngle, Vector3 positionA, Vector3 positionB, Vector3 positionC)
    {
        float angle;

        float hypothenuse = DistanceBetweenTwoPoints(positionA, positionB);
        float opposite = DistanceBetweenTwoPoints(positionB, positionC);
        float adjacent = DistanceBetweenTwoPoints(positionA, positionC);

        float alfa = Mathf.Asin(opposite / hypothenuse) * Mathf.Rad2Deg;
        float beta = 180 - (90 + alfa);

        switch (lookedAngle)
        {
            case 1://for alfa
                angle = alfa;
                break;

            case 2://for beta
                angle = beta;
                break;

            default://for theta
                angle = 90;
                break;
        }

        return angle;
    }

    public static Vector3 NormalVectorPlane(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        Vector3 normal = Vector3.zero;

        Vector3 directionA = pointA - pointB;
        Vector3 directionB = pointA - pointC;

        normal = Vector3.Cross(directionA, directionB);

        return normal;
    }

    public static Vector3 ProjectPointToLine(Vector3 pointLineStart, Vector3 pointLineEnd, Vector3 point)
    {
		Vector3 pq = point - pointLineStart;
		Vector3 u = pointLineEnd - pointLineStart;
  		Vector3 result = pointLineStart + Vector3.Dot(pq, u)/Vector3.Dot(u, u) * u;

        return result;
    }

    public static Vector3 ProjectPointToPlane(Vector3 point, Vector3 planeNormal, Vector3 planePosition)
    {
        //get v1 (vector between plane origin and point)
        Vector3 v1 = point - planePosition;

        //distance between plane and v1
        float distance = Vector3.Dot(v1, planeNormal);

        //projectedPos (position + ( - normal * d))
        Vector3 projectedPos = point - planeNormal * distance;

        //get new 3D position
        return projectedPos;
    }

    public static bool PointOnPlane(Vector3 point, Vector3 planeNormal, Vector3 planePosition)
    {

        Vector3 v1 = point - planePosition;

        //distance between plane and v1
        float distance = Mathf.Abs(Vector3.Dot(v1, planeNormal));

        if (distance <= 0.01f)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public static Quaternion RotationFromThreeVectors(Vector3 localUp, Vector3 onPlaneRight, Vector3 onPlaneUp, Vector3 planeNormal)
    {
        //check if localUp is stranding (using world coordinate system)
        float angle2 = Mathf.Abs(Vector3.Dot(localUp, Vector3.up));

        //EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(5, t.position, Color.green, Vector3.up, 5));
        //EventManager.Instance.QueueEvent(new CreateRayWidgetEvent(6, t.position, Color.yellow, GlobalVars.Instance.surface.transform.up, 2));

        Quaternion newRotation;
        if (angle2 > 0.7f)
        {
            //world z always points towards onPlaneRight and world up depends on the controller rotation vs the world up
            newRotation = Quaternion.LookRotation(onPlaneRight, onPlaneUp);
        }
        else
        {
            newRotation = Quaternion.LookRotation(onPlaneRight, planeNormal);
        }

        return newRotation;
    }

    public static Quaternion QuaternionFromMatrix(Matrix4x4 m) {
        return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
    }
}

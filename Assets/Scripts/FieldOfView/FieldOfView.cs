using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float ViewRadius;

    [Range(0, 360)]
    public float ViewConeWidth;

    public LayerMask ObstacleMask;
    public float MeshResolution;
    public int EdgeResolveIterations;
    public float EdgeDistanceThreshold;
    public Vector3 DetectionOffset;

    public MeshFilter ViewMeshFilter;
    private Mesh _viewMesh;

    private JobHandle raycastHandle;

    private void Start()
    {
        _viewMesh = new Mesh();
        _viewMesh.name = "View Mesh";
        ViewMeshFilter.mesh = _viewMesh;
    }

    private void Update()
    {
        DrawFieldOfView();
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void DrawFieldOfView()
    {
        var viewPoints = CalculateViewPoints();

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i] - DetectionOffset);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        _viewMesh.Clear();
        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateNormals();
    }

    private float fogDistance = 12;
    private float fogHeight = 2;

    public void DrawFieldOfViewBig()
    {
        var viewPoints = CalculateViewPoints();

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[vertexCount * 12];

//        vertices[0] = Vector3.zero;
        for (int i = 2; i < vertexCount - 5; i++)
        {
            //a
            vertices[i] = transform.InverseTransformPoint(viewPoints[i]);
            Debug.DrawLine(transform.position, viewPoints[i], Color.red, 10f);
            //b
            vertices[i + 1] = transform.InverseTransformPoint(
                (viewPoints[i] + viewPoints[i].normalized * fogDistance));
            //c
            vertices[i + 2] = transform.InverseTransformPoint(
                (viewPoints[i - 1] + viewPoints[i + 1].normalized * fogDistance));
            //d
            vertices[i + 3] = transform.InverseTransformPoint(
                viewPoints[i - 1] - DetectionOffset);
            //e
            vertices[i + 4] = transform.InverseTransformPoint(
                (viewPoints[i] - Vector3.down * fogHeight));
            //f
            vertices[i + 5] = transform.InverseTransformPoint(
                (viewPoints[i - 1] + Vector3.down * fogHeight));


            if (i < vertexCount - 3)
            {
                //abc
                triangles[i * 12] = i;
                triangles[i * 12 + 1] = i + 1;
                triangles[i * 12 + 2] = i + 2;

                //acd
                triangles[i * 12 + 3] = i;
                triangles[i * 12 + 4] = i + 2;
                triangles[i * 12 + 5] = i + 3;

                //ade
                triangles[i * 12 + 6] = i;
                triangles[i * 12 + 7] = i + 3;
                triangles[i * 12 + 8] = i + 4;

                //aef
                triangles[i * 12 + 9] = i;
                triangles[i * 12 + 10] = i + 5;
                triangles[i * 12 + 11] = i + 5;
            }
        }

        _viewMesh.Clear();
        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateNormals();
    }

    // TODO: Implement RaycastCommand https://docs.unity3d.com/ScriptReference/RaycastCommand.html
    public List<Vector3> CalculateViewPoints()
    {
        int stepCount = Mathf.RoundToInt(ViewConeWidth * MeshResolution);
        float stepAngleSize = ViewConeWidth / stepCount;

        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - ViewConeWidth / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDistanceThresholdExceeded =
                    Mathf.Abs(oldViewCast.Distance - newViewCast.Distance) > EdgeDistanceThreshold;
                if (oldViewCast.Hit != newViewCast.Hit ||
                    (oldViewCast.Hit && newViewCast.Hit && edgeDistanceThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.PointA != Vector3.zero) viewPoints.Add(edge.PointA);
                    if (edge.PointB != Vector3.zero) viewPoints.Add(edge.PointB);
                }
            }

            viewPoints.Add(newViewCast.Point);
            oldViewCast = newViewCast;
        }

        return viewPoints;
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.Angle;
        float maxAngle = maxViewCast.Angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < EdgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDistanceThresholdExceeded =
                Mathf.Abs(minViewCast.Distance - newViewCast.Distance) > EdgeDistanceThreshold;
            if (newViewCast.Hit == minViewCast.Hit && !edgeDistanceThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.Point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.Point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirectionFromAngle(globalAngle, true);
        RaycastHit hit;


        if (Physics.Raycast((transform.position + DetectionOffset), dir, out hit, ViewRadius, ObstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, (transform.position + DetectionOffset) + dir * ViewRadius, ViewRadius,
                globalAngle);
        }
    }

    public struct EdgeInfo
    {
        public Vector3 PointA;
        public Vector3 PointB;

        public EdgeInfo(Vector3 pointA, Vector3 pointB)
        {
            PointA = pointA;
            PointB = pointB;
        }
    }

    public struct ViewCastInfo
    {
        public bool Hit;
        public Vector3 Point;
        public float Distance;
        public float Angle;

        public ViewCastInfo(bool hit, Vector3 point, float distance, float angle)
        {
            Hit = hit;
            Point = point;
            Distance = distance;
            Angle = angle;
        }
    }
}
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField]
    private float viewRadius;

    [Range(0, 360)]
    [SerializeField]
    private float viewConeWidth;

    [SerializeField]
    private LayerMask obstacleMask;

    [SerializeField]
    private float meshResolution;

    [SerializeField]
    private int edgeResolveIterations;

    [SerializeField]
    private float edgeDistanceThreshold;

    [SerializeField]
    private Vector3 detectionOffset;

    [SerializeField]
    private MeshFilter viewMeshFilter;

    [SerializeField]
    private float maskCutawayDistance = 0.1f;

    private Mesh viewMesh;

    private float fogDistance = 12;
    private float fogHeight = 2;

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
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
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i] - detectionOffset);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
    }


    // TODO: Implement RaycastCommand https://docs.unity3d.com/ScriptReference/RaycastCommand.html
    public List<Vector3> CalculateViewPoints()
    {
        int stepCount = Mathf.RoundToInt(viewConeWidth * meshResolution);
        float stepAngleSize = viewConeWidth / stepCount;

        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewConeWidth / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDistanceThresholdExceeded =
                    Mathf.Abs(oldViewCast.Distance - newViewCast.Distance) > edgeDistanceThreshold;
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

    private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.Angle;
        float maxAngle = maxViewCast.Angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDistanceThresholdExceeded =
                Mathf.Abs(minViewCast.Distance - newViewCast.Distance) > edgeDistanceThreshold;
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

    private ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirectionFromAngle(globalAngle, true);

        if (Physics.Raycast((transform.position + detectionOffset), dir, out var hit, viewRadius, obstacleMask))
        {
            // Applying maskCutawayDistance to make sure the walls remain visible.
            return new ViewCastInfo(true, hit.point + (-hit.normal * maskCutawayDistance), hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, (transform.position + detectionOffset) + dir * viewRadius, viewRadius,
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
using System;
using System.Linq;
using Boo.Lang;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class Connectable : MonoBehaviour
{
    [Tooltip("Add new Connectable Settings through Create>SS3D>Connectable Data")]
    public ConnectableSettings ConnectableSettings;

    [SerializeField]
    private AdjacentData adjacentData;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    [SerializeField]
    private ConnectableGroup connectableGroup;

    private Vector2Int position
    {
        get => new Vector2Int(Convert.ToInt16(transform.position.x), Convert.ToInt16(transform.position.z));
    }

    // Using Awake in edit mode so meshFilter and meshRenderer are set during edit mode too.
#if UNITY_EDITOR
    private void Awake()
    {
        if (!EditorApplication.isPlaying)
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
        }
    }
#endif

    private void Start()
    {
        if (!connectableGroup) connectableGroup = GetComponentInParent<ConnectableGroup>();
        GetAdjacentsOnGrid();
    }

    public void Construct(bool updateGrid)
    {
        if (!connectableGroup) connectableGroup = GetComponentInParent<ConnectableGroup>();
        if (connectableGroup)
        {
            if (updateGrid) connectableGroup.UpdateGridContent();

            Connect();
            if (adjacentData.NorthWest) adjacentData.NorthWest.Connect();
            if (adjacentData.North) adjacentData.North.Connect();
            if (adjacentData.NorthEast) adjacentData.NorthEast.Connect();
            if (adjacentData.East) adjacentData.East.Connect();
            if (adjacentData.SouthEast) adjacentData.SouthEast.Connect();
            if (adjacentData.South) adjacentData.South.Connect();
            if (adjacentData.SouthWest) adjacentData.SouthWest.Connect();
            if (adjacentData.West) adjacentData.West.Connect();
        }
    }

    private void Reset()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // method of connecting, works, but UGLY and long.
    public void Connect()
    {
        GetAdjacentsOnGrid();

        switch (ConnectableSettings.connectableType)
        {
            // Only rotating (Doors for example)
            case ConnectableSettings.ConnectableType.OnlyConnect:
                switch (adjacentData.directions & ~(Directions.NorthEast | Directions.SouthEast |
                                                    Directions.SouthWest | Directions.NorthWest))
                {
                    case Directions.North | Directions.South:
                        transform.eulerAngles = new Vector3(0, 90, 0);
                        break;
                    case Directions.East | Directions.West:
                        transform.eulerAngles = new Vector3(0, 0, 0);
                        break;
                }

                break;

            // Material Connections
            case ConnectableSettings.ConnectableType.MaterialBased:
                switch (adjacentData.directions)
                {
                    case Directions.None:
                        UpdateMaterialAndRotation(ConnectableSettings.NoColorMaterial, 0);
                        break;

                    // North
                    case Directions.North:
                    case Directions.North | Directions.NorthEast:
                    case Directions.North | Directions.NorthWest:
                    case Directions.North | Directions.NorthEast | Directions.NorthWest:
                        UpdateMaterialAndRotation(ConnectableSettings.DoubleColorMaterial, 270);
                        break;

                    // East
                    case Directions.East:
                    case Directions.East | Directions.NorthEast:
                    case Directions.East | Directions.SouthEast:
                    case Directions.East | Directions.NorthEast | Directions.SouthEast:
                        UpdateMaterialAndRotation(ConnectableSettings.DoubleColorMaterial, 0);
                        break;

                    // South
                    case Directions.South:
                    case Directions.South | Directions.SouthEast:
                    case Directions.South | Directions.SouthWest:
                    case Directions.South | Directions.SouthEast | Directions.SouthWest:
                        UpdateMaterialAndRotation(ConnectableSettings.DoubleColorMaterial, 90);
                        break;

                    // West
                    case Directions.West:
                    case Directions.West | Directions.NorthWest:
                    case Directions.West | Directions.SouthWest:
                    case Directions.West | Directions.NorthWest | Directions.SouthWest:
                        UpdateMaterialAndRotation(ConnectableSettings.DoubleColorMaterial, 180);
                        break;

                    // North East Outer
                    case Directions.North | Directions.NorthEast | Directions.East:
                    case Directions.North | Directions.NorthEast | Directions.East | Directions.NorthWest:
                    case Directions.North | Directions.NorthEast | Directions.East | Directions.SouthEast:
                    case Directions.North | Directions.NorthEast | Directions.East | Directions.SouthEast |
                         Directions.NorthWest:
                    case Directions.NorthEast | Directions.East | Directions.SouthEast | Directions.NorthWest:
                    case Directions.North | Directions.NorthEast | Directions.SouthEast | Directions.NorthWest:
                        UpdateMaterialAndRotation(ConnectableSettings.TriColorMaterial, 90);
                        break;

                    // South East Outer
                    case Directions.South | Directions.SouthEast | Directions.East:
                    case Directions.South | Directions.SouthEast | Directions.East | Directions.NorthEast:
                    case Directions.South | Directions.SouthEast | Directions.East | Directions.SouthWest:
                    case Directions.South | Directions.SouthEast | Directions.East | Directions.SouthWest |
                         Directions.NorthEast:
                    case Directions.SouthEast | Directions.East | Directions.SouthWest | Directions.NorthEast:
                    case Directions.South | Directions.SouthEast | Directions.SouthWest | Directions.NorthEast:
                        UpdateMaterialAndRotation(ConnectableSettings.TriColorMaterial, 180);
                        break;

                    // North West Outer
                    case Directions.NorthEast | Directions.West | Directions.NorthWest:
                    case Directions.North | Directions.NorthWest | Directions.West:
                    case Directions.North | Directions.NorthWest | Directions.West | Directions.SouthWest:
                    case Directions.North | Directions.NorthWest | Directions.West | Directions.NorthEast:
                    case Directions.North | Directions.NorthWest | Directions.West | Directions.NorthEast |
                         Directions.SouthWest:
                    case Directions.NorthWest | Directions.West | Directions.NorthEast | Directions.SouthWest:
                    case Directions.North | Directions.NorthWest | Directions.NorthEast | Directions.SouthWest:
                        UpdateMaterialAndRotation(ConnectableSettings.TriColorMaterial, 0);
                        break;

                    // South West Outer
                    case Directions.South | Directions.SouthWest | Directions.West:
                    case Directions.South | Directions.SouthWest | Directions.West | Directions.SouthEast:
                    case Directions.South | Directions.SouthWest | Directions.West | Directions.NorthWest:
                    case Directions.South | Directions.SouthWest | Directions.West | Directions.NorthWest |
                         Directions.SouthEast:
                    case Directions.SouthWest | Directions.West | Directions.NorthWest | Directions.SouthEast:
                    case Directions.South | Directions.SouthWest | Directions.NorthWest | Directions.SouthEast:
                    case Directions.SouthEast | Directions.SouthWest | Directions.West:
                        UpdateMaterialAndRotation(ConnectableSettings.TriColorMaterial, 270);
                        break;

                    // North East Inner
                    case Directions.NorthEast:
                        UpdateMaterialAndRotation(ConnectableSettings.SingleColorMaterial, 180);
                        break;

                    // South East Inner
                    case Directions.SouthEast:
                        UpdateMaterialAndRotation(ConnectableSettings.SingleColorMaterial, 270);
                        break;

                    // South West Inner
                    case Directions.SouthWest:
                        UpdateMaterialAndRotation(ConnectableSettings.SingleColorMaterial, 0);
                        break;
                    // North West Inner
                    case Directions.NorthWest:
                        UpdateMaterialAndRotation(ConnectableSettings.SingleColorMaterial, 90);
                        break;

                    // Split NorthEast SouthEast
                    case Directions.NorthEast | Directions.SouthEast:
                        UpdateMaterialAndRotation(ConnectableSettings.DoubleColorMaterial, 0);
                        break;

                    // Split NorthEast SouthEast
                    case Directions.SouthEast | Directions.SouthWest:
                        UpdateMaterialAndRotation(ConnectableSettings.DoubleColorMaterial, 90);
                        break;

                    // Split NorthEast SouthEast
                    case Directions.SouthWest | Directions.NorthWest:
                        UpdateMaterialAndRotation(ConnectableSettings.DoubleColorMaterial, 180);
                        break;

                    // Split NorthEast SouthEast
                    case Directions.NorthWest | Directions.NorthEast:
                        UpdateMaterialAndRotation(ConnectableSettings.DoubleColorMaterial, 270);
                        break;

                    // Quad Cases
                    case Directions.North | Directions.South:
                    case Directions.North | Directions.South | Directions.NorthEast:
                    case Directions.North | Directions.South | Directions.SouthEast:
                    case Directions.North | Directions.South | Directions.SouthWest:
                    case Directions.North | Directions.South | Directions.NorthWest:
                    case Directions.North | Directions.South | Directions.NorthEast | Directions.SouthEast:
                    case Directions.North | Directions.South | Directions.NorthEast | Directions.SouthEast |
                         Directions.NorthWest:
                    case Directions.North | Directions.South | Directions.NorthEast | Directions.SouthEast |
                         Directions.SouthWest:
                    case Directions.North | Directions.South | Directions.NorthEast | Directions.SouthEast |
                         Directions.East:
                    case Directions.North | Directions.South | Directions.NorthWest | Directions.SouthWest:
                    case Directions.North | Directions.South | Directions.NorthWest | Directions.SouthWest |
                         Directions.NorthEast:
                    case Directions.North | Directions.South | Directions.NorthWest | Directions.SouthWest |
                         Directions.SouthEast:
                    case Directions.North | Directions.South | Directions.NorthWest | Directions.SouthWest |
                         Directions.West:
                    case Directions.North | Directions.East | Directions.West | Directions.NorthEast |
                         Directions.SouthWest | Directions.NorthWest:
                    case Directions.East | Directions.South | Directions.West | Directions.NorthEast |
                         Directions.SouthEast | Directions.SouthWest:
                    case Directions.North | Directions.East | Directions.South | Directions.NorthEast |
                         Directions.SouthEast | Directions.NorthWest:
                    case Directions.North | Directions.South | Directions.West | Directions.SouthEast |
                         Directions.SouthWest | Directions.NorthWest:
                    case Directions.East | Directions.West:
                    case Directions.East | Directions.West | Directions.NorthEast:
                    case Directions.East | Directions.SouthEast | Directions.SouthWest:
                    case Directions.North | Directions.NorthEast | Directions.SouthEast:
                    case Directions.East | Directions.SouthEast | Directions.West:
                    case Directions.East | Directions.West | Directions.SouthWest:
                    case Directions.East | Directions.West | Directions.NorthWest:
                    case Directions.East | Directions.West | Directions.NorthEast | Directions.NorthWest:
                    case Directions.East | Directions.West | Directions.NorthEast | Directions.NorthWest |
                         Directions.SouthEast:
                    case Directions.East | Directions.West | Directions.NorthEast | Directions.NorthWest |
                         Directions.SouthWest:
                    case Directions.East | Directions.West | Directions.NorthEast | Directions.NorthWest |
                         Directions.North:
                    case Directions.East | Directions.West | Directions.SouthEast | Directions.SouthWest:
                    case Directions.East | Directions.West | Directions.SouthEast | Directions.SouthWest |
                         Directions.NorthEast:
                    case Directions.East | Directions.West | Directions.SouthEast | Directions.SouthWest |
                         Directions.NorthWest:
                    case Directions.East | Directions.West | Directions.SouthEast | Directions.SouthWest |
                         Directions.South:
                    case Directions.North | Directions.South | Directions.West | Directions.NorthEast |
                         Directions.SouthEast | Directions.SouthWest | Directions.NorthWest:
                    case Directions.East | Directions.South | Directions.West | Directions.NorthEast |
                         Directions.SouthEast | Directions.SouthWest | Directions.NorthWest:
                    case Directions.North | Directions.East | Directions.West | Directions.NorthEast |
                         Directions.SouthEast | Directions.SouthWest | Directions.NorthWest:
                    case Directions.East | Directions.West | Directions.NorthEast | Directions.SouthEast |
                         Directions.SouthWest | Directions.NorthWest:
                    case Directions.North | Directions.East | Directions.South | Directions.NorthEast |
                         Directions.SouthEast | Directions.SouthWest | Directions.NorthWest:
                    case Directions.North | Directions.South | Directions.NorthEast | Directions.SouthEast |
                         Directions.SouthWest | Directions.NorthWest:
                    case Directions.NorthWest | Directions.NorthEast | Directions.South | Directions.SouthWest |
                         Directions.West:
                    case Directions.North | Directions.NorthEast | Directions.East | Directions.SouthEast |
                         Directions.SouthWest | Directions.NorthWest:
                    case Directions.NorthEast | Directions.East | Directions.SouthEast | Directions.West:
                    case Directions.NorthWest | Directions.North | Directions.NorthEast | Directions.SouthEast |
                         Directions.SouthWest:
                    case Directions.NorthEast | Directions.East | Directions.SouthWest | Directions.West:
                    case Directions.NorthWest | Directions.North | Directions.NorthEast | Directions.South:
                    case Directions.North | Directions.NorthEast | Directions.East | Directions.SouthEast |
                         Directions.SouthWest:
                        UpdateMaterialAndRotation(ConnectableSettings.QuadColorMaterial, 0);
                        break;

                    default:

                        // Woops! the surrounding tile combination you encountered hasn't been added to the switch yet, do so in the cases above.
                        Debug.LogWarning(
                            $"{gameObject.name}: No fitting connection setup yet. Please paste \"case Directions.{adjacentData.directions.ToString().Replace(", ", " | Directions.")}:\" where it should be in Connectable.cs");
                        break;
                }

                break;

            // Connecting meshes
            case ConnectableSettings.ConnectableType.MeshBased:
                switch (adjacentData.directions & ~(Directions.NorthEast | Directions.SouthEast |
                                                    Directions.SouthWest | Directions.NorthWest))
                {
                    // North Orientation
                    case Directions.North:
                        UpdateMeshAndRotation(ConnectableSettings.EndMesh,
                            ConnectableSettings.EndRotationOffset + new Vector3(0, 0, 90));
                        break;
                    // East Orientation
                    case Directions.East:
                        UpdateMeshAndRotation(ConnectableSettings.EndMesh,
                            ConnectableSettings.EndRotationOffset + new Vector3(0, 0, 180));
                        break;
                    // South Orientation
                    case Directions.South:
                        UpdateMeshAndRotation(ConnectableSettings.EndMesh,
                            ConnectableSettings.EndRotationOffset + new Vector3(0, 0, 270));
                        break;
                    // West Orientation
                    case Directions.West:
                        UpdateMeshAndRotation(ConnectableSettings.EndMesh,
                            ConnectableSettings.EndRotationOffset + new Vector3(0, 0, 0));
                        break;

                    // North South Orientation
                    case Directions.North | Directions.South:
                        UpdateMeshAndRotation(ConnectableSettings.StraightMesh,
                            ConnectableSettings.StraightRotationOffset + new Vector3(0, 0, 90));
                        break;
                    case Directions.East | Directions.West:
                        UpdateMeshAndRotation(ConnectableSettings.StraightMesh,
                            ConnectableSettings.StraightRotationOffset + new Vector3(0, 0, 0));
                        break;


                    // North East Orientation
                    case Directions.North | Directions.East:
                        UpdateMeshAndRotation(ConnectableSettings.CornerMesh,
                            ConnectableSettings.CornerRotationOffset + new Vector3(0, 0, 0));
                        break;
                    // East South Orientation
                    case Directions.East | Directions.South:
                        UpdateMeshAndRotation(ConnectableSettings.CornerMesh,
                            ConnectableSettings.CornerRotationOffset + new Vector3(0, 0, 90));
                        break;
                    // South West Orientation
                    case Directions.South | Directions.West:
                        UpdateMeshAndRotation(ConnectableSettings.CornerMesh,
                            ConnectableSettings.CornerRotationOffset + new Vector3(0, 0, 180));
                        break;
                    // North West Orientation
                    case Directions.North | Directions.West:
                        UpdateMeshAndRotation(ConnectableSettings.CornerMesh,
                            ConnectableSettings.CornerRotationOffset + new Vector3(0, 0, 270));
                        break;

                    // North East South Orientation
                    case Directions.North | Directions.East | Directions.South:
                        UpdateMeshAndRotation(ConnectableSettings.SplitMesh,
                            ConnectableSettings.SplitRotationOffset + new Vector3(0, 0, 180));
                        break;
                    // East South West Orientation
                    case Directions.East | Directions.South | Directions.West:
                        UpdateMeshAndRotation(ConnectableSettings.SplitMesh,
                            ConnectableSettings.SplitRotationOffset + new Vector3(0, 0, 270));
                        break;
                    // North South West Orientation
                    case Directions.North | Directions.South | Directions.West:
                        UpdateMeshAndRotation(ConnectableSettings.SplitMesh,
                            ConnectableSettings.SplitRotationOffset + new Vector3(0, 0, 0));
                        break;
                    // North East West Orientation
                    case Directions.North | Directions.East | Directions.West:
                        UpdateMeshAndRotation(ConnectableSettings.SplitMesh,
                            ConnectableSettings.SplitRotationOffset + new Vector3(0, 0, 90));
                        break;

                    // North East South West Orientation
                    case Directions.North | Directions.East | Directions.South |
                         Directions.West:
                        UpdateMeshAndRotation(ConnectableSettings.CrossMesh, ConnectableSettings.SplitRotationOffset);
                        break;

                    default:
                        Debug.LogWarning($"{gameObject.name}: No fitting connections for: {adjacentData.directions}");
                        break;
                }

                break;
        }
    }

    private void UpdateMeshAndRotation(Mesh mesh, Vector3 rotation)
    {
        meshFilter.sharedMesh = mesh;
        transform.eulerAngles = rotation;
    }

    private void UpdateMaterialAndRotation(Material material, float rotation)
    {
        meshRenderer.sharedMaterial = material;
        transform.eulerAngles = new Vector3(90, 0, -rotation);
    }

    public void GetAdjacentsOnGrid()
    {
        if (!connectableGroup) connectableGroup = GetComponentInParent<ConnectableGroup>();
        if (connectableGroup.Grid.Length < 1) connectableGroup.UpdateGridContent();

        if (!connectableGroup)
        {
            Debug.LogWarning("No ConnectableGroup parent found");
        }

        adjacentData.directions = Directions.None;
        Connectable adjacent;

        adjacent = connectableGroup.GetConnectableAtPosition(position + new Vector2Int(-1, 1), ConnectableSettings);
        adjacentData.NorthWest = connectableGroup.GetConnectableAtPosition(position + new Vector2Int(-1, 1));
        if (adjacent)
        {
            adjacentData.directions |= Directions.NorthWest;
            adjacentData.NorthWest = adjacent;
        }


        adjacent = connectableGroup.GetConnectableAtPosition(position + new Vector2Int(0, 1), ConnectableSettings);
        adjacentData.North = connectableGroup.GetConnectableAtPosition(position + new Vector2Int(0, 1));

        if (adjacent)
        {
            adjacentData.directions |= Directions.North;
            adjacentData.North = adjacent;
        }


        adjacent = connectableGroup.GetConnectableAtPosition(position + new Vector2Int(1, 1), ConnectableSettings);
        adjacentData.NorthEast = connectableGroup.GetConnectableAtPosition(position + new Vector2Int(1, 1));

        if (adjacent)
        {
            adjacentData.directions |= Directions.NorthEast;
            adjacentData.NorthEast = adjacent;
        }


        adjacent = connectableGroup.GetConnectableAtPosition(position + new Vector2Int(1, 0), ConnectableSettings);
        adjacentData.East = connectableGroup.GetConnectableAtPosition(position + new Vector2Int(1, 0));

        if (adjacent)
        {
            adjacentData.directions |= Directions.East;
            adjacentData.East = adjacent;
        }


        adjacent = connectableGroup.GetConnectableAtPosition(position + new Vector2Int(1, -1), ConnectableSettings);
        adjacentData.SouthEast = connectableGroup.GetConnectableAtPosition(position + new Vector2Int(1, -1));

        if (adjacent)
        {
            adjacentData.directions |= Directions.SouthEast;
            adjacentData.SouthEast = adjacent;
        }


        adjacent = connectableGroup.GetConnectableAtPosition(position + new Vector2Int(0, -1), ConnectableSettings);
        adjacentData.South = connectableGroup.GetConnectableAtPosition(position + new Vector2Int(0, -1));

        if (adjacent)
        {
            adjacentData.directions |= Directions.South;
            adjacentData.South = adjacent;
        }


        adjacent = connectableGroup.GetConnectableAtPosition(position + new Vector2Int(-1, -1), ConnectableSettings);
        adjacentData.SouthWest = connectableGroup.GetConnectableAtPosition(position + new Vector2Int(-1, -1));

        if (adjacent)
        {
            adjacentData.directions |= Directions.SouthWest;
            adjacentData.SouthWest = adjacent;
        }


        adjacent = connectableGroup.GetConnectableAtPosition(position + new Vector2Int(-1, 0), ConnectableSettings);
        adjacentData.West = connectableGroup.GetConnectableAtPosition(position + new Vector2Int(-1, 0));

        if (adjacent)
        {
            adjacentData.directions |= Directions.West;
            adjacentData.West = adjacent;
        }
    }
}
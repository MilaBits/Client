using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Connectable : MonoBehaviour
{
    [Tooltip("Add new Connectable Settings through Create>SS3D>Connectable Data")]
    public ConnectableSettings ConnectableSettings;

    [SerializeField]
    private AdjacentData adjacents;

    [SerializeField]
    private MeshFilter meshFilter;

    [SerializeField]
    private MeshRenderer meshRenderer;

    private void Start()
    {
        if (ConnectableSettings.connectableType == ConnectableSettings.ConnectableType.MaterialBased)
            UpdateMaterialAndRotation(ConnectableSettings.NoColorMaterial, 0);
        Connect();
    }

    private void Reset()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Connect()
    {
        adjacents = GetAdjacents();

        switch (ConnectableSettings.connectableType)
        {
            // Only rotating (Doors for example)
            case ConnectableSettings.ConnectableType.OnlyConnect:
                switch (adjacents.Connections)
                {
                    case Directions.North | Directions.South:
                        transform.eulerAngles = new Vector3(0, 90, 0);
                        break;
                    case Directions.East | Directions.West:
                        transform.eulerAngles = new Vector3(0, 0, 0);
                        break;
                }

                break;

            // Tile Connections
            case ConnectableSettings.ConnectableType.MaterialBased:
                switch (adjacents.Connections)
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
                        UpdateMaterialAndRotation(ConnectableSettings.QuadColorMaterial, 0);
                        break;

                    default:

                        // Woops! the surrounding tile combination you encountered hasn't been added to the switch yet, do so in the cases above.
                        Debug.LogWarning(
                            $"{gameObject.name}: No fitting connections for: {adjacents.Connections.ToString().Replace(',', '|')}");
                        break;
                }

                break;

            // Wall Connections (probably also tables and such?)
            // TODO: Try tables
            case ConnectableSettings.ConnectableType.MeshBased:
                switch (adjacents.Connections)
                {
                    // One connection
                    case Directions.North:
                        UpdateMeshAndRotation(ConnectableSettings.EndMesh,
                            ConnectableSettings.EndRotationOffset + new Vector3(0, 0, 90));
                        break;
                    case Directions.East:
                        UpdateMeshAndRotation(ConnectableSettings.EndMesh,
                            ConnectableSettings.EndRotationOffset + new Vector3(0, 0, 180));
                        break;
                    case Directions.South:
                        UpdateMeshAndRotation(ConnectableSettings.EndMesh,
                            ConnectableSettings.EndRotationOffset + new Vector3(0, 0, 270));
                        break;
                    case Directions.West:
                        UpdateMeshAndRotation(ConnectableSettings.EndMesh,
                            ConnectableSettings.EndRotationOffset + new Vector3(0, 0, 0));
                        break;
                    // Two connections
                    // Straight
                    case Directions.North | Directions.South:
                        UpdateMeshAndRotation(ConnectableSettings.StraightMesh,
                            ConnectableSettings.StraightRotationOffset + new Vector3(0, 0, 90));
                        break;
                    case Directions.East | Directions.West:
                        UpdateMeshAndRotation(ConnectableSettings.StraightMesh,
                            ConnectableSettings.StraightRotationOffset + new Vector3(0, 0, 0));
                        break;
                    // Corner
                    case Directions.North | Directions.East:
                        UpdateMeshAndRotation(ConnectableSettings.CornerMesh,
                            ConnectableSettings.CornerRotationOffset + new Vector3(0, 0, 0));
                        break;
                    case Directions.East | Directions.South:
                        UpdateMeshAndRotation(ConnectableSettings.CornerMesh,
                            ConnectableSettings.CornerRotationOffset + new Vector3(0, 0, 90));
                        break;
                    case Directions.South | Directions.West:
                        UpdateMeshAndRotation(ConnectableSettings.CornerMesh,
                            ConnectableSettings.CornerRotationOffset + new Vector3(0, 0, 180));
                        break;
                    case Directions.West | Directions.North:
                        UpdateMeshAndRotation(ConnectableSettings.CornerMesh,
                            ConnectableSettings.CornerRotationOffset + new Vector3(0, 0, 270));
                        break;

                    // Three connections
                    case Directions.North | Directions.East | Directions.South:
                        UpdateMeshAndRotation(ConnectableSettings.SplitMesh,
                            ConnectableSettings.SplitRotationOffset + new Vector3(0, 0, 180));
                        break;
                    case Directions.East | Directions.South | Directions.West:
                        UpdateMeshAndRotation(ConnectableSettings.SplitMesh,
                            ConnectableSettings.SplitRotationOffset + new Vector3(0, 0, 270));
                        break;
                    case Directions.South | Directions.West | Directions.North:
                        UpdateMeshAndRotation(ConnectableSettings.SplitMesh,
                            ConnectableSettings.SplitRotationOffset + new Vector3(0, 0, 0));
                        break;
                    case Directions.West | Directions.North | Directions.East:
                        UpdateMeshAndRotation(ConnectableSettings.SplitMesh,
                            ConnectableSettings.SplitRotationOffset + new Vector3(0, 0, 90));
                        break;

                    // Four connections
                    case Directions.North | Directions.East | Directions.South |
                         Directions.West:
                        UpdateMeshAndRotation(ConnectableSettings.CrossMesh, ConnectableSettings.SplitRotationOffset);
                        break;

                    default:
                        Debug.LogWarning($"{gameObject.name}: No fitting connections for: {adjacents.Connections}");
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

    private AdjacentData GetAdjacents()
    {
        Ray ray = new Ray();
        AdjacentData adjacentData = new AdjacentData();
        Vector3 pos = transform.position;
        Physics.Raycast(pos + new Vector3(0, -.5f, 1), Vector3.up, out var northHit, 1f);
        Physics.Raycast(pos + new Vector3(1, -.5f, 0), Vector3.up, out var eastHit, 1f);
        Physics.Raycast(pos + new Vector3(0, -.5f, -1), Vector3.up, out var southHit, 1f);
        Physics.Raycast(pos + new Vector3(-1, -.5f, 0), Vector3.up, out var westHit, 1f);
        if (ConnectableSettings.connectableType == ConnectableSettings.ConnectableType.MaterialBased)
        {
            Physics.Raycast(pos + new Vector3(1, -.5f, 1), Vector3.up, out var northEastHit, 1f);
            Physics.Raycast(pos + new Vector3(1, -.5f, -1), Vector3.up, out var southEastHit, 1f);
            Physics.Raycast(pos + new Vector3(-1, -.5f, -1), Vector3.up, out var southWestHit, 1f);
            Physics.Raycast(pos + new Vector3(-1, -.5f, 1), Vector3.up, out var northWestHit, 1f);

            if (northEastHit.collider != null)
            {
                GameObject adjacent = northEastHit.collider.gameObject;
                if (northEastHit.collider.attachedRigidbody)
                    adjacent = northEastHit.collider.attachedRigidbody.gameObject;
                if (adjacent.GetComponent<Connectable>())
                {
                    adjacentData.Connections |= Directions.NorthEast;
                    adjacentData.NorthEast = adjacent.GetComponent<Connectable>();
                }
            }

            if (southEastHit.collider != null)
            {
                GameObject adjacent = southEastHit.collider.gameObject;
                if (southEastHit.collider.attachedRigidbody)
                    adjacent = southEastHit.collider.attachedRigidbody.gameObject;
                if (southEastHit.collider.GetComponent<Connectable>())
                {
                    adjacentData.Connections |= Directions.SouthEast;
                    adjacentData.SouthEast = adjacent.GetComponent<Connectable>();
                }
            }

            if (southWestHit.collider != null)
            {
                GameObject adjacent = southWestHit.collider.gameObject;
                if (southWestHit.collider.attachedRigidbody)
                    adjacent = southWestHit.collider.attachedRigidbody.gameObject;
                if (southWestHit.collider.GetComponent<Connectable>())
                {
                    adjacentData.Connections |= Directions.SouthWest;
                    adjacentData.SouthEast = adjacent.GetComponent<Connectable>();
                }
            }

            if (northWestHit.collider != null)
            {
                GameObject adjacent = northWestHit.collider.gameObject;
                if (northWestHit.collider.attachedRigidbody)
                    adjacent = northWestHit.collider.attachedRigidbody.gameObject;
                if (northWestHit.collider.GetComponent<Connectable>())
                {
                    adjacentData.Connections |= Directions.NorthWest;
                    adjacentData.SouthEast = adjacent.GetComponent<Connectable>();
                }
            }
        }

        if (northHit.collider != null)
        {
            GameObject adjacent = northHit.collider.gameObject;
            if (northHit.collider.attachedRigidbody) adjacent = northHit.collider.attachedRigidbody.gameObject;
            if (adjacent.GetComponent<Connectable>())
            {

                Connectable connectable = adjacent.GetComponent<Connectable>();
                if (ConnectableSettings.ConnectsWith.Any(x => x.name == connectable.ConnectableSettings.name))
                {
                    adjacentData.Connections |= Directions.North;
                    adjacentData.North = adjacent.GetComponent<Connectable>();
                }
            }
        }

        if (eastHit.collider != null)
        {
            GameObject adjacent = eastHit.collider.gameObject;
            if (eastHit.collider.attachedRigidbody) adjacent = eastHit.collider.attachedRigidbody.gameObject;
            if (adjacent.GetComponent<Connectable>())
            {
                Connectable connectable = adjacent.GetComponent<Connectable>();
                if (ConnectableSettings.ConnectsWith.Any(x => x.name == connectable.ConnectableSettings.name))
                {
                    adjacentData.Connections |= Directions.East;
                    adjacentData.East = adjacent.GetComponent<Connectable>();
                }
            }
        }

        if (southHit.collider != null)
        {
            GameObject adjacent = southHit.collider.gameObject;
            if (southHit.collider.attachedRigidbody) adjacent = southHit.collider.attachedRigidbody.gameObject;
            if (adjacent.GetComponent<Connectable>())
            {
                Connectable connectable = adjacent.GetComponent<Connectable>();
                if (ConnectableSettings.ConnectsWith.Any(x => x.name == connectable.ConnectableSettings.name))
                {
                    adjacentData.Connections |= Directions.South;
                    adjacentData.South = adjacent.GetComponent<Connectable>();
                }
            }
        }

        if (westHit.collider != null)
        {
            GameObject adjacent = westHit.collider.gameObject;
            if (westHit.collider.attachedRigidbody) adjacent = westHit.collider.attachedRigidbody.gameObject;
            if (adjacent.GetComponent<Connectable>())
            {
                Connectable connectable = adjacent.GetComponent<Connectable>();
                if (ConnectableSettings.ConnectsWith.Any(x => x.name == connectable.ConnectableSettings.name))
                {
                    adjacentData.Connections |= Directions.West;
                    adjacentData.West = adjacent.GetComponent<Connectable>();
                }
            }
        }

        return adjacentData;
    }
}
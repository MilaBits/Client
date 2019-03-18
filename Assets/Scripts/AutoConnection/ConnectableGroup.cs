using System;
using System.Linq;
using UnityEngine;

public class ConnectableGroup : MonoBehaviour
{
    [SerializeField]
    private Vector2Int gridSize;

    public Connectable[,] Grid = new Connectable[0, 0];

    public Vector2Int Size => new Vector2Int(Grid.GetLength(0), Grid.GetLength(1));

    public void UpdateGridContent()
    {
        UpdateGridSize();

        for (int x = 0; x < Grid.GetLength(0); x++)
        {
            for (int z = 0; z < Grid.GetLength(1); z++)
            {
                Physics.Raycast(new Vector3(x, -.5f, z), Vector3.up, out var hit, 1f);
                if (hit.collider != null)
                {
                    Grid[x, z] = hit.collider.GetComponent<Connectable>();
                }
                else if (hit.collider != null && hit.collider.attachedRigidbody)
                {
                    Grid[x, z] = hit.collider.attachedRigidbody.GetComponent<Connectable>();
                }
            }
        }
    }

    public Connectable GetConnectableAtPosition(int x, int y)
    {
        return GetConnectableAtPosition(new Vector2Int(x, y));
    }

    public Connectable GetConnectableAtPosition(Vector2Int position, ConnectableSettings connectableSettings)
    {
        // Position is out of range
        if (position.x < 0 || position.x >= Grid.GetLength(0) ||
            position.y < 0 || position.y >= Grid.GetLength(1)) return null;

        Connectable connectable = Grid[position.x, position.y];
        if (connectable && connectableSettings.ConnectsWith.Any(x => x.name == connectable.ConnectableSettings.name))
        {
            return connectable;
        }

        // There is no tile at position
        return null;
    }

    public Connectable GetConnectableAtPosition(Vector2Int position)
    {
        // Position is out of range
        if (position.x < 0 || position.x >= Grid.GetLength(0) ||
            position.y < 0 || position.y >= Grid.GetLength(1)) return null;

        return Grid[position.x, position.y];
    }

    public Connectable[] GetSurroundingConnectables(Vector2Int position)
    {
        Connectable[] surroundings = new Connectable[8];

        if (GetConnectableAtPosition(position + new Vector2Int(-1, 1)))
        {
            surroundings[0] = GetConnectableAtPosition(position + new Vector2Int(-1, 1));
        }

        if (GetConnectableAtPosition(position + new Vector2Int(0, 1)))
        {
            surroundings[1] = GetConnectableAtPosition(position + new Vector2Int(0, 1));
        }

        if (GetConnectableAtPosition(position + new Vector2Int(1, 1)))
        {
            surroundings[2] = GetConnectableAtPosition(position + new Vector2Int(1, 1));
        }

        if (GetConnectableAtPosition(position + new Vector2Int(1, 0)))
        {
            surroundings[3] = GetConnectableAtPosition(position + new Vector2Int(1, 0));
        }

        if (GetConnectableAtPosition(position + new Vector2Int(1, -1)))
        {
            surroundings[4] = GetConnectableAtPosition(position + new Vector2Int(1, -1));
        }

        if (GetConnectableAtPosition(position + new Vector2Int(0, -1)))
        {
            surroundings[5] = GetConnectableAtPosition(position + new Vector2Int(0, -1));
        }

        if (GetConnectableAtPosition(position + new Vector2Int(-1, -1)))
        {
            surroundings[6] = GetConnectableAtPosition(position + new Vector2Int(-1, -1));
        }

        if (GetConnectableAtPosition(position + new Vector2Int(-1, 0)))
        {
            surroundings[7] = GetConnectableAtPosition(position + new Vector2Int(-1, 0));
        }

        return surroundings;
    }

    public void UpdateGridSize()
    {
        if (gridSize.x == Grid.GetLength(0) && gridSize.y == Grid.GetLength(1)) return;

        var original = Grid;
        int minRows = Math.Min(original.GetLength(0), gridSize.x);
        int minCols = Math.Min(original.GetLength(1), gridSize.y);

        Grid = new Connectable[gridSize.x, gridSize.y];
        for (int x = 0; x < minRows; x++)
        {
            for (int z = 0; z < minCols; z++)
            {
                Grid[x, z] = original[x, z];
            }
        }
    }

    private void OnDrawGizmos()
    {
        var bottomLeft = transform.position + new Vector3(-.5f, 0, -.5f);
        var topLeft = transform.position + new Vector3(-.5f, 0, Size.y - .5f);
        var bottomRight = transform.position + new Vector3(Size.x - .5f, 0, -.5f);
        var topRight = transform.position + new Vector3(Size.x - .5f, 0, Size.y - .5f);

        Gizmos.color = Color.gray;
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topRight, bottomRight);
    }

    public void UpdateGridTile(Connectable connectable, Vector2Int position)
    {
        Grid[position.x, position.y] = connectable;
    }
}
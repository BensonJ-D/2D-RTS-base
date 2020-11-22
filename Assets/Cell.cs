using UnityEngine;

public class Cell
{
    public Vector3 worldPos;
    public Vector2Int gridPos;

    public byte cost { get; set; }

    public Cell(Vector3 _worldPos, Vector2Int _gridPos, byte _cost)
    {
        worldPos = _worldPos;
        gridPos = _gridPos;
        cost = _cost;
    }
}

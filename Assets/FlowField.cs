using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FlowField
{
    public Cell[][] grid { get; private set; }
    public Vector2Int gridSize { get; private set; }
    public float cellRadius { get; private set; }

    private float cellDiameter;

    private Collider[] obstacles;

    public FlowField(float _cellRadius, Vector2Int _gridSize)
    {
        cellRadius = _cellRadius;
        cellDiameter = cellRadius * 2;
        gridSize = _gridSize;
        obstacles = new Collider[2];
    }

    public void CreateGrid()
    {
        grid = new Cell[gridSize.x][];

        for(var x = 0; x < gridSize.x; x++)
        {
            grid[x] = new Cell[gridSize.y];
            for(var y = 0; y < gridSize.y; y++)
            {
                var gridPos = new Vector2Int(x, y);
                var worldPos = new Vector3(x * cellDiameter + cellRadius, y * cellDiameter + cellRadius, 0);
                grid[x][y] = new Cell(worldPos, gridPos, 1);
            }
        }
    }
    
    public void CreateCostField()
    {
        Vector3 cellHalfExtents = Vector3.one * cellRadius;
        int terrainMask = LayerMask.GetMask("Terrain");
        foreach (var jaggedCells in grid)
        {
            foreach (var curCell in jaggedCells)
            {
                Physics.OverlapBoxNonAlloc(curCell.worldPos, cellHalfExtents, obstacles, Quaternion.identity, terrainMask);
                foreach (var col in obstacles)
                {
                    if (col.gameObject.layer == 8)
                    {
                        curCell.cost = 255;
                        break;
                    }
                    
                    if (col.gameObject.layer == 9)
                    {
                        curCell.cost = 4;
                    }
                }
            }
        }
    }
}

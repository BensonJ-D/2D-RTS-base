using System;
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

    private Cell _destinationCell;

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
        foreach (Cell[] jaggedCells in grid)
        {
            foreach (var curCell in jaggedCells)
            {
                var numObstacles = Physics.OverlapBoxNonAlloc(curCell.worldPos, cellHalfExtents, obstacles, Quaternion.identity, terrainMask);
                for(var index = 0; index < numObstacles; index++)
                {
                    if (obstacles[index].gameObject.layer != 9) continue;
                    curCell.Cost = 255;
                    break;
                }
            }
        }
    }
    
    public void CreateIntegrationField(Cell destinationCell)
    {
        _destinationCell = destinationCell;
        _destinationCell.IntegrationCost = 0;
 
        Queue<Cell> cellsToCheck = new Queue<Cell>();
 
        cellsToCheck.Enqueue(_destinationCell);
 
        while(cellsToCheck.Count > 0)
        {
            Cell curCell = cellsToCheck.Dequeue();
            ArrayWithLength<Cell> curNeighbors = GetNeighborCells(curCell);
            
            void Callback(Cell curNeighbour){
                if (curNeighbour.Cost == byte.MaxValue) { return; }
                if (curNeighbour.Cost + curCell.IntegrationCost >= curNeighbour.IntegrationCost) { return; }
                
                curNeighbour.IntegrationCost = (ushort)(curNeighbour.Cost + curCell.IntegrationCost);
                cellsToCheck.Enqueue(curNeighbour);
            }
            curNeighbors.forEach(Callback);
        }
    }
    
    public void CreateFlowField()
    {
        // foreach (Cell[] jaggedCells in grid)
        // {
        //     foreach (var curCell in jaggedCells)
        //     {
        //         var numObstacles = Physics.OverlapBoxNonAlloc(curCell.worldPos, cellHalfExtents, obstacles,
        //             Quaternion.identity, terrainMask);
        //         for (var index = 0; index < numObstacles; index++)
        //         {
        //             if (obstacles[index].gameObject.layer != 9) continue;
        //             curCell.Cost = 255;
        //             break;
        //         }
        //     }
        // }
        //
        // foreach(Cell curCell in grid)
        // {
        //     List<Cell> curNeighbors = GetNeighborCells(curCell.gridIndex, GridDirection.AllDirections);
        //
        //     int bestCost = curCell.bestCost;
        //
        //     foreach(Cell curNeighbor in curNeighbors)
        //     {
        //         if(curNeighbor.bestCost < bestCost)
        //         {
        //             bestCost = curNeighbor.bestCost;
        //             curCell.bestDirection = GridDirection.GetDirectionFromV2I(curNeighbor.gridIndex - curCell.gridIndex);
        //         }
        //     }
        // }
    }
    
    private ArrayWithLength<Cell> GetNeighborCells(Cell cell)
    {
        var cellX = cell.gridPos.x;
        var cellY = cell.gridPos.y;
        var index = 0;
        Cell[] neighborCells = new Cell[4];

        for (var i = 0; i < 4; i++)
        {
            var x = cellX + (i - 2) * (i % 2);
            var y = cellY + (i - 1) * ((i + 1) % 2);

            if (grid[x][y] == null) continue;
            neighborCells[index] = grid[x][y];
            index++;
        }
        
        return new ArrayWithLength<Cell>(neighborCells, index);
    }
    
    private ArrayWithLength<Cell> GetAllNeighborCells(Cell cell)
    {
        var cellX = cell.gridPos.x;
        var cellY = cell.gridPos.y;
        var index = 0;
        Cell[] neighborCells = new Cell[4];

        for (var i = 0; i < 8; i++)
        {
            var x = cellX - ((i - 2) - 2*((i%4)*((i+1)%2))) * (i%2) - (4 * (i/4) * ((i)%2) * (i/4));
            var y = cellY - (((i+1) - 2) - 2*(((i+1)%4)*(((i+1)+1)%2))) * ((i+1)%2) - (4 * ((i+1)/4) * (((i+1))%2) * ((i+1)/4));
            Debug.Log("X: "+ x+ ", Y: "+y);

            if (grid[x][y] == null || i == 4) continue;
            neighborCells[index] = grid[x][y];
            index++;
        }
        
        return new ArrayWithLength<Cell>(neighborCells, index);
    }
}

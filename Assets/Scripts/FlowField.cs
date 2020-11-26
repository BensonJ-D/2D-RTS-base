using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FlowField
{
    public Cell[][] Grid { get; private set; }
    public Vector2Int GridSize { get; private set; }
    public float CellRadius { get; private set; }

    private float cellDiameter;

    private Collider[] obstacles;

    private Cell _destinationCell;

    public FlowField(float _cellRadius, Vector2Int _gridSize)
    {
        CellRadius = _cellRadius;
        cellDiameter = CellRadius * 2;
        GridSize = _gridSize;
        obstacles = new Collider[2];
    }

    public void CreateGrid()
    {
        Grid = new Cell[GridSize.x][];

        for(var x = 0; x < GridSize.x; x++)
        {
            Grid[x] = new Cell[GridSize.y];
            for(var y = 0; y < GridSize.y; y++)
            {
                var gridPos = new Vector2Int(x, y);
                var worldPos = new Vector3(x * cellDiameter + CellRadius, y * cellDiameter + CellRadius, 0);
                Grid[x][y] = new Cell(worldPos, gridPos, 1);
            }
        }
    }
    
    public void CreateCostField()
    {
        Vector3 cellHalfExtents = Vector3.one * CellRadius;
        int terrainMask = LayerMask.GetMask("Terrain");
        foreach (Cell[] jaggedCells in Grid)
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
        _destinationCell.Cost = 0;
        _destinationCell.IntegrationCost = 0;
 
        Queue<Cell> cellsToCheck = new Queue<Cell>();
 
        cellsToCheck.Enqueue(_destinationCell);
 
        while(cellsToCheck.Count > 0)
        {
            Cell curCell = cellsToCheck.Dequeue();
            List<Cell> curNeighbors = GetCardinalNeighborCells(curCell);
            
            for(int i = 0; i < curNeighbors.Count; i++){
                
                if (curNeighbors[i].Cost == byte.MaxValue) { continue; }
                if (curNeighbors[i].Cost + curCell.IntegrationCost >= curNeighbors[i].IntegrationCost) { continue; }
                
                curNeighbors[i].IntegrationCost = (ushort)(curNeighbors[i].Cost + curCell.IntegrationCost);
                
                cellsToCheck.Enqueue(curNeighbors[i]);
            }
        }
    }
    
    public void CreateFlowField()
    {
        foreach (Cell[] jaggedCells in Grid)
        {
            foreach (var curCell in jaggedCells)
            {
                List<Cell> cardinalNeighbours = GetCardinalNeighborCells(curCell);
                List<Cell> diagonalNeighbours = GetDiagonalNeighborCells(curCell);

                int bestCost = curCell.IntegrationCost;
                for (int i = 0; i < cardinalNeighbours.Count; i ++)
                {
                    var direction = cardinalNeighbours[i].gridPos - curCell.gridPos;
                    
                    if (cardinalNeighbours[i].IntegrationCost >= bestCost) continue;
                    
                    bestCost = cardinalNeighbours[i].IntegrationCost;
                    curCell.BestDirection = direction;
                }
                
                for (int i = 0; i < diagonalNeighbours.Count; i ++)
                {
                    var gridPos = curCell.gridPos;
                    var direction = diagonalNeighbours[i].gridPos - curCell.gridPos;
                    var x = gridPos.x + direction.x;
                    var y = gridPos.y + direction.y;
                    
                    if(gridPos.x == 6 && gridPos.y == 4) Debug.Log($"Current: {gridPos} ... Offset X: {x}, Y: {y}");
                    
                    if (Grid[x][gridPos.y].IntegrationCost == ushort.MaxValue) continue;
                    if (Grid[gridPos.x][y].IntegrationCost == ushort.MaxValue) continue;
                    if (diagonalNeighbours[i].IntegrationCost >= bestCost) continue;
                    
                    bestCost = diagonalNeighbours[i].IntegrationCost;
                    curCell.BestDirection = direction;
                }
            }
        }
    }
    
    private List<Cell> GetCardinalNeighborCells(Cell cell)
    {
        var cellX = cell.gridPos.x;
        var cellY = cell.gridPos.y;
        var index = 0;
        Cell[] neighborCells = new Cell[4];

        for (var i = 0; i < 4; i++)
        {
            var x = cellX + (i - 2) * (i % 2);
            var y = cellY + (i - 1) * ((i + 1) % 2);
            
            if (x < 0 || x >= GridSize.x || y < 0 || y >= GridSize.y) continue;
            if(Grid[x][y] == null) continue;
            
            neighborCells[index] = Grid[x][y];
            index++;
        }

        var result = new Cell[index];
        Array.Copy(neighborCells, result, index);
        return new List<Cell>(result);
    }
    
    private List<Cell> GetDiagonalNeighborCells(Cell cell)
    {
        var cellX = cell.gridPos.x;
        var cellY = cell.gridPos.y;
        var index = 0;
        Cell[] neighborCells = new Cell[4];

        for (var i = 0; i < 4; i++)
        {
            var x = cellX + -1 + 2 * (i / 2);
            var y = cellY + -1 + 2 * (i % 2);
            
            if (x < 0 || x >= GridSize.x || y < 0 || y >= GridSize.y) continue;
            if (Grid[x][y] == null) continue;

            neighborCells[index] = Grid[x][y];
            index++;
        }
        
        var result = new Cell[index];
        Array.Copy(neighborCells, result, index);
        return new List<Cell>(result);
    }
    
    public Cell GetCellFromWorldPos(Vector3 worldPos)
    {
        float percentX = worldPos.x / (GridSize.x * cellDiameter);
        float percentY = worldPos.y / (GridSize.y * cellDiameter);
 
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
 
        int x = Mathf.Clamp(Mathf.FloorToInt((GridSize.x) * percentX), 0, GridSize.x - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt((GridSize.y) * percentY), 0, GridSize.y - 1);
        return Grid[x][y];
    }
}

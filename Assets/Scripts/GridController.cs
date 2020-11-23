using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public Vector2Int gridSize;

    public float cellRadius = 0.5f;

    public FlowField curFlowField;
    
    public GridDebug gridDebug;
    
    void Start()
    {
        curFlowField = new FlowField(cellRadius, gridSize);
        curFlowField.CreateGrid();
        curFlowField.CreateCostField();
        
        gridDebug.SetFlowField(curFlowField);
    }
}
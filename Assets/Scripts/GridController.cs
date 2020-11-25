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

        for (int i = 0; i < 8; i++)
        {
            var x = (i - 2 - 2 * (i % 4) * ((i + 1) % 2)) * (i % 2) - 4 * (i / 4) * (i % 2) * (i / 4);
            var y = (i + 1 - 2 - 2 * ((i + 1) % 4) * ((i + 1 + 1) % 2)) * ((i + 1) % 2) - 4 * ((i + 1) / 4) * ((i + 1) % 2) * ((i + 1) / 4);
            Debug.Log("X: " + x + ", Y: " + y);
        }
    }
}
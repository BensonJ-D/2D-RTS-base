using UnityEditor;
using UnityEngine;


public enum FlowFieldDisplayType { None, AllIcons, DestinationIcon, CostField, IntegrationField, DirectionField };

public class GridDebug : MonoBehaviour
{
	public GridController gridController;
	public bool displayGrid;

	public FlowFieldDisplayType curDisplayType;

	private Vector2Int gridSize;
	private float cellRadius;
	private FlowField curFlowField;

	private Sprite[] ffIcons;
	
	public void SetFlowField(FlowField newFlowField)
	{
		curFlowField = newFlowField;
		cellRadius = newFlowField.CellRadius;
		gridSize = newFlowField.GridSize;
	}
	
	private void OnDrawGizmos()
	{
		if (displayGrid)
		{
			if (curFlowField == null)
			{
				DrawGrid(gridController.gridSize, Color.yellow, gridController.cellRadius);
			}
			else
			{
				DrawGrid(gridSize, Color.green, cellRadius);
			}
		}
		
		if (curFlowField == null) { return; }
 
		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.alignment = TextAnchor.MiddleCenter;
 
		switch (curDisplayType)
		{
			case FlowFieldDisplayType.CostField:
				foreach (var jaggedCells in curFlowField.Grid)
				{
					foreach (var curCell in jaggedCells)
					{
						Handles.Label(curCell.worldPos, curCell.Cost.ToString(), style);
					}
				}

				break;
                
			case FlowFieldDisplayType.IntegrationField:
				foreach (var jaggedCells in curFlowField.Grid)
				{
					foreach (var curCell in jaggedCells)
					{
						if (curCell.IntegrationCost == ushort.MaxValue) { continue; }
						Handles.Label(curCell.worldPos, curCell.IntegrationCost.ToString(), style);
					}
				}

				break;
			
			case FlowFieldDisplayType.DirectionField:
				foreach (var jaggedCells in curFlowField.Grid)
				{
					foreach (var curCell in jaggedCells)
					{
						if (curCell.IntegrationCost == ushort.MaxValue) { continue; }

						var target = new Vector3(curCell.worldPos.x + curCell.BestDirection.x * cellRadius * 2,
							curCell.worldPos.y + curCell.BestDirection.y * cellRadius * 2);
						Handles.color = Color.red;
						Handles.DrawLine(curCell.worldPos, target);
					}
				}

				break;
			
			default:
				break;
		}
	}

	private void DrawGrid(Vector2Int drawGridSize, Color drawColor, float drawCellRadius)
	{
		Gizmos.color = drawColor;
		for (int x = 0; x < drawGridSize.x; x++)
		{
			for (int y = 0; y < drawGridSize.y; y++)
			{
				Vector3 center = new Vector3(drawCellRadius * 2 * x + drawCellRadius, drawCellRadius * 2 * y + drawCellRadius, 0);
				Vector3 size = Vector3.one * drawCellRadius * 2;
				Gizmos.DrawWireCube(center, size);
			}
		}
	}
}

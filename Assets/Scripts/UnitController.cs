using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public GridController gridController;
    public GameObject unitPrefab;
    public int numUnitsPerSpawn;
    public float moveSpeed;
 
    private GameObject[] unitsInGame;
    // Start is called before the first frame update
    void Start()
    {
        unitsInGame = GameObject.FindGameObjectsWithTag("Player");
    }

    private void FixedUpdate()
    {
        if (gridController.curFlowField == null) { return; }
        foreach (GameObject unit in unitsInGame)
        {
            Cell cellBelow = gridController.curFlowField.GetCellFromWorldPos(unit.transform.position);
            Vector3 moveDirection = new Vector3(cellBelow.BestDirection.x, cellBelow.BestDirection.y).normalized;
            Rigidbody2D unitRB = unit.GetComponent<Rigidbody2D>();
            unitRB.velocity = moveDirection * moveSpeed;
        }
    }
}

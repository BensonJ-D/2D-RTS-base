using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class SelectionBox : MonoBehaviour
{
    private Vector2 screenBoxStart;
    private Vector2 screenBoxEnd;

    [SerializeField]
    private RectTransform selectionRect;
    [SerializeField]
    private RectTransform selectionFill;
    private Camera cam;

    List<Collider2D> selections = new List<Collider2D>();


    private void Start() {
        selectionRect.gameObject.SetActive(false);
        selectionFill.gameObject.SetActive(false);
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            screenBoxStart = Input.mousePosition;

            if (selections.Count > 0)
            {
                foreach (Collider2D it in selections)
                {
                    it.gameObject.GetComponent<Selection>().selected = false;
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            screenBoxEnd = Input.mousePosition;
            if (!selectionRect.gameObject.activeInHierarchy)
            {
                selectionRect.gameObject.SetActive(true);
                selectionFill.gameObject.SetActive(true);
            }

            float dx = Mathf.Abs(screenBoxStart.x - screenBoxEnd.x);
            float dy = Mathf.Abs(screenBoxStart.y - screenBoxEnd.y);
            Vector2 delta = new Vector2(dx, dy);

            selectionRect.sizeDelta = delta;
            selectionFill.sizeDelta = delta;

            Vector2 centre = (screenBoxStart + screenBoxEnd) / 2f;
            selectionRect.position = centre;
            selectionFill.position = centre;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 worldBoxStart = cam.ScreenToWorldPoint(screenBoxStart);
            Vector2 worldBoxEnd = cam.ScreenToWorldPoint(screenBoxEnd);

            float dx = Mathf.Abs(worldBoxStart.x - worldBoxEnd.x);
            float dy = Mathf.Abs(worldBoxStart.y - worldBoxEnd.y);
            Vector2 delta = new Vector2(dx, dy);

            Vector2 centre = (worldBoxStart + worldBoxEnd) / 2f;

            LayerMask mask = LayerMask.GetMask("Units");
            selections = Physics2D.OverlapBoxAll(centre, delta, 0.0f, mask).ToList();

            foreach (Collider2D it in selections)
            {
                it.gameObject.GetComponent<Selection>().selected = true;
            }

            selectionRect.gameObject.SetActive(false);
            selectionFill.gameObject.SetActive(false);
        }
    }

}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Selection : MonoBehaviour
{
    public GameObject uiLayer;

    public GameObject selectorPrefab;

    public float scaleFactor = 1f;

    private RectTransform _selectorTransform;

    private BoxCollider2D _collider;

    private Boolean _selectable;

    
    public Vector3 target;
    
    public bool selected;

    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        _selectorTransform = Instantiate(selectorPrefab, uiLayer.transform).GetComponent<RectTransform>();
        _selectorTransform.gameObject.SetActive(false);
        _collider = GetComponent<BoxCollider2D>();
        cam = Camera.main;

        target = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!_selectorTransform.gameObject.activeSelf && selected && _selectable)
        {
            _selectorTransform.gameObject.SetActive(true);
            _selectorTransform.position = cam.WorldToScreenPoint(transform.position);
            _selectorTransform.sizeDelta = _collider.size * 120 * scaleFactor;
        }
        else if (_selectorTransform.gameObject.activeSelf && !selected)
        {
            _selectorTransform.gameObject.SetActive(false);
        }
        else if (_selectorTransform.gameObject.activeSelf && selected && _selectable)
        {
            _selectorTransform.position = cam.WorldToScreenPoint(transform.position);
            _selectorTransform.sizeDelta = _collider.size * 120 * scaleFactor ;
        }

        if (selected && Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right mouse button pressed.");
            var newTarget = cam.ScreenToWorldPoint(Input.mousePosition);
            target = new Vector3(newTarget.x, newTarget.y, -10);
        }

        if (transform.position != target)
        {
            var step = 2f * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
        }
    }

    private void OnBecameInvisible()
    {
        _selectable = false;
        _selectorTransform.gameObject.SetActive(false);
    }

    private void OnBecameVisible()
    {
        _selectable = true;
    }

    private void OnMouseDown()
    {
        
        
    }
}
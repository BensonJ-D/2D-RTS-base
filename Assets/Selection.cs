using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selection : MonoBehaviour
{
    public GameObject uiLayer;

    public GameObject selectorPrefab;

    private RectTransform _selectorTransform;

    private BoxCollider2D _collider;

    // [System.NonSerialized]
    public bool selected = false;
    // Start is called before the first frame update
    void Start()
    {
        _selectorTransform = Instantiate(selectorPrefab, uiLayer.transform).GetComponent<RectTransform>();
        _selectorTransform.gameObject.SetActive(false);
        _collider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_selectorTransform.gameObject.activeSelf && selected)
        {
            _selectorTransform.gameObject.SetActive(true);
            _selectorTransform.position = Camera.main.WorldToScreenPoint(transform.position);
            _selectorTransform.sizeDelta = _collider.size * 120;
        }
        else if (_selectorTransform.gameObject.activeSelf && !selected)
        {
            _selectorTransform.gameObject.SetActive(false);
        }
        else if (_selectorTransform.gameObject.activeSelf && selected)
        {
            _selectorTransform.position = Camera.main.WorldToScreenPoint(transform.position);
            _selectorTransform.sizeDelta = _collider.size * 120;
        }
    }

    private void OnMouseDown()
    {
        
        
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject uiLayer;

    public GameObject selectorPrefab;

    private RectTransform _selectorTransform;

    private BoxCollider2D _collider;

    [System.NonSerialized]
    public bool selected = false;
    // Start is called before the first frame update
    void Start()
    {
        _selectorTransform = Instantiate(selectorPrefab, uiLayer.transform).GetComponent<RectTransform>();
        _selectorTransform.parent.gameObject.SetActive(false);
        _collider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnMouseDown()
    {
        
        if (_selectorTransform.parent.gameObject.activeSelf == false)
        {
            _selectorTransform.parent.gameObject.SetActive(true);
        }
        _selectorTransform.position = Camera.main.WorldToScreenPoint(transform.position);
        _selectorTransform.sizeDelta = _collider.size * 120;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using IoC.Attribute;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class EndlessScrollView : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _scrollpanels = new List<GameObject>();

    [FormerlySerializedAs("_centralelement")]
    [SerializeField]
    private int _defaultCentralElement;
    private int _centralElement;
    [SerializeField]
    [Range(0, 1000)]
    private float _offset;

    public void Start()
    {
        _centralElement = _defaultCentralElement;
        InitScroll();
    }

    private void InitScroll()
    {
        foreach (GameObject panel in _scrollpanels) {
            panel.transform.localPosition = new Vector2(0, 0);
        }

        List<GameObject> list1 = _scrollpanels.GetRange(0, _centralElement);
        List<GameObject> list2 = _scrollpanels.GetRange(_centralElement + 1, _scrollpanels.Count - _centralElement - 1);

        int count = _centralElement;

        foreach (GameObject scrollpanel in list1) {
            Vector2 localPosition = scrollpanel.transform.localPosition;
            localPosition = new Vector2((localPosition.x - scrollpanel.GetComponent<RectTransform>().sizeDelta.x - _offset) * count, localPosition.y);
            scrollpanel.transform.localPosition = localPosition;
            count--;
        }
        int count2 = 1;
        foreach (GameObject scrollpanel in list2) {
            Vector2 localPosition = scrollpanel.transform.localPosition;
            localPosition = new Vector2((localPosition.x + scrollpanel.GetComponent<RectTransform>().sizeDelta.x + _offset) * count2,
                                        localPosition.y);
            scrollpanel.transform.localPosition = localPosition;
            count2++;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            MoveRight();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            MoveLeft();
        }
    }

    private void MoveRight()
    {
        if (_centralElement-1 < 0) {
            _centralElement = _scrollpanels.Count - 1;
            InitScroll();
            return;
        }
        _centralElement--;
        Debug.Log("Right Move");
        foreach (GameObject scrollpanel in _scrollpanels) {
            Vector2 localPosition = scrollpanel.transform.localPosition;
            localPosition = new Vector2(localPosition.x + scrollpanel.GetComponent<RectTransform>().sizeDelta.x + _offset, localPosition.y);
            scrollpanel.transform.localPosition = localPosition;
        }
    }

    private void MoveLeft()
    {
        if (_centralElement+1 > _scrollpanels.Count - 1) {
            _centralElement = 0;
            InitScroll();
            return;
        }
        _centralElement++;
        Debug.Log("Left Move");
        foreach (GameObject scrollpanel in _scrollpanels) {
            Vector2 localPosition = scrollpanel.transform.localPosition;
            localPosition = new Vector2(localPosition.x - scrollpanel.GetComponent<RectTransform>().sizeDelta.x - _offset, localPosition.y);
            scrollpanel.transform.localPosition = localPosition;
        }
    }
}
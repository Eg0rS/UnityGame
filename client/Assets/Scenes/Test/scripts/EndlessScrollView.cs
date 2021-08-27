using System;
using System.Collections;
using System.Collections.Generic;
using IoC.Attribute;
using NUnit.Framework;
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
    [SerializeField]
    private int _centralElement;
    [SerializeField]
    [UnityEngine.Range(0, 1000)]
    private float _offset;
    [SerializeField]
    private float _scrollSpeed;
    private bool _isMoving = false;

    private IEnumerator _coroutine;

    private struct Panel
    {
        public Panel(Vector3 position, Vector3 position2, GameObject gameObject)
        {
            Start = position;
            MoveTo = position2;
            Object = gameObject;
        }

        public Vector3 Start { get; }
        public Vector3 MoveTo { get; }
        public GameObject Object { get; }
    }

    public void Start()
    {
        _centralElement = _defaultCentralElement;
        InitScroll();
    }

    private void InitScroll()
    {
        foreach (GameObject panel in _scrollpanels) {
            panel.transform.localPosition = new Vector3(0, 0, 0);
        }
        List<GameObject> leftList = _scrollpanels.GetRange(0, _centralElement);
        List<GameObject> rightList = _scrollpanels.GetRange(_centralElement + 1, _scrollpanels.Count - _centralElement - 1);
        int countLeftPanel = leftList.Count;
        foreach (GameObject scrollpanel in leftList) {
            Vector3 localPosition = scrollpanel.transform.localPosition;
            localPosition = new Vector3((localPosition.x - scrollpanel.GetComponent<RectTransform>().sizeDelta.x - _offset) * countLeftPanel,
                                        localPosition.y, localPosition.z);
            scrollpanel.transform.localPosition = localPosition;
            countLeftPanel--;
        }
        int countRightPanels = 1;
        foreach (GameObject scrollpanel in rightList) {
            Vector3 localPosition = scrollpanel.transform.localPosition;
            localPosition =
                    new Vector3((localPosition.x + scrollpanel.GetComponent<RectTransform>().sizeDelta.x + _offset) * countRightPanels,
                                localPosition.y, localPosition.z);
            scrollpanel.transform.localPosition = localPosition;
            countRightPanels++;
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
        if (_isMoving) {
            return;
        }
        if (_centralElement - 1 < 0) {
            _centralElement = _scrollpanels.Count - 1;
            InitScroll();
            return;
        }
        _centralElement--;
        Debug.Log("Right Move");
        List<Panel> currentPanels = new List<Panel>();
        foreach (GameObject scrollpanel in _scrollpanels) {
            Vector3 startPosition = scrollpanel.transform.localPosition;
            Vector3 moveToPosition = new Vector3(startPosition.x + scrollpanel.GetComponent<RectTransform>().sizeDelta.x + _offset, startPosition.y,
                                                 startPosition.z);
            Panel currentPanel = new Panel(startPosition, moveToPosition, scrollpanel);
            currentPanels.Add(currentPanel);
        }
        _coroutine = MoveScrollPanels(currentPanels);
        StartCoroutine(_coroutine);
    }

    private void MoveLeft()
    {
        if (_isMoving) {
            return;
        }
        if (_centralElement + 1 > _scrollpanels.Count - 1) {
            _centralElement = 0;
            InitScroll();
            return;
        }
        _centralElement++;
        Debug.Log("Left Move");
        List<Panel> currentPanels = new List<Panel>();
        foreach (GameObject scrollpanel in _scrollpanels) {
            Vector3 startPosition = scrollpanel.transform.localPosition;
            Vector3 moveToPosition = new Vector3(startPosition.x - scrollpanel.GetComponent<RectTransform>().sizeDelta.x - _offset, startPosition.y,
                                                 startPosition.z);
            Panel currentPanel = new Panel(startPosition, moveToPosition, scrollpanel);
            currentPanels.Add(currentPanel);
        }
        _coroutine = MoveScrollPanels(currentPanels);
        StartCoroutine(_coroutine);
    }

    private IEnumerator MoveScrollPanels(List<Panel> panels)
    {
        _isMoving = true;
        float i = 0.0f;
        float rate = 1.0f / _scrollSpeed;
        while (i < 1.0) {
            i += Time.deltaTime * rate;
            foreach (Panel panel in panels) {
                panel.Object.transform.localPosition = Vector3.Lerp(panel.Start, panel.MoveTo, i);
            }
            yield return null;
        }
        _isMoving = false;
    }
}
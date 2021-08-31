using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ScrollViewPanel
{
    private Vector3 _startPosition;
    private Vector3 _moveToPosition;
    private readonly GameObject _object;

    public ScrollViewPanel(Vector3 startPosition, Vector3 moveToPosition, GameObject gameObject)
    {
        _startPosition = startPosition;
        _moveToPosition = moveToPosition;
        _object = gameObject;
    }

    public Vector3 StartPosition
    {
        get { return _startPosition; }
        set { _startPosition = value; }
    }
    public Vector3 MoveToPosition
    {
        get { return _moveToPosition; }
        set { _moveToPosition = value; }
    }
    public GameObject Object
    {
        get { return _object; }
    }
}

public class EndlessScrollView : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _scrollpanels = new List<GameObject>();

    [FormerlySerializedAs("_centralelement")]
    [SerializeField]
    private int _defaultCentralElement;
    private int _selectedElement;
    [SerializeField]
    [Range(0, 1000)]
    private float _offset;
    [SerializeField]
    private float _scrollSpeed;
    private bool _isMoving = false;

    [SerializeField]
    private Vector3 _initCoords;

    private IEnumerator _coroutine;

    private readonly List<ScrollViewPanel> _panelList = new List<ScrollViewPanel>();
    
    public void Start()
    {
        _selectedElement = _defaultCentralElement;
        InitScroll(_initCoords);
    }

    private void InitScroll(Vector3 initCoords)
    {
        foreach (GameObject panel in _scrollpanels) {
            panel.transform.localPosition = initCoords;
        }
        List<GameObject> leftList = _scrollpanels.GetRange(0, _selectedElement);
        List<GameObject> rightList = _scrollpanels.GetRange(_selectedElement + 1, _scrollpanels.Count - _selectedElement - 1);
        int countLeftPanel = leftList.Count;
        foreach (GameObject scrollpanel in leftList) {
            Vector3 localPosition = scrollpanel.transform.localPosition;
            localPosition = new Vector3(localPosition.x - (scrollpanel.GetComponent<RectTransform>().sizeDelta.x + _offset) * countLeftPanel,
                                        localPosition.y, localPosition.z);
            scrollpanel.transform.localPosition = localPosition;
            countLeftPanel--;
        }
        int countRightPanels = 1;
        foreach (GameObject scrollpanel in rightList) {
            Vector3 localPosition = scrollpanel.transform.localPosition;
            localPosition = new Vector3(localPosition.x + (scrollpanel.GetComponent<RectTransform>().sizeDelta.x + _offset) * countRightPanels,
                                        localPosition.y, localPosition.z);
            scrollpanel.transform.localPosition = localPosition;
            countRightPanels++;
        }

        foreach (GameObject panel in _scrollpanels) {
            _panelList.Add(new ScrollViewPanel(panel.transform.localPosition, new Vector3(0, 0, 0), panel));
        }
    }

    private void UpdateRightScroll()
    {
        Vector3 newCoords = new Vector3(_initCoords.x + (_scrollpanels[0].GetComponent<RectTransform>().sizeDelta.x + _offset), 0, 0);
        InitScroll(newCoords);
        MoveLeft(false);
    }

    private void UpdateLeftScroll()
    {
        Vector3 newCoords = new Vector3(_initCoords.x - (_scrollpanels[0].GetComponent<RectTransform>().sizeDelta.x + _offset), 0, 0);
        InitScroll(newCoords);
        MoveRight(false);
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

    private void MoveRight(bool isLastElement = true)
    {
        if (_isMoving) {
            StopCoroutine(_coroutine);
        }
        if (_selectedElement - 1 < 0) {
            _selectedElement = _scrollpanels.Count - 1;
            UpdateLeftScroll();
            return;
        }
        if (isLastElement) {
            _selectedElement--;
        }
        Debug.Log("Right Move");
        List<ScrollViewPanel> currentPanels = new List<ScrollViewPanel>();
        foreach (ScrollViewPanel scrollpanel in _panelList) {
            Vector3 startPosition = scrollpanel.StartPosition;
            Vector3 moveToPosition = new Vector3(startPosition.x + scrollpanel.Object.GetComponent<RectTransform>().sizeDelta.x + _offset,
                                                 startPosition.y, startPosition.z);
            Vector3 currentPosition = scrollpanel.Object.transform.transform.localPosition;
            ScrollViewPanel currentScrollViewPanel = new ScrollViewPanel(currentPosition, moveToPosition, scrollpanel.Object);
            currentPanels.Add(currentScrollViewPanel);
            scrollpanel.StartPosition = moveToPosition;
        }
        _coroutine = MoveScrollPanels(currentPanels);
        StartCoroutine(_coroutine);
    }

    private void MoveLeft(bool isLastElement = true)
    {
        if (_isMoving) {
            StopCoroutine(_coroutine);
        }
        if (_selectedElement + 1 > _scrollpanels.Count - 1) {
            _selectedElement = 0;
            UpdateRightScroll();
            return;
        }
        if (isLastElement) {
            _selectedElement++;
        }
        Debug.Log("Left Move");
        List<ScrollViewPanel> currentPanels = new List<ScrollViewPanel>();
        foreach (ScrollViewPanel scrollpanel in _panelList) {
            Vector3 startPosition = scrollpanel.StartPosition;
            Vector3 moveToPosition = new Vector3(startPosition.x - (scrollpanel.Object.GetComponent<RectTransform>().sizeDelta.x + _offset),
                                                 startPosition.y, startPosition.z);
            Vector3 currentPosition = scrollpanel.Object.transform.transform.localPosition;
            ScrollViewPanel currentScrollViewPanel = new ScrollViewPanel(currentPosition, moveToPosition, scrollpanel.Object);
            currentPanels.Add(currentScrollViewPanel);
            scrollpanel.StartPosition = moveToPosition;
        }
        _coroutine = MoveScrollPanels(currentPanels);
        StartCoroutine(_coroutine);
    }

    private IEnumerator MoveScrollPanels(List<ScrollViewPanel> panels)
    {
        _isMoving = true;
        float i = 0.0f;
        float rate = 1.0f / _scrollSpeed;
        while (i < 1.0) {
            i += Time.deltaTime * rate;
            foreach (ScrollViewPanel panel in panels) {
                panel.Object.transform.localPosition = Vector3.Lerp(panel.StartPosition, panel.MoveToPosition, i);
            }
            yield return null;
        }
        _isMoving = false;
    }

    public int MiddleElement
    {
        get { return _selectedElement; }
    }
}
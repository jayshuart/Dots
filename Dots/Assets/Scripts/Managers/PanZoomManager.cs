using Unity.VisualScripting;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PanZoomManager : MonoBehaviour
{
    // Inspector props
    [SerializeField] float easyZoomLevel = 2.00f;

    //private props
    private float _zoomLevel = 0.00f;
    public float ZoomLevel { get { return _zoomLevel; } }
    private float _defaultZoom = 1.00f;
    private float _offsetX = 0.0f;
    public float offsetX { get { return _offsetX; } }
    private float _offsetY = 0.0f;
    public float offsetY { get { return _offsetY; } }

    private Vector3 _originalPos;

    public static PanZoomManager I { get; private set; } //singleton, but dies with the scene

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(I);
        }
        I = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetZoom(); //always start at default zoom
        _originalPos = RoundManager.I.gridParent.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        PanWithTouchInput();
    }

    // -- Zoom functionality
    public void Zoom(float zoomLevel)
    {
        _zoomLevel = zoomLevel;
        RoundManager.I.gridParent.transform.localScale = new Vector3(zoomLevel, zoomLevel, 1);
    }

    public void ResetZoom()
    {
        Zoom(_defaultZoom);
    }

    public void ToggleEasyZoom()
    {
        if (_zoomLevel != _defaultZoom)
        {
            ResetZoom();
        }
        else
        {
            Zoom(easyZoomLevel);
        }
    }

    // -- Pan functionality
    private void PanWithTouchInput()
    {
        if(Touch.activeTouches.Count > 0 &&
            Touch.activeTouches[0].isInProgress &&
            RoundManager.I.TouchingClickArea(Touch.activeTouches[0].startScreenPosition))
        {
            Pan(Touch.activeTouches[0].delta.x, Touch.activeTouches[0].delta.y);
        }

    }
    public void Pan(float panX, float panY)
    {
        _offsetX += panX;
        _offsetY += panY;

        Vector3 pos = _originalPos;
        pos.x += _offsetX;
        pos.y += _offsetY;

        RoundManager.I.gridParent.transform.position = pos;
    }

    public void ResetPan()
    {
        RoundManager.I.gridParent.transform.position = _originalPos;
        _offsetX = 0.0f;
        _offsetY = 0.0f;
    }
    

}

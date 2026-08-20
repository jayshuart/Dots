using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class Line : MonoBehaviour
{
    //Properties
    private Player _owner;
    private Image _img;
    private Dot[] _dotAnchors;
    private RectTransform _rt;
    private Animator _animator;

    public bool Owned
    {
        get { return _owner != null; }
    }

    public bool IsVertical
    {
        get { return Owned && _dotAnchors[0].coords.x == _dotAnchors[1].coords.x;  }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Awake()
    {
        _img = GetComponent<Image>();
        _rt = GetComponent<RectTransform>();
        _dotAnchors = new Dot[2];
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetOwner(Player owner)
    {
        _owner = owner;
        _img.color = owner.color;
    }

    public void ConnectDots(Dot start, Dot end)
    {
        //set anchors
        this._dotAnchors[0] = start;
        this._dotAnchors[1] = end;

        start.Connect(end);
        end.Connect(start);

        //calc width/height and if its horizontal or vert- then position and size
        float dist = Vector3.Distance(start.transform.position, end.transform.position);

        bool isHorizontal = start.transform.position.y == end.transform.position.y;
        Vector3 newPos = start.transform.position;
        if (isHorizontal)
        {
            _rt.sizeDelta = new Vector2(dist / RoundManager.I.gameCanvas.scaleFactor, _rt.sizeDelta.y);
            newPos.x = start.transform.position.x < end.transform.position.x ?
                newPos.x + (dist / 2) :
                newPos.x - (dist / 2);
        }
        else
        {
            _rt.sizeDelta = new Vector2(_rt.sizeDelta.y, dist / RoundManager.I.gameCanvas.scaleFactor);
            newPos.y = start.transform.position.y < end.transform.position.y ?
                newPos.y + (dist / 2) :
                newPos.y - (dist / 2);
        }

        //move to the point between out start-end dots
        this.transform.position = newPos;
    }
}

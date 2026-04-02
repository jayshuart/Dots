using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class BoxFill : MonoBehaviour
{
    //Properties
    private Player _owner;
    private Image _img;
    private Dot[] _dotAnchors;
    private RectTransform _rt;

    public bool Owned
    {
        get { return _owner != null; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Awake()
    {
        _img = GetComponent<Image>();
        _rt = GetComponent<RectTransform>();
        _dotAnchors = new Dot[4];
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

    public void SetSize(float width, float height)
    {
        _rt.sizeDelta = new UnityEngine.Vector2(width, height);
    }

    public void ConnectDots(Dot[] corners)
    {
        float width = 0;
        float height = 0;
        UnityEngine.Vector2 pos = new UnityEngine.Vector2(0, 0);

        for (int i = 0; i < corners.Length; i++)
        {
            //add onto pos so we can find the midpoint
            pos.x += corners[i].transform.position.x;
            pos.y += corners[i].transform.position.y;

            //calc width/height
            if (i > 0)
            {
                if (width > 0)
                { width = Mathf.Abs(corners[0].transform.position.x - corners[i].transform.position.x); }

                if (height > 0)
                { height = Mathf.Abs(corners[0].transform.position.y - corners[i].transform.position.y); }
            }
        }

        //find average pos
        pos.x /= corners.Length;
        pos.y /= corners.Length;

        //place box and size
        this.transform.position = pos;
        _rt.sizeDelta.Set(width, height);
    }
}

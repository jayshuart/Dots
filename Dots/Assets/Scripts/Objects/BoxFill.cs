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
        Color color = owner.color;
        color.a = .75f;
        _img.color = color;
    }

    public void SetSize(float width, float height)
    {
        _rt.sizeDelta = new UnityEngine.Vector2(width, height);
    }
}

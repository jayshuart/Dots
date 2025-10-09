using UnityEngine;
using UnityEngine.UI;

public class Line : MonoBehaviour
{
    //Properties
    private Player _owner;
    private Button _lineBtn;
    private Dot[] _dotAnchors;

    public bool Owned
    {
        get { return _owner != null; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _lineBtn = GetComponent<Button>();
        _dotAnchors = new Dot[2];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetOwner(Player owner)
    {
        _owner = owner;
        _lineBtn.image.color = _owner.color;
        _lineBtn.interactable = false;
    }

    public void ConnectDots(Dot start, Dot end)
    {
        
    }
}

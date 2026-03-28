using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Dot : MonoBehaviour
{
    //Properties
    private Player _owner;
    private Image _img;
    public Vector2Int coords = new Vector2Int();

    public Dot[] connections;

    public bool Owned{
        get { return _owner != null;}
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _img = GetComponent<Image>();
        connections = new Dot[4];
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void SetCoords(int x, int y)
    {
        coords.x = x;
        coords.y = y;
    }

    public void SetOwner(Player owner){
        _owner = owner;
        _img.color = _owner.color;
    }
}

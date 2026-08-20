using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

[RequireComponent(typeof(Image))]
public class Dot : MonoBehaviour
{
    //Properties
    private Player _owner;
    public Image _img;
    public Vector2Int coords = new Vector2Int();
    public RectTransform rt;
    private Animator _animator;

     /**
        0 - top
        1 - right
        2 - bottom
        3 - left
    **/
    public Dot[] connections;

    public bool Owned{
        get { return _owner != null;}
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _img = GetComponent<Image>();
        rt = GetComponent<RectTransform>();
        _animator = GetComponent<Animator>();
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

    public void SetOwner(Player owner)
    {
        _owner = owner;
        _img.color = _owner.color;
    }

    public void Connect(Dot connection)
    {
        int index;
        if (connection.coords.x == this.coords.x)
        {
            index = connection.coords.y < this.coords.y ? 0 : 2;
        }
        else
        {
            index = connection.coords.x > this.coords.x ? 1 : 3;
        }

        connections[index] = connection;
    }

    public async Task PlayAnimSpawn(float delay = 0)
    {
        await Awaitable.WaitForSecondsAsync(delay);
        _animator.SetTrigger("Spawn");
    }
    
    public void PlayAnimTap()
    {
        _animator.SetTrigger("Tap");
    }
}

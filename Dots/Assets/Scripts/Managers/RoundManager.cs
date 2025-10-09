using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public Player[] players;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private GameObject linePrefab;

    [Header("Grid Settings")]
    [SerializeField] private GameObject gridParent;
    [SerializeField] private GameObject gridRowPrefab;
    [SerializeField] private int gridRows = 10;
    [SerializeField] private int gridColumns = 10;

    //private props
    private Dot[] _dots;
    private Line[] _lines;
    private int _currentPlayerIndex = 0;

    //public get/sets
    public Player CurrentPlayer
    {
        get { return players[_currentPlayerIndex]; }
    }

    //private get/sets
    private float _lineLengthX{
        get { return Mathf.Abs(_dots[1].transform.position.x - _dots[0].transform.position.x);  }
    }
    private float _lineLengthY{
        get { return Mathf.Abs(_dots[gridColumns].transform.position.y - _dots[0].transform.position.y);  }
    }

    //make round manager singleton
    public static RoundManager I { get; private set; }

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
        }
        else
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        BuildGrid();
    }

    // Update is called once per frame
    void Update()
    {
        CheckMouseInput();
    }

    private void CheckTouchInput()
    {

    }

    private void CheckMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            GetClosestDots(mouseScreenPos.x, mouseScreenPos.y);
            Debug.Log("Left mouse button pressed anywhere on screen!");
            StartNextPlayerTurn();

        }
    }

    private void StartNextPlayerTurn()
    {
        _currentPlayerIndex = (_currentPlayerIndex + 1) % players.Length;
        Debug.Log("Current Player: " + CurrentPlayer.name);

        //todo trigger ui change to showcase this
    }

    private void BuildGrid()
    {
        _dots = new Dot[gridRows * gridColumns];

        for (int x = 0; x < gridRows; x++)
        {
            GameObject row = Instantiate(gridRowPrefab, gridParent.transform);
            for (int y = 0; y < gridColumns; y++)
            {
                Dot dot = Instantiate(dotPrefab, row.transform).GetComponent<Dot>();
                _dots[(x * gridColumns) + y] = dot;
            }
        }

        _lines = new Line[gridRows * (gridColumns * 2)];
        // for (int x = 0; x < gridRows; x++)
        // {
        //     for (int y = 0; y < gridColumns * 2; y++)
        //     {
        //         Line line = Instantiate(linePrefab, row.transform).GetComponent<Line>();
        //         _lines[(x * (gridColumns * 2)) + y] = line;
        //     }
        // }
    }

    private void GetClosestDots(float x, float y)
    {
        int dotX = Mathf.RoundToInt(Mathf.Abs(x - _dots[0].transform.position.x) / _lineLengthX);
        int dotY = Mathf.RoundToInt(Mathf.Abs(y - _dots[0].transform.position.y) / _lineLengthY);

        Dot closestDot = _dots[(dotY * gridColumns) + dotX];
        closestDot.SetOwner(CurrentPlayer);
    }
}

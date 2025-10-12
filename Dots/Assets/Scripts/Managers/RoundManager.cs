using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public Player[] players;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private GameObject linePrefab;

    [Header("Grid Settings")]
    [SerializeField] private GameObject gridParent;
    [SerializeField] private GameObject lineParent;
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
    }

    private void GetClosestDots(float x, float y)
    {
        //TODO: cleanup the math on thi func to be more succinct

        //claim closest dot
        float dotXFloat = Mathf.Abs(x - _dots[0].transform.position.x) / _lineLengthX;
        float dotYFloat = Mathf.Abs(y - _dots[0].transform.position.y) / _lineLengthY;
        int dotX = Mathf.RoundToInt(dotXFloat);
        int dotY = Mathf.RoundToInt(dotYFloat);

        Dot closestDot = _dots[(dotY * gridColumns) + dotX];
        closestDot.SetOwner(CurrentPlayer);

        //claim closest neighbor dot based on axis
        float dotXDecimal = dotXFloat - Mathf.FloorToInt(dotXFloat);
        float dotYDecimal = dotYFloat - Mathf.FloorToInt(dotYFloat);

        float dotXF2 = dotXDecimal > .5f ? 1f - dotXDecimal : dotXDecimal;
        float dotYF2 = dotYDecimal > .5f ? 1f - dotYDecimal : dotYDecimal;

        if (dotXF2 > dotYF2)
        {
            dotX = dotXDecimal > .5f ? dotX - 1 : dotX + 1;
        }
        else
        {
            dotY = dotYDecimal > .5f ? dotY - 1 : dotY + 1;
        }


        Dot secondDot = _dots[(dotY * gridColumns) + dotX];
        secondDot.SetOwner(CurrentPlayer);

        //build line between
        Line line = Instantiate(linePrefab, lineParent.transform).GetComponent<Line>();
        line.SetOwner(CurrentPlayer);
        line.ConnectDots(closestDot, secondDot);
    }
}

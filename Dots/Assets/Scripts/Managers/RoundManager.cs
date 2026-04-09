using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public Player[] players;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject boxFillPrefab;

    [Header("Grid Settings")]
    [SerializeField] private GameObject gridParent;
    [SerializeField] private GameObject lineParent;
    [SerializeField] private GameObject boxFillParent;
    [SerializeField] private GameObject gridRowPrefab;
    [SerializeField] private int gridRows = 10;
    [SerializeField] private int gridColumns = 10;

    //private props
    private Dot[] _dots;
    private Line[] _lines;
    private BoxFill[] _areas;
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
        get { return Mathf.Abs(_dots[gridRows].transform.position.y - _dots[0].transform.position.y);  }
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
            bool successful = ClaimClosestLine(mouseScreenPos.x, mouseScreenPos.y);
            if (!successful) { return; }

            successful = ClaimClosestArea(mouseScreenPos.x, mouseScreenPos.y);
            if (successful)
            {
                BonusTurn();
            }
            else
            {
                StartNextPlayerTurn();
            }
        }
    }

    private void StartNextPlayerTurn()
    {
        _currentPlayerIndex = (_currentPlayerIndex + 1) % players.Length;
        Debug.Log("Current Player: " + CurrentPlayer.name);

        //todo trigger ui change to showcase this
    }

    private void BonusTurn()
    {
        Debug.Log("Bonus Turn: " + CurrentPlayer.name);

        //todo trigger ui to celebrate
    }

    private void BuildGrid()
    {
        //build dots
        _dots = new Dot[gridRows * gridColumns];
        for (int y = 0; y < gridRows; y++)
        {
            GameObject row = Instantiate(gridRowPrefab, gridParent.transform);
            for (int x = 0; x < gridColumns; x++)
            {
                Dot dot = Instantiate(dotPrefab, row.transform).GetComponent<Dot>();
                _dots[(y * gridRows) + x] = dot;
                dot.SetCoords(x, y);
            }
        }

        //force layout group to apply on all the dots so they are positioned properly and calcs like lineLength are correct
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(this.gridParent.GetComponent<RectTransform>());

        //generate lines array, we'll arrange these as players play.
        _lines = new Line[(2 * gridRows * gridColumns) - gridRows - gridColumns];

        //build fill areas to be claimed when all 4 lines are closed.
        _areas = new BoxFill[(gridRows - 1) * (gridColumns - 1)];
        for (int y = 0; y < gridRows - 1; y++)
        {
            for (int x = 0; x < gridColumns - 1; x++)
            {
                BoxFill boxFill = Instantiate(boxFillPrefab, boxFillParent.transform).GetComponent<BoxFill>();
                boxFill.SetSize(_lineLengthX, _lineLengthY);

                Vector3 pos = _dots[0].transform.position;
                pos.x += (_lineLengthX / 2) + (_lineLengthX * x);
                pos.y -= (_lineLengthY / 2) + (_lineLengthY * y);
                boxFill.transform.position = pos;

                _areas[(y * (gridRows - 1)) + x] = boxFill;
            }
        }
    }

    private bool ClaimClosestLine(float x, float y)
    {
        //find dots in line
        Dot closestDot = GetClosestDot(x, y);
        Dot secondDot = GetSecondClosestDotInLine(x, y);

        //claim line if possible
        int lineIndex = GetClosestLineIndex(x, y, closestDot, secondDot);
        if (_lines[lineIndex] != null) { return false; } //tell caller this func failed because the tapped line wasnt claimable
        

        //build line between
        Line line = Instantiate(linePrefab, lineParent.transform).GetComponent<Line>();
        _lines[lineIndex] = line;
        line.SetOwner(CurrentPlayer);
        line.ConnectDots(closestDot, secondDot);
        return true;
    }

    private bool ClaimClosestArea(float x, float y)
    {
        //claim area if possible
        BoxFill area = GetClosestArea(x, y);
        area.SetOwner(CurrentPlayer);
        return true;
    }

    private Dot GetClosestDot(float x, float y)
    {
        //find closest dot
        int dotX = Mathf.RoundToInt(Mathf.Abs(x - _dots[0].transform.position.x) / _lineLengthX);
        int dotY = Mathf.RoundToInt(Mathf.Abs(y - _dots[0].transform.position.y) / _lineLengthY);

        return _dots[(dotY * gridRows) + dotX];
    }

    private Dot GetSecondClosestDotInLine(float x, float y)
    {
        float dotXFloat = Mathf.Abs(x - _dots[0].transform.position.x) / _lineLengthX;
        float dotYFloat = Mathf.Abs(y - _dots[0].transform.position.y) / _lineLengthY;

        int dotX = Mathf.RoundToInt(dotXFloat);
        int dotY = Mathf.RoundToInt(dotYFloat);
        
        float dotXF2 = dotXFloat % 1 > .5f ? 1f - dotXFloat % 1 : dotXFloat % 1;
        float dotYF2 = dotYFloat % 1 > .5f ? 1f - dotYFloat % 1 : dotYFloat % 1;

        if (dotXF2 > dotYF2) //is x axis from baseline larger than y axis from y baseline?
        {
            dotX = dotXFloat % 1 > .5 ? (dotX - 1) : (dotX + 1);
        }
        else
        {
            dotY = dotYFloat % 1 > .5 ? (dotY - 1) : (dotY + 1);
        }

        return _dots[(dotY * gridRows) + dotX];
    }

    private int GetClosestLineIndex(float x, float y, Dot closestDot = null, Dot secondDot = null)
    {
        closestDot = closestDot ?? GetClosestDot(x, y);
        secondDot = secondDot ?? GetSecondClosestDotInLine(x, y);

        int lineIndex, dotX, dotY;
        if (closestDot.coords.y == secondDot.coords.y)
        {
            dotX = Mathf.RoundToInt(Mathf.Abs(x - _dots[0].transform.position.x - (_lineLengthX / 2)) / _lineLengthX);
            dotY = Mathf.RoundToInt(Mathf.Abs(y - _dots[0].transform.position.y) / _lineLengthY);
            lineIndex = (dotY * (gridRows - 1)) + dotX;
        }
        else
        {
            dotX = Mathf.RoundToInt(Mathf.Abs(x - _dots[0].transform.position.x) / _lineLengthX);
            dotY = Mathf.RoundToInt(Mathf.Abs(y - _dots[0].transform.position.y - (_lineLengthY / 2)) / _lineLengthY);
            lineIndex = ((gridRows - 1) * (gridColumns - 1)) + (dotY * (gridRows - 1)) + dotX;
        }

        secondDot.SetOwner(CurrentPlayer);
        closestDot.SetOwner(CurrentPlayer);

        return lineIndex;
    }

    private Line GetClosestLine(float x, float y)
    {
        return _lines[GetClosestLineIndex(x, y)];
    }
    
    private BoxFill GetClosestArea(float x, float y)
    {
        int dotX = Mathf.RoundToInt(Mathf.Abs(x - _areas[0].transform.position.x) / _lineLengthX);
        int dotY = Mathf.RoundToInt(Mathf.Abs(y - _areas[0].transform.position.y) / _lineLengthY);
        return _areas[(dotY * (gridRows - 1)) + dotX];
    }
}

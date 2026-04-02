using System;
using NUnit.Framework;
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
            bool successful = GetClosestDots(mouseScreenPos.x, mouseScreenPos.y);

            if(!successful){ return;  }
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
        //build dots
        _dots = new Dot[gridRows * gridColumns];
        for (int x = 0; x < gridRows; x++)
        {
            GameObject row = Instantiate(gridRowPrefab, gridParent.transform);
            for (int y = 0; y < gridColumns; y++)
            {
                Dot dot = Instantiate(dotPrefab, row.transform).GetComponent<Dot>();
                _dots[(x * gridColumns) + y] = dot;
                dot.SetCoords(x, y);
            }
        }

        //force layout group to apply on all the dots so they are positioned properly and calcs like lineLength are correct
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(this.gridParent.GetComponent<RectTransform>());

        //generate lines array, we'll arrange these as players play.
        _lines = new Line[(2 * gridRows * gridColumns) - gridRows - gridColumns];

        //build fill areas to be claimed when all 4 lines are closed.
        _areas = new BoxFill[(gridRows - 1) * (gridColumns - 1)];
        for (int x = 0; x < gridRows - 1; x++)
        {
            for (int y = 0; y < gridColumns - 1; y++)
            {
                BoxFill boxFill = Instantiate(boxFillPrefab, boxFillParent.transform).GetComponent<BoxFill>();
                boxFill.SetSize(_lineLengthX, _lineLengthY);

                Vector3 pos = _dots[0].transform.position;
                pos.x += (_lineLengthX / 2) + (_lineLengthX * x);
                pos.y -= (_lineLengthY / 2) + (_lineLengthY * y);
                boxFill.transform.position = pos;

                _areas[(x * (gridColumns - 1)) + y] = boxFill;
            }
        }
    }

    private bool GetClosestDots(float x, float y)
    {
        //TODO: cleanup the math on this func to be more succinct

        //find closest dot
        float dotXFloat = Mathf.Abs(x - _dots[0].transform.position.x) / _lineLengthX;
        float dotYFloat = Mathf.Abs(y - _dots[0].transform.position.y) / _lineLengthY;
        int dotX = Mathf.RoundToInt(dotXFloat);
        int dotY = Mathf.RoundToInt(dotYFloat);

        Dot closestDot = _dots[(dotY * gridColumns) + dotX];
        
        //findclosest neighbor dot based on axis
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

        //calc line index
        int lineIndex;
        if (closestDot.coords.x == secondDot.coords.x)
        {
            dotX = Mathf.RoundToInt(Mathf.Abs(x - _dots[0].transform.position.x + (_lineLengthX / 2)) / _lineLengthX);
            dotY = Mathf.RoundToInt(Mathf.Abs(y - _dots[0].transform.position.y) / _lineLengthY);
            lineIndex = (dotY * (gridColumns - 1)) + dotX - 1;
        }
        else
        {
            dotX = Mathf.RoundToInt(Mathf.Abs(x - _dots[0].transform.position.x) / _lineLengthX);
            dotY = Mathf.RoundToInt(Mathf.Abs(y - _dots[0].transform.position.y + (_lineLengthY / 2)) / _lineLengthY);
            lineIndex = ((gridColumns - 1) * gridRows) + (dotY * gridColumns) + dotX;
        }

        //claim if possible
        if (_lines[lineIndex] != null) { return false; } //tell caller this func failed because the tapped line wasnt claimable
        secondDot.SetOwner(CurrentPlayer);
        closestDot.SetOwner(CurrentPlayer);

        //build line between
        Line line = Instantiate(linePrefab, lineParent.transform).GetComponent<Line>();
        _lines[lineIndex] = line;
        line.SetOwner(CurrentPlayer);
        line.ConnectDots(closestDot, secondDot);

        //claim area if possible
        dotX = Mathf.RoundToInt(Mathf.Abs(x - _areas[0].transform.position.x) / _lineLengthX);
        dotY = Mathf.RoundToInt(Mathf.Abs(y - _areas[0].transform.position.y) / _lineLengthY);
        int index = (dotX * (gridColumns - 1)) + dotY;
        _areas[index].SetOwner(CurrentPlayer);

        return true;
    }
}

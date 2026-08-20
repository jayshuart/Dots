using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class RoundManager : MonoBehaviour
{
    public Canvas gameCanvas;
    public Player[] players;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject boxFillPrefab;

    [Header("Grid Settings")]
    [SerializeField] private GameObject gridParent;
    [SerializeField] private GameObject lineParent;
    [SerializeField] private GameObject boxFillParent;
    [SerializeField] private GameObject gridRowPrefab;

    //non editor props
    public RoundPlayer[] roundPlayers;
    private Dot[] _dots;
    private Line[] _lines;
    private BoxFill[] _areas;

    private int _gridRows;
    private int _gridColumns;

    //round control propers
    private int _currentPlayerIndex = 0;
    private int _areasLeft; //how many areas until gameover?

    //public get/sets
    public RoundPlayer CurrentPlayer
    {
        get { return roundPlayers[_currentPlayerIndex]; }
    }

    //private get/sets
    private float _lineLengthX{
        get { return Mathf.Abs(_dots[1].transform.position.x - _dots[0].transform.position.x);  }
    }
    private float _lineLengthY{
        get { return Mathf.Abs(_dots[_gridColumns].transform.position.y - _dots[0].transform.position.y);  }
    }

    //make round manager singleton
    public static RoundManager I { get; private set; } //singleton, but dies with the scene

    //events
    public Action onRoundReady;
    public Action onRoundStart;
    public Action<Player> onNextTurn;
    public Action<Player> onRoundEnd;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(I);
        }
        I = this;
    }

    void OnEnable() {
        EnhancedTouchSupport.Enable();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //get set grid
        _gridRows = SettingsData.gridRows;
        _gridColumns = SettingsData.gridColumns;

        BuildGrid();
        PrepareRound();

        PlayAnimGridSpawn();
        onRoundStart?.Invoke();
    }

    private void PrepareRound()
    {
        //generate players with scoring and extra features from our absic player data
        roundPlayers = new RoundPlayer[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            roundPlayers[i] = ScriptableObject.CreateInstance<RoundPlayer>();
            roundPlayers[i].player = players[i];
        }

        //determine how many areas are claimable
        _areasLeft = _areas.Length;

        //tell listeners round is ready
        onRoundReady?.Invoke();
    }

    private void PlayAnimGridSpawn()
    {
        float delay = (.65f / Mathf.Max(_gridRows, _gridColumns));
        for (int y = 0; y < _gridRows; y++)
        {
            for (int x = 0; x < _gridColumns; x++)
            {
                int index = (y * _gridRows) + x;
                int max = Mathf.Max(x, y);
                _dots[index].PlayAnimSpawn(delay * max);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        if (Touch.activeTouches.Count > 0 && Touch.activeTouches[0].isTap)
        {
            //check if we claimed a valid line
            Vector2 mouseScreenPos = ClampPosToDots(Touch.activeTouches[0].screenPosition);

            (int, bool) successfulLine = ClaimClosestLine(mouseScreenPos.x, mouseScreenPos.y);
            if (!successfulLine.Item2) { return; }

            //check if that line closed off any areas
            Line closestLine = _lines[successfulLine.Item1];
            bool areaFirstSuccess, areaSecondSuccess; //do these checks speratly so the claim func fores for both if needed
            if (closestLine.IsVertical)
            {
                areaFirstSuccess = ClaimClosestArea(closestLine.transform.position.x - (_lineLengthX / 2), closestLine.transform.position.y);
                areaSecondSuccess = ClaimClosestArea(closestLine.transform.position.x + (_lineLengthX / 2), closestLine.transform.position.y);
            }
            else
            {
                areaFirstSuccess = ClaimClosestArea(closestLine.transform.position.x, closestLine.transform.position.y - (_lineLengthY / 2));
                areaSecondSuccess = ClaimClosestArea(closestLine.transform.position.x, closestLine.transform.position.y + (_lineLengthY / 2));
            }

            //if we claimed an area, get another turn
            if (areaFirstSuccess || areaSecondSuccess)
            {
                OnScoringMove();
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
        Debug.Log("Current Player: " + CurrentPlayer.player.playerName);

        //trigger ui change to showcase this
        onNextTurn?.Invoke(CurrentPlayer.player);
    }

    private void OnScoringMove()
    {
        //check if all areas have been claimed
        if(_areasLeft <= 0)
        {
            OnRoundOver();
        }
        else
        {
            //trigger ui to celebrate
            Debug.Log("Bonus Turn: " + CurrentPlayer.player.playerName);
            onNextTurn?.Invoke(CurrentPlayer.player);
        }
    }

    private void OnRoundOver()
    {
        Debug.Log("--------------------------------------------------------------");
        Debug.Log("[Final Scores]");
        Debug.Log("(" + roundPlayers[0].player.playerName + ") " + roundPlayers[0].score + " || " + roundPlayers[1].score + " (" + roundPlayers[1].player.playerName + ")");


        //see who won base don number of areas owned
        if (roundPlayers[0].score == roundPlayers[1].score) //tie
        {
            //trigger ui to celebrate
            onRoundEnd?.Invoke(null);
        }
        else
        {
            Player winner = roundPlayers[0].score > roundPlayers[1].score ?
                roundPlayers[0].player : roundPlayers[1].player;

            //trigger ui to celebrate
            onRoundEnd?.Invoke(winner);
        }        
    }

    private void BuildGrid()
    {
        //build dots
        _dots = new Dot[_gridRows * _gridColumns];
        for (int y = 0; y < _gridRows; y++)
        {
            GameObject row = Instantiate(gridRowPrefab, gridParent.transform);
            for (int x = 0; x < _gridColumns; x++)
            {
                Dot dot = Instantiate(dotPrefab, row.transform).GetComponent<Dot>();
                _dots[(y * _gridColumns) + x] = dot;
                dot.SetCoords(x, y);
            }
        }

        //force layout group to apply on all the dots so they are positioned properly and calcs like lineLength are correct
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(this.gridParent.GetComponent<RectTransform>());

        //generate lines array, we'll arrange these as players play.
        _lines = new Line[(2 * _gridRows * _gridColumns) - _gridRows - _gridColumns + 1];

        //build fill areas to be claimed when all 4 lines are closed.
        _areas = new BoxFill[(_gridRows - 1) * (_gridColumns - 1)];
        for (int y = 0; y < _gridRows - 1; y++)
        {
            for (int x = 0; x < _gridColumns - 1; x++)
            {
                BoxFill boxFill = Instantiate(boxFillPrefab, boxFillParent.transform).GetComponent<BoxFill>();
                boxFill.SetSize(_lineLengthX / gameCanvas.scaleFactor, _lineLengthY / gameCanvas.scaleFactor);

                Vector3 pos = _dots[0].transform.position;
                pos.x += (_lineLengthX / 2) + (_lineLengthX * x);
                pos.y -= (_lineLengthY / 2) + (_lineLengthY * y);
                boxFill.transform.position = pos;

                _areas[(y * (_gridColumns - 1)) + x] = boxFill;
            }
        }

        Debug.Log(_lineLengthX);
        Debug.Log(_lineLengthY);
    }

    private (int, bool) ClaimClosestLine(float x, float y)
    {
        //find dots in line
        Dot closestDot = GetClosestDot(x, y);
        Dot secondDot = GetSecondClosestDotInLine(x, y);

        //claim line if possible
        int lineIndex = GetClosestLineIndex(x, y, closestDot, secondDot);
        if (_lines[lineIndex] != null) { return (lineIndex, false); } //tell caller this func failed because the tapped line wasnt claimable


        //build line between
        Line line = Instantiate(linePrefab, lineParent.transform).GetComponent<Line>();
        _lines[lineIndex] = line;
        line.ConnectDots(closestDot, secondDot);

        line.SetOwner(CurrentPlayer.player);

        return (lineIndex, true);
    }

    private bool ClaimClosestArea(float x, float y)
    {
        //find closest areas
        BoxFill area = GetClosestArea(x, y);
        if(area.Owned) { return false; } //cant claim what already has been claimed

        //check surrounding lines for being claimed
        Dot[] dots = {
            GetClosestDot(area.transform.position.x - _lineLengthX / 2, area.transform.position.y - _lineLengthY / 2), //0 - bottom left
            GetClosestDot(area.transform.position.x - _lineLengthX / 2, area.transform.position.y + _lineLengthY / 2), //1 - top left
            GetClosestDot(area.transform.position.x + _lineLengthX / 2, area.transform.position.y - _lineLengthY / 2), //2 - bottom right
            GetClosestDot(area.transform.position.x + _lineLengthX / 2, area.transform.position.y + _lineLengthY / 2) //3 - top right
        };

        //now check they are properly connected
        if (dots[1].connections[1] == dots[3] &&
        dots[1].connections[2] == dots[0] &&
        dots[2].connections[0] == dots[3] &&
        dots[2].connections[3] == dots[0])
        {
            //if so, set owner and return the claim successful
            area.SetOwner(CurrentPlayer.player);
            area.PlayAnimSpawn();
            _areasLeft--;
            GivePoint(CurrentPlayer);
            return true;
        }

        //fallback, failed claiming
        return false;
    }

    private Dot GetClosestDot(float x, float y)
    {
        //find closest dot
        int dotX = Mathf.RoundToInt(Mathf.Abs(x - _dots[0].transform.position.x) / _lineLengthX);
        int dotY = Mathf.RoundToInt(Mathf.Abs(y - _dots[0].transform.position.y) / _lineLengthY);
        Debug.Log(dotX);
        Debug.Log(dotY);
        return _dots[(dotY * _gridColumns) + dotX];
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

        return _dots[(dotY * _gridColumns) + dotX];
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
            lineIndex = (dotY * (_gridColumns - 1)) + dotX;
        }
        else
        {
            dotX = Mathf.RoundToInt(Mathf.Abs(x - _dots[0].transform.position.x) / _lineLengthX);
            dotY = Mathf.RoundToInt(Mathf.Abs(y - _dots[0].transform.position.y - (_lineLengthY / 2)) / _lineLengthY);
            lineIndex = ((_gridRows - 1) * (_gridColumns - 1)) + (dotY * _gridColumns) + dotX;
        }

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
        int index = (dotY * (_gridColumns - 1)) + dotX;
        index = Mathf.Clamp(index, 0, _areas.Length - 1);
        return _areas[index];
    }

    // -- scoring
    private void GivePoint(RoundPlayer player)
    {
        CurrentPlayer.score++;
        Debug.Log(CurrentPlayer.player.playerName + " Score: " + CurrentPlayer.score);
    }

    // -- Helper funs
    private Vector2 ClampPosToDots(float x, float y)
    {
        return new Vector2(
            Mathf.Clamp(x, _dots[0].transform.position.x, _dots[_dots.Length - 1].transform.position.x),
            Mathf.Clamp(y, _dots[_dots.Length - 1].transform.position.y, _dots[0].transform.position.y)
        );
    }

    private Vector2 ClampPosToDots(Vector2 pos)
    {
        return ClampPosToDots(pos.x, pos.y);
    }

    // -- Helper Funcs
    public RoundPlayer GetRoundPlayer(string name)
    {
        for (int i = 0; i < roundPlayers.Length; i++)
        {
            if (roundPlayers[i].player.playerName == name)
            {
                return roundPlayers[i];
            }
        }
        
        return null;
    }
}

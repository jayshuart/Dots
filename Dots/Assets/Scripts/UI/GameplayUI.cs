using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private Score[] scores;
    [SerializeField] private Image currentPlayerIndicator;

    void Awake()
    {
        RoundManager.I.onRoundReady += onRoundReady;
        RoundManager.I.onRoundStart += onRoundStart;
        RoundManager.I.onNextTurn += onNextTurn;
        RoundManager.I.onRoundEnd += onRoundEnd;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void onRoundReady()
    {
        InitPlayers(RoundManager.I.roundPlayers);
    }

    void onRoundStart()
    {
        SetCurrentPlayer();
    }

    void onNextTurn(Player player)
    {
        //update scores
        UpdateScores();

        //todo: trigger ui transition
        SetCurrentPlayer();
    }

    void onRoundEnd(Player winner)
    {
        UpdateScores();
    }

    void InitPlayers(RoundPlayer[] players)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                scores[i].SetColour(players[i].player.color);
                scores[i].SetScoreText(players[i].score);
            }
        }
    }

    void SetCurrentPlayer()
    {
        currentPlayerIndicator.color = RoundManager.I.CurrentPlayer.player.color;
    }

    void UpdateScores()
    {
        for(int i = 0; i < scores.Length; i++)
        {
            scores[i].SetScoreText(RoundManager.I.roundPlayers[i].score);
        }
    }

    void OnDestroy()
    {
        RoundManager.I.onRoundReady -= onRoundReady;
        RoundManager.I.onRoundStart -= onRoundStart;
        RoundManager.I.onNextTurn -= onNextTurn;
        RoundManager.I.onRoundEnd -= onRoundEnd;
    }
}

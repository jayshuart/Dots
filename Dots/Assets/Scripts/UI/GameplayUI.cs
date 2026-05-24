using UnityEditor.Build.Content;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private Score[] scores;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RoundManager.I.onRoundReady += onRoundReady;
        RoundManager.I.onNextTurn += onNextTurn;
        RoundManager.I.onRoundEnd += onRoundEnd;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void onRoundReady()
    {
        InitPlayers(RoundManager.I.roundPlayers);
    }

    void onNextTurn(Player player)
    {
        //update scores
        UpdateScores();

        //todo: trigger ui transition
    }

    void onRoundEnd(Player winner)
    {
        UpdateScores();
    }

    void UpdateScores()
    {
        for(int i = 0; i < scores.Length; i++)
        {
            scores[i].SetScoreText(RoundManager.I.roundPlayers[i].score);
        }
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

    void OnDestroy()
    {
        RoundManager.I.onRoundReady -= onRoundReady;
        RoundManager.I.onNextTurn -= onNextTurn;
        RoundManager.I.onRoundEnd -= onRoundEnd;
    }
}

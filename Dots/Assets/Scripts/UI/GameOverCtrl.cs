using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverCtrl : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TextMeshProUGUI winnerName;
    [SerializeField] private Image background;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RoundManager.I.onRoundEnd += OnRoundEnd;
    }

    private void OnRoundEnd(Player winner)
    {
        RoundManager.I.gameCanvas.gameObject.SetActive(false);
        gameOverScreen.SetActive(true);

        if (winner != null)
        {
            winnerName.text = winner.name;
            background.color = winner.color;
        }
        else
        {
            winnerName.text = "Tie!";
            background.color = Color.black;
        }
    }
    
    public void OnClickHome()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

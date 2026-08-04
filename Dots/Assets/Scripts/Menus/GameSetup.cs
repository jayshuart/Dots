using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSetup : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private Player[] players;
    [SerializeField] private Color[] colours;
    [SerializeField] private Button[] buttons;
    [SerializeField] private TMP_InputField[] nameFields;

    [Header("Grid")]
    [SerializeField] private TextMeshProUGUI columnsText;
    [SerializeField] private TextMeshProUGUI rowsText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < players.Length; i++)
        {
            SetInitialColorBtnColor(i);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // --- Player Settings
    private void SetInitialPlayername(int playerIndex)
    {

    }

    public void SetPlayerName(string playerName)
    {
        
    }
    
    private void SetInitialColorBtnColor(int playerIndex)
    {
        //find current colour
        int current = 0;
        for (int i = 0; i < colours.Length; i++)
        {
            if (colours[i] == players[playerIndex].color)
            {
                current = i;
            }
        }

        //set colour
        players[playerIndex].color = colours[current];
        buttons[playerIndex].GetComponent<Image>().color = colours[current];
    }

    public void OnClickColorBtn(int playerIndex)
    { //todo: replace with grid of options - buttons or eyedroper

        //find current colour
        int current = 0;
        for (int i = 0; i < colours.Length; i++)
        {
            if (colours[i] == players[playerIndex].color)
            {
                current = i;
            }
        }

        //get new colour
        current = (current + 1) % colours.Length;

        //set to visual rep and player
        players[playerIndex].color = colours[current];
        buttons[playerIndex].GetComponent<Image>().color = colours[current];
    }
    
    public void OnChangePlayerName(string playerName, int playerIndex)
    {
        players[playerIndex].name = playerName;
    }

    // --- Grid Settings
    public void OnColumnsChange(float value)
    {
        columnsText.text = value.ToString();
        SettingsData.gridColumns = (int) value;
    }

    public void OnRowsChange(float value)
    {
        rowsText.text = value.ToString();
        SettingsData.gridRows = (int) value;
    }
    
    // --- Navigation
    public void OnClickStart()
    {
        SceneManager.LoadScene("Gameplay");
    }
}

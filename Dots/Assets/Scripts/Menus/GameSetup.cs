using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSetup : MonoBehaviour
{
    [SerializeField] private Button startBtn;

    [Header("Players")]
    [SerializeField] private Player[] players;
    [SerializeField] private Color[] colours;
    [SerializeField] private Button[] buttons;
    [SerializeField] private TMP_InputField[] nameFields;

    [Header("Grid")]
    [SerializeField] private TextMeshProUGUI columnsText;
    [SerializeField] private TextMeshProUGUI rowsText;
    [SerializeField] private Slider columnsSlider;
    [SerializeField] private Slider rowsSlider;

    [Header("Errors")]
    [SerializeField] private TextMeshProUGUI error;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < players.Length; i++)
        {
            SetInitialColorBtnColor(i);
            SetInitialPlayerName(i);
        }

        SetInitialGridSize();

        refreshErrorMessage();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // --- Player Settings
    private void SetInitialPlayerName(int playerIndex)
    {
        nameFields[playerIndex].text = players[playerIndex].name;
    }

    public void OnChangePlayerOneName(string playerName)
    {
        OnChangePlayerName(playerName, 0);
    }
    
    public void OnChangePlayerTwoName(string playerName)
    {
        OnChangePlayerName(playerName, 1);
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
        refreshErrorMessage();
    }

    public void OnChangePlayerName(string playerName, int playerIndex)
    {
        players[playerIndex].name = playerName;
        refreshErrorMessage();
    }

    // --- Grid Settings
    private void SetInitialGridSize()
    {
        columnsSlider.value = SettingsData.gridColumns;
        rowsSlider.value = SettingsData.gridRows;
    }
    public void OnColumnsChange(float value)
    {
        columnsText.text = value.ToString();
        SettingsData.gridColumns = (int) value;
    }

    public void OnRowsChange(float value)
    {
        rowsText.text = value.ToString();
        SettingsData.gridRows = (int)value;
    }

    // -- Error Messaging
    private void refreshErrorMessage()
    {

        if (players[0].color == players[1].color //same colour
         || players[0].name == players[1].name //same name
         || players[0].name.Trim() == "" || players[0].name.Trim() == "") //empty names
        {
            error.gameObject.SetActive(true);
            error.text = "Error: empty name, the same name, or the same colour.";
            startBtn.interactable = false;
        }
        else
        {
            error.gameObject.SetActive(false);
            error.text = "";
            startBtn.interactable = true;
        }
    }

    // --- Navigation
    public void OnClickStart()
    {
        SceneManager.LoadScene("Gameplay");
    }
    
    public void OnClickBack()
    {
        MainMenu.I.GotoSplash();
    }
}

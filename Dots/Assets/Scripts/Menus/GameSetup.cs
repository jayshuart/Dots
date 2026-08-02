using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSetup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI columnsText;
    [SerializeField] private TextMeshProUGUI rowsText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnColumnsChange(float value)
    {
        columnsText.text = value.ToString();
    }

    public void OnRowsChange(float value)
    {
        rowsText.text = value.ToString();
    }
    
    public void OnClickStart()
    {
        SceneManager.LoadScene("Gameplay");
    }
}

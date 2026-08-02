using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playBtn;
    [SerializeField] private Button settingsBtn;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playBtn.onClick.AddListener(OnClickPlay);
        settingsBtn.onClick.AddListener(OnClickSettings);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickPlay()
    {
        playBtn.interactable = false;
    }
    
    public void OnClickSettings()
    {
        settingsBtn.interactable = false;
    }
}

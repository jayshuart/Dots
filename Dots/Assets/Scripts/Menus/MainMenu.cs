using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Screens")] //todo: expand this into a mor eproper UI system
    [SerializeField] private GameObject splashScreen;
    [SerializeField] private GameObject gameSetupScreen;


    public static MainMenu I { get; private set; }
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

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GotoSplash()
    {
        splashScreen.SetActive(true);
        gameSetupScreen.SetActive(false);
    }

    public void GotoGameSetup()
    {
        splashScreen.SetActive(false);
        gameSetupScreen.SetActive(true);
    }
    
    public void GotoSettings()
    {
        splashScreen.SetActive(false);
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] private Image backingImage;
    [SerializeField] private TMP_Text scoreText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetColour(Color colour)
    {
        colour.a = backingImage.color.a;
        backingImage.color = colour;
    }
    
    public void SetScoreText(int score)
    {
        scoreText.text = score.ToString();
    }
}

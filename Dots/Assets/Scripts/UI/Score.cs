using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] private Image backingImage;
    [SerializeField] private Image bar;
    [SerializeField] private TMP_Text scoreText;
    private int _numScore = 0;
    private Animator _animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetColour(Color colour)
    {
        backingImage.color = colour;
    }

    public void SetScoreText(int score)
    {
        PlayAnimScore(score);
        _numScore = score;
        scoreText.text = score.ToString();
    }

    public void SetIsTurn(bool isTurn)
    {
        bar.gameObject.SetActive(!isTurn);
    }
    
    public void PlayAnimScore(int score)
    {
        //if (score <= _numScore) { return; } 

        int diff = score - _numScore;
        // int streak = diff > 0 ?
        //     _animator.GetInteger("Streak") : 0;
        int streak = 0; //remove if streask are used
        _animator.SetInteger("Streak", streak + diff);
        _animator.SetTrigger("Increment");
    }
}

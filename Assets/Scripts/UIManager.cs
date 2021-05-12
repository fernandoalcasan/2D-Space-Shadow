using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // UI score text
    [SerializeField]
    private Text _scoreText;

    //lives img
    [SerializeField]
    private Image _livesImg;

    //array for sprites
    [SerializeField]
    private Sprite[] _liveSprites;

    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: 0";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScore(int value)
    {
        _scoreText.text = "Score: " + value;
    }

    public void UpdateLives(int lives)
    {
        _livesImg.sprite = _liveSprites[lives];
    }
}

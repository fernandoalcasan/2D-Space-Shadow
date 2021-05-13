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

    // Game over ui text
    [SerializeField]
    private Text _gameOverText;

    //Restart key ui text
    [SerializeField]
    private Text _restartText;

    //Game Manager reference
    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: 0";
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if(_gameManager is null)
        {
            Debug.LogError("Game Manager is NULL");
        }
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

    public void OnPlayerDeath()
    {
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverEffect());
        _gameManager.GameOver();
    }

    IEnumerator GameOverEffect()
    {
        while(true)
        {
            _gameOverText.text = "Game Over";
            yield return new WaitForSeconds(0.3f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.3f);
        }
    }
}

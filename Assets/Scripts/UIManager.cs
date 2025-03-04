using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get { return _instance; }
    }

    [SerializeField] TMP_Text _scoreText;
    [SerializeField] Image _livesImages;
    [SerializeField] Sprite[] _livesSprites;
    [SerializeField] GameObject _extraDisplay;
    [SerializeField] GameObject _gameOverText;

    private void Awake()
    {
        _instance = this;
        if (_extraDisplay == null)
            Debug.LogError("Extra Lives Display is NULL!",this);
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = $"Score: {score}";
    }

    public void UpdateLives(int currentLives)
    {
        if (currentLives < _livesSprites.Length && currentLives >= 0)
        {
            _livesImages.sprite = _livesSprites[currentLives];
            _extraDisplay.SetActive(false);
        }
        else if (currentLives >  _livesSprites.Length)
        {
            _extraDisplay.SetActive(true);
        }
        else
        {
            _livesImages.sprite = _livesSprites[0];
            currentLives = 0;
        }

        if (currentLives == 0)
            OnGameOver();
    }
    
    public void OnGameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        while (true)
        {
            _gameOverText.SetActive(true);
            yield return new WaitForSeconds(1f);
            _gameOverText.SetActive(false);
            yield return new WaitForSeconds(.25f);
        }
    }
}

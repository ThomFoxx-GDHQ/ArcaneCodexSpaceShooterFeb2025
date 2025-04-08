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
    [SerializeField] Image _thrusterImage;
    [SerializeField] Image _shieldMeter;
    [SerializeField] Sprite[] _shieldMeterSprites;
    [SerializeField] TMP_Text _ammoText;
    [SerializeField] TMP_Text _waveText;

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

    public void UpdateShieldMeter(int shieldHealth)
    {
        if (shieldHealth < _shieldMeterSprites.Length && shieldHealth >= 0)
            _shieldMeter.sprite = _shieldMeterSprites[shieldHealth];
        else
            _shieldMeter.sprite = _shieldMeterSprites[0];

        if (shieldHealth == 0)
            _shieldMeter.gameObject.SetActive(false);
        else if (!_shieldMeter.gameObject.activeInHierarchy)
            _shieldMeter.gameObject.SetActive(true);
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

    public void UpdateThrustSlider(float heatValue)
    {
        if (heatValue == 0)
            _thrusterImage.gameObject.SetActive(false);
        else if (!_thrusterImage.gameObject.activeInHierarchy)
            _thrusterImage.gameObject.SetActive(true);

        _thrusterImage.fillAmount = heatValue / 100;
    }

    public void UpdateAmmo(int ammoCount)
    {
        _ammoText.text = $"Ammo: {ammoCount}";
    }

    public void UpdateWaveText(int waveCount, bool state)
    {
        _waveText.gameObject.SetActive(state);
        _waveText.text = $"Wave {waveCount}";
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get { return _instance; }
    }

    [SerializeField] TMP_Text _scoreText;

    private void Awake()
    {
        _instance = this;
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = $"Score: {score}";
    }
}

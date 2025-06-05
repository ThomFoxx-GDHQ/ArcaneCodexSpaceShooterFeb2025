using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private SceneAsset _level1Scene;

    public void StartGame()
    {
        SceneManager.LoadScene(_level1Scene.name);
    }
}

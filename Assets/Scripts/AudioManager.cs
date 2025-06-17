using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    [SerializeField] private AudioSource _emptyClipSound;

    public void PlayEmptyClip()
    {
        if (!_emptyClipSound.isPlaying)
        {
            _emptyClipSound.Play();
        }
        else
        {
            _emptyClipSound.Stop();
            _emptyClipSound.Play();
        }
    }
}

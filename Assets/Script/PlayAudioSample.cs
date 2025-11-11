using TMPro;
using UnityEngine;

public class PlayAudioSample : MonoBehaviour
{
    [SerializeField] private TMP_InputField _bgmAudioName;
    [SerializeField] private TMP_InputField _sfxAudioName;

    public void PlayBGMAudio()
    {
        AudioManager.Instance.PlayBGM(_bgmAudioName.text);
    }

    public void StopBGMAudio()
    {
        AudioManager.Instance.StopBGM();
    }

    public void PlaySFXAudio()
    {
        AudioManager.Instance.PlaySFX(_sfxAudioName.text);
    }
}

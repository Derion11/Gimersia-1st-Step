using TMPro;
using UnityEngine;

public class PlayAudioSample : MonoBehaviour
{
    [SerializeField] private TMP_InputField _bgmAudioName;
    [SerializeField] private TMP_InputField _sfxAudioName;

    public void PlayBGMAudio()
    {
        AudioManaging.Instance.PlayBGM(_bgmAudioName.text);
    }

    public void StopBGMAudio()
    {
        AudioManaging.Instance.StopBGM();
    }

    public void PlaySFXAudio()
    {
        AudioManaging.Instance.PlaySFX(_sfxAudioName.text);
    }
}

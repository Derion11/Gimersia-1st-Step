using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    void Start()
    {
        // Panggil BGM menggunakan NAMA FILENYA
        if (AudioManaging.Instance != null)
        {
            AudioManaging.Instance.PlayBGM("main_menu_theme");
        }
    }

    // 2. MODIFIKASI FUNGSI INI
    public void StartTutorial()
    {
        // Panggil SFX menggunakan NAMA FILENYA
        if (AudioManaging.Instance != null)
        {
            AudioManaging.Instance.PlaySFX("button_push");
        }

        // Kode Anda yang sudah ada
        SceneManager.LoadScene("TutorialMenu"); // Ganti jika perlu
    }
    public void StartGame()
    {
        // Panggil SFX menggunakan NAMA FILENYA
        if (AudioManaging.Instance != null)
        {
            AudioManaging.Instance.PlaySFX("button_push");
            AudioManaging.Instance.StopBGM();
        }

        // Kode Anda yang sudah ada
        SceneManager.LoadScene("Game"); // Ganti jika perlu
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
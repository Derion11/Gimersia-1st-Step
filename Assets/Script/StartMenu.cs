using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    void Start()
    {
        // Panggil BGM menggunakan NAMA FILENYA
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM("main_menu_theme");
            // ^ Ganti "MainTheme" jika Anda menggunakan "CasualTheme"
        }
    }

    // 2. MODIFIKASI FUNGSI INI
    public void StartGame()
    {
        // Panggil SFX menggunakan NAMA FILENYA
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("button_push");
            AudioManager.Instance.StopBGM();
        }

        // Kode Anda yang sudah ada
        SceneManager.LoadScene(1); // Ganti jika perlu
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
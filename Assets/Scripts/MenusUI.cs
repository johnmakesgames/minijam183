using UnityEngine;
using UnityEngine.SceneManagement;

public class MenusUI : MonoBehaviour
{
    public GameObject HelpText;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void ToggleHelp()
    {
        HelpText.SetActive(!HelpText.activeSelf);
    }

    public void Play()
    {
        SceneManager.LoadScene("Lvl1");
    }

    public void Quit()
    {
        Application.Quit();
    }
}

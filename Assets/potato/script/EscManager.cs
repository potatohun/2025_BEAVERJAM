using UnityEngine;
using UnityEngine.SceneManagement;

public class EscManager : MonoBehaviour
{
    [Header("Object")]
    [SerializeField] private GameObject escPanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (escPanel.activeSelf)
            {
                escPanel.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                escPanel.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }

    public void ResumeGame()
    {
        escPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void ReStartGame()
    {
        SceneManager.LoadScene("Intro");
        Time.timeScale = 1;
    }
}

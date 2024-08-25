using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartStudentMode()
    {
        SceneManager.LoadScene("StudentMode");
    }

    public void StartTeacherMode()
    {
        SceneManager.LoadScene("TeacherMode");
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
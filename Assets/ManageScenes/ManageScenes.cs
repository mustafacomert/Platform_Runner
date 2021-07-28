using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageScenes : MonoBehaviour
{
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void LoadBeginnerLevel()
    {
        SceneManager.LoadScene("BeginnerLevel"); 
    }

    public void LoadExpertLevel()
    {
        SceneManager.LoadScene("ExpertLevel");
    }

}

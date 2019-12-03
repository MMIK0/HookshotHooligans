using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public int sceneNumber;


    public void ChangeScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}

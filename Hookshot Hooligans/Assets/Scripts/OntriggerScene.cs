using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class OntriggerScene : MonoBehaviour
{
    public GameObject player;
    public int sceneNumber;
    public void OnTriggerEnter(Collider other)
    {
        if (player)
        {
            SceneManager.LoadScene(sceneNumber);
            if (sceneNumber == 0)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

            }
        }
    }
}
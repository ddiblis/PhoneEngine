using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Michsky.DreamOS;

public class SceneLoader : MonoBehaviour
{

    [SerializeField] private MessagingManager messagingApp;

    // public void Start()
    // {
        
    //     // Force chat data to be updated
    //     messagingApp.InitializeChat();

    //     // Create storyteller
    //     messagingApp.CreateStoryTeller("gf", "SEQ_0");

    // }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void OpenLink() {
        Application.OpenURL("http://fischhaus.com/");
    }

}
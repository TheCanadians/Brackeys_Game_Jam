using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class UIController : MonoBehaviour
{

    [SerializeField] private int scene = 0;

    [SerializeField] private GameObject ui;
    [SerializeField] private TMP_Text statusText;

    public void QuitGame()
    {
        // save any game data here
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public void LoadLevel(int level)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public void SetUIState(bool state, string message)
    {
        ui.SetActive(state);
        statusText.text = message;
    }
}

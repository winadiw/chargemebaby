using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public AudioClip buttonClick;
    private void Update()
    {
        if(Application.loadedLevelName == "GameOptions1")
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                SoundManager.instance.PlaySingle(buttonClick);
                Application.LoadLevel("GameOptions2");
            }
        }
    }
    public void Matchmaking()
    {
        SoundManager.instance.PlaySingle(buttonClick);
        //Taro Random Room
    }
    public void GameLobby()
    {
        SoundManager.instance.PlaySingle(buttonClick);
        Application.LoadLevel("Launcher");
    }
    public void GameOptions()
    {
        SoundManager.instance.PlaySingle(buttonClick);
        Application.LoadLevel("GameOptions1");
    }
    public void QuitGame()
    {
        SoundManager.instance.PlaySingle(buttonClick);
        Application.Quit();
    }
    public void CancelGameOptions()
    {
        SoundManager.instance.PlaySingle(buttonClick);
        Application.LoadLevel("Menu");
    }
}

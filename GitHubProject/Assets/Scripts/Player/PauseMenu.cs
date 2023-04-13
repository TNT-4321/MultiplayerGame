using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PauseMenu : NetworkBehaviour
{
    public PlayerNetworkController networkPlayer;

    [SerializeField] private KeyCode pauseKey = KeyCode.P;
    //[SerializeField] private GameObject pauseMenu;
    //[SerializeField] private GameObject gameView;
    
    private bool paused;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) {return;}

        if(Input.GetKeyDown(pauseKey))
        {
            paused = !paused;
        }

        if(paused)
        {
            OpenMenu();
        }
        else if(!paused)
        {
            CloseMenu();
        }
    }

    public void OpenMenu()
    {
        //pauseMenu.SetActive(true);
       // gameView.SetActive(false);

        networkPlayer.playerState = PlayerNetworkController.PlayerState.Freezed;
        networkPlayer.camState = PlayerNetworkController.CamState.Freezed;
    }

    public void CloseMenu()
    {
       // gameView.SetActive(true);
        //pauseMenu.SetActive(false);

        networkPlayer.playerState = PlayerNetworkController.PlayerState.Normal;
        networkPlayer.camState = PlayerNetworkController.CamState.FPS;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void MenuContent()
    {

    }

    public void ButtonTest()
    {
        Debug.Log("Was pressed");
    }
}

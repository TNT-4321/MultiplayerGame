using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PauseMenu : NetworkBehaviour
{
    public PlayerNetworkController networkPlayer;

    [SerializeField] private KeyCode pauseKey = KeyCode.P;
    private GameObject pauseMenu;
    private GameObject gameView;
    private GameUI gameUI;
    
    private bool paused;

    // Start is called before the first frame update
    void Start()
    {
        //Set references
        gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();
        pauseMenu = GameObject.Find("PauseMenu");
        gameView = GameObject.Find("GameView");

        CloseMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) {return;}

        if(Input.GetKeyDown(pauseKey) && !paused)
        {
            paused = !paused;

            OpenMenu();
        }
        else if(Input.GetKeyDown(pauseKey) && paused)
        {
            paused = !paused;

            CloseMenu();
        }
    }

    public void OpenMenu()
    {
        pauseMenu.SetActive(true);
        gameView.SetActive(false);

        networkPlayer.playerState = PlayerNetworkController.PlayerState.Freezed;
        networkPlayer.camState = PlayerNetworkController.CamState.Freezed;
    }

    public void CloseMenu()
    {
        gameView.SetActive(true);
        pauseMenu.SetActive(false);

        networkPlayer.playerState = networkPlayer.lastPlayerState;
        networkPlayer.camState = networkPlayer.lastCamState;
    }

    private void MenuContent()
    {

    }

    public void ButtonTest()
    {
        Debug.Log("Was pressed");
    }
}

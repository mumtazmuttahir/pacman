﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InitailMenu : MonoBehaviour
{

    #region public_variables
    public bool isPlayer1Selected = true;
    public Button player1Button;
    public Button player2Button;
    public Button playButton;
    public LoadScene leadScene;

    public Text playerSelectionIndicator;
    #endregion

    public void SelectPlayer1 () {

        isPlayer1Selected = true;

        playerSelectionIndicator.transform.localPosition = new Vector3 (playerSelectionIndicator.transform.localPosition.x,
                                                                        player1Button.transform.localPosition.y,
                                                                        playerSelectionIndicator.transform.localPosition.z);
    }

    public void SelectPlayer2 () {

        isPlayer1Selected = false;

        playerSelectionIndicator.transform.localPosition = new Vector3 (playerSelectionIndicator.transform.localPosition.x,
                                                                        player2Button.transform.localPosition.y,
                                                                        playerSelectionIndicator.transform.localPosition.z);
    }

    public void GoToGame () {
        if (isPlayer1Selected) {
             leadScene.LoadSceneMethod("Level1");
          //  SceneManager.LoadScene ("Level1") ;
        } else {
            Debug.Log ("Loading separate Scene");
         //   SceneManager.LoadScene ("Level1") ;
            leadScene.LoadSceneMethod("Level1");
        }
    }
  
}

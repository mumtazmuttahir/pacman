using System.Collections;
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
        int scenNumber = Random.Range(1,3);
        scenNumber = (scenNumber==3)?2:scenNumber;
        SceneManager.LoadScene(scenNumber);
    }
  
}

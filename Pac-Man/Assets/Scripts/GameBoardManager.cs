using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardManager : MonoBehaviour
{

    #region public_variables
    public static int boardWidth = 28;
    public static int boardHeight = 36;

    public GameObject[,] gameBoard = new GameObject[boardWidth, boardHeight];

    public int totalPellets = 0;
    public int score = 0;
    public int pacManLife = 5;

    public AudioClip backgroundAudioNormal;
    public AudioClip backgroundAudioScared;
    #endregion


    #region private_variable
    GameObject pacMan;
    GameObject[] ghosts;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

        foreach (GameObject obj in objects) {
            Vector2 objPos = obj.transform.position;

            if (obj.gameObject.name != "PacMan" &&
                    obj.gameObject.name != "Nodes" &&
                    obj.gameObject.name != "NonNodes" &&
                    obj.gameObject.name != "Maze" &&
                    obj.gameObject.name != "Pellets" &&
                    obj.gameObject.tag != "GhostHome"  &&
                    obj.gameObject.tag != "Ghost")  {

                if (obj.GetComponent<Tile>() != null) {

                    if (obj.GetComponent<Tile>().isPellet || obj.GetComponent<Tile>().isSuperPellet) {

                        totalPellets++;

                    }

                }

                gameBoard[(int)objPos.x, (int)objPos.y] = obj;
            }
        }

        pacMan = GameObject.FindGameObjectWithTag("PacMan");
        ghosts = GameObject.FindGameObjectsWithTag("Ghost");

    }

    //PacMan respawn
    public void PacManRespawn() {

        if(pacManLife > 0) {
            Debug.Log ("Pacman lives left = " + pacManLife);

            pacMan.GetComponent<PacMan>().RestartPacMan();
            pacManLife--;
        }
        else {
            //gameover
            pacMan.SetActive(false);
            //UI message
        }

    }

    public void Restart () {
        
        pacMan.GetComponent<PacMan>().RestartPacMan ();

        foreach (GameObject ghost in ghosts) {
            ghost.GetComponent<Ghost>().RestartGhost ();
        }
    }
}

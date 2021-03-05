using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoardManager : MonoBehaviour
{

    #region public_variables
    public static int boardWidth = 28;
    public static int boardHeight = 36;

    public GameObject[,] gameBoard = new GameObject[boardWidth, boardHeight];

    public int totalPellets = 0;
    public int score = 0;
    public int pacManLife = 5;

    public AudioClip backgroundAudioIntro;
    public AudioClip backgroundAudioNormal;
    public AudioClip backgroundAudioScared;
    public AudioClip backgroundAudioPacManDeath;

    public Text PlayerText;
    public Text ReadyText;
    #endregion


    #region private_variable
    GameObject pacMan;
    GameObject[] ghosts;
    bool didStartDeath = false;
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
                    obj.gameObject.tag != "Ghost" &&
                    obj.gameObject.tag != "UI_Elements")  {

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

        this.gameObject.transform. GetComponent<AudioSource>().clip = backgroundAudioIntro;
        this.gameObject.transform. GetComponent<AudioSource>().Play();

        StartGame ();

    }

    void StartGame () {
        pacMan.GetComponent<SpriteRenderer>().enabled = false;
        pacMan.GetComponent<PacMan>().canMove = false;

        foreach (GameObject ghost in ghosts) {
            ghost.GetComponent<SkinnedMeshRenderer>().enabled = false;
            ghost.GetComponent<Ghost>().canMove = false;
        }

        StartCoroutine(startGameAfterSomeTime(2.25f));
    }

    IEnumerator startGameAfterSomeTime (float _delay) {
        yield return new WaitForSeconds (_delay);

        pacMan.GetComponent<SpriteRenderer>().enabled = true;
        foreach (GameObject ghost in ghosts) {
            ghost.GetComponent<SkinnedMeshRenderer>().enabled = true;
        }

        PlayerText.GetComponent<Text>().enabled = false;
        

        StartCoroutine(startGameNowAs(2.25f));
    }

    IEnumerator startGameNowAs (float _delay) {

        yield return new WaitForSeconds (_delay);

        pacMan.GetComponent<PacMan>().canMove = true;
        foreach (GameObject ghost in ghosts) {
            ghost.GetComponent<Ghost>().canMove = true;
        }

        ReadyText.GetComponent<Text>().enabled = false;

        this.gameObject.transform. GetComponent<AudioSource>().Stop();
        this.gameObject.transform. GetComponent<AudioSource>().clip = backgroundAudioNormal;
        this.gameObject.transform. GetComponent<AudioSource>().Play();

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

    public void StartPacManDeath () {
        if (!didStartDeath) {

            didStartDeath = true;

            foreach (GameObject ghost in ghosts) {
                ghost.GetComponent<Ghost>().canMove = false;
            }

            pacMan.GetComponent<PacMan>().canMove = false;

            //Stop any animation on Pacman

            this.gameObject.transform. GetComponent<AudioSource>().Stop ();

            StartCoroutine (createDeathAfter (StaticsAndConstants.CreateDeathAfterDelay));
        }
    }

    IEnumerator createDeathAfter (float _delay) {

        yield return new WaitForSeconds(_delay);
        
        foreach (GameObject ghost in ghosts) {
            ghost.GetComponent<SkinnedMeshRenderer>().enabled = false;
        }

        StartCoroutine (createDeathAnimation (StaticsAndConstants.CreateDeathAnimationDelay));
    }

    IEnumerator createDeathAnimation (float _delay) {

        pacMan.transform.localScale = new Vector3 (1,1,1);
        pacMan.transform.localRotation = Quaternion.Euler (0,0,0);

        //Run required animation

        this.gameObject.transform. GetComponent<AudioSource>().clip = backgroundAudioPacManDeath;
        this.gameObject.transform. GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(_delay);

        StartCoroutine (createRestart (StaticsAndConstants.CreateRestartDelay));

    }

    IEnumerator createRestart (float _delay) {
        
        PlayerText.GetComponent<Text>().enabled = true;
        ReadyText.GetComponent<Text>().enabled = true;
        
        pacMan.GetComponent<SpriteRenderer>().enabled = false;

        this.gameObject.transform. GetComponent<AudioSource>().Stop ();

        yield return new WaitForSeconds(_delay);

        StartCoroutine (createRestartShowObjects (StaticsAndConstants.CreateRestartDelay));

    }

    IEnumerator createRestartShowObjects (float _delay) {

        PlayerText.GetComponent<Text>().enabled = false;

        pacMan.GetComponent<SpriteRenderer>().enabled = true;
        pacMan.GetComponent<PacMan>().MoveToStartPosition();
        foreach (GameObject ghost in ghosts) {
            ghost.GetComponent<SkinnedMeshRenderer>().enabled = true;
            ghost.GetComponent<Ghost>().MoveToStartPosition();
        }

        yield return new WaitForSeconds(_delay);

        Restart ();

    }

    public void Restart () {

        ReadyText.GetComponent<Text>().enabled = false;
        
        pacMan.GetComponent<PacMan>().RestartPacMan ();

        GameObject[] reqGhosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in reqGhosts) {
            ghost.GetComponent<Ghost>().RestartGhost ();
        }

        this.gameObject.transform. GetComponent<AudioSource>().clip = backgroundAudioNormal;
        this.gameObject.transform. GetComponent<AudioSource>().Play();

        didStartDeath = false;
    }
}

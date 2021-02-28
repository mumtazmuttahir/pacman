using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardManager : MonoBehaviour
{

    public static int boardWidth = 28;
    public static int boardHeight = 36;

    public GameObject[,] gameBoard = new GameObject[boardWidth, boardHeight];

    // Start is called before the first frame update
    void Start()
    {
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

        foreach (GameObject obj in objects) {
            Vector2 objPos = obj.transform.position;

            if (obj.gameObject.name != "PacMan") {
                gameBoard[(int)objPos.x, (int)objPos.y] = obj;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

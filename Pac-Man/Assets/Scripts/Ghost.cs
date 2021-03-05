using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    #region enums
    public enum Mode  { Chase, Scatter, Scared, Consumed}
    [SerializeField]
    Mode currentMode = Mode.Scatter;

    public enum GhostType {Red, Pink, Blue, Orange}
    [SerializeField] 
    GhostType ghostType = GhostType.Red;
    #endregion

    #region private_variables
    private GameObject pacMan;
    private GameObject tile;
    private int modeChangeIteration;
    private float modeChangeTimer = 0;
    private float ghostReleaseTimer = 0;
    private Rect ghostRect, pacManRect;
    private AudioSource collisionFX;
    private AudioSource bgAudio;
    private float scaredModeTimer = 0;
    private float blinkingTimer = 0;
    private bool isScaredModeWhite = false;
    private Mode previousMode;
    #endregion

    #region public_variables
    public float moveSpeed = 5.9f; //ghost speed
    public float normalMoveSpeed = 5.9f;
    public float previousMoveSpeed = 2.9f;
    public float scaredModeMoveSpeed = 2.9f;
    public float consumedModeMoveSpeed = 15.0f;
    public Node startingPosition;
    public Node currentNode, targetNode, previousNode, ghostHouse;
    public Vector2 direction, nextDirection;
    public bool isInGhostHouse = false;
    public float currentGhostReleaseTimer = 5;
    public int[] scatterModetimer;
    public int[] chaseModeTimer;
    public int scaredModeDuration = 10;
    public int blinkingBackToOriginal = 7;
    public Node homeNode;
    public GameBoardManager gameBoardManager;
    public bool canMove = true;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        collisionFX = this.GetComponent<AudioSource>();

        bgAudio = GameObject.Find("GameManager").GetComponent<AudioSource>();

        pacMan = GameObject.FindGameObjectWithTag("PacMan");
        
        if(isInGhostHouse) {
            direction = Vector2.up;
            targetNode = currentNode.neighbors[StaticsAndConstants.ZerothNeighbor];
        }
        else {
            direction = Vector2.left;
            targetNode = NextNode();
        }
 
        previousNode = currentNode;

        updateGhostOrientation ();

        modeChangeIteration = StaticsAndConstants.ModeChangeIteration;

        Random rnd = new Random();
        currentGhostReleaseTimer = Random.Range(1, 7);
        
    }

    public void MoveToStartPosition () {

        if (this.gameObject.name != "redGhost") {
            isInGhostHouse = true;
        }

        this.gameObject.transform.position = startingPosition.transform.position;

        if (isInGhostHouse) {

            direction = Vector2.up;

        } else {

            direction = Vector2.left;
            
        }


        updateGhostOrientation ();
	}

    public void RestartGhost () {
            Debug.Log("Ghost Name = " + this.gameObject.name);

        canMove = true;

        // if (this.gameObject.name != "redGhost" || 
        //         this.gameObject.name != "pinkGhost" ||
        //             this.gameObject.name != "blueGhost" ||
        //                 this.gameObject.name != "orangeGhost") {
        //                     this.gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
        // }

        

        currentMode = Mode.Scatter;
        currentNode = startingPosition;

        moveSpeed = normalMoveSpeed;
        previousMoveSpeed = StaticsAndConstants.SpeedResetToZero;


        ghostReleaseTimer = (float)StaticsAndConstants.TimerResetToZero;
        modeChangeIteration = StaticsAndConstants.ModeChangeIteration;
        modeChangeTimer = StaticsAndConstants.ModeChangeTimer;



        if (isInGhostHouse) {

            direction = Vector2.up;
            targetNode = currentNode.neighbors[StaticsAndConstants.ZerothNeighbor];

        } else {

            direction = Vector2.left;
            targetNode = NextNode ();
            
        }

        previousNode = currentNode;
        updateGhostOrientation ();

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (canMove) {
            //Ghost Mode
            ModeUpdate ();
            //Move Ghost
            Move ();
            //Release Ghost from the house
            ReleaseGhost ();
            //Check ghost collision with Pacman
            CheckCollision ();
            //Check ghost if it is in the House
            checkIfInGhostouse ();

        }
        
        
    }

    void Move() {
        if(targetNode!=currentNode 
                && targetNode!=null 
                && !isInGhostHouse)
        {
            if(OverShotTarget())
            {
                currentNode = targetNode;
                transform.localPosition = currentNode.transform.position;
                GameObject otherPortal = GetPortal(currentNode.transform.position);
                if(otherPortal!=null)
                {
                    transform.localPosition = otherPortal.transform.position;
                    currentNode = otherPortal.GetComponent<Node>();
                }
                targetNode = NextNode();
                previousNode = currentNode;
                currentNode = null;
                updateGhostOrientation ();
            }
            else
            {
                transform.localPosition += (Vector3)direction * moveSpeed * Time.deltaTime;
            }
        }
    }

    void updateGhostOrientation () {

        if (currentMode != Mode.Scared && currentMode != Mode.Consumed) {
            if (direction == Vector2.up) {
                this.gameObject.transform.localRotation = Quaternion.Euler (-90.0f, -90f, 90f);
            } else if (direction == Vector2.down) {
                this.gameObject.transform.localRotation = Quaternion.Euler (-270.0f, -90f, 90f);
            } else if (direction == Vector2.right) {
                this.gameObject.transform.localRotation = Quaternion.Euler (-180.0f, -90f, 90f);
            } else {
                this.gameObject.transform.localRotation = Quaternion.Euler (0f, -90f, 90f);
            }
        } else if (currentMode == Mode.Scared) {
            if (direction == Vector2.up) {
                this.gameObject.transform.localRotation = Quaternion.Euler (-90.0f, -90f, 90f);
            } else if (direction == Vector2.down) {
                this.gameObject.transform.localRotation = Quaternion.Euler (-270.0f, -90f, 90f);
            } else if (direction == Vector2.right) {
                this.gameObject.transform.localRotation = Quaternion.Euler (-180.0f, -90f, 90f);
            } else {
                this.gameObject.transform.localRotation = Quaternion.Euler (0f, -90f, 90f);
            }
        } else if (currentMode == Mode.Consumed) {
            if (direction == Vector2.up) {
                this.gameObject.transform.localRotation = Quaternion.Euler (-90.0f, -90f, 90f);
            } else if (direction == Vector2.down) {
                this.gameObject.transform.localRotation = Quaternion.Euler (-270.0f, -90f, 90f);
            } else if (direction == Vector2.right) {
                this.gameObject.transform.localRotation = Quaternion.Euler (-180.0f, -90f, 90f);
            } else {
                this.gameObject.transform.localRotation = Quaternion.Euler (0f, -90f, 90f);
            }
        } 

        
    }

    Node GetNodeAtPosition (Vector2 position) {

        tile = GameObject.Find("GameManager").GetComponent<GameBoardManager>().gameBoard[(int)position.x, (int)position.y];
        
        return (tile.GetComponent<Node>() != null)? tile.GetComponent<Node> ():null;
        
    } 

    void ChangeMode(Mode mode)  {
        

        if (currentMode == Mode.Scared) {
            moveSpeed = previousMoveSpeed;
        }

        if (mode == Mode.Scared) {
            previousMoveSpeed = moveSpeed;
            moveSpeed = scaredModeMoveSpeed;
        }

        if(currentMode != mode) {
            previousMode = currentMode;
            currentMode = mode;
        }
    }

    public void StartScaredMode () {

        if (currentMode != Mode.Consumed) {

            scaredModeTimer = StaticsAndConstants.ResetToZero;

            bgAudio.clip = GameObject.Find("GameManager").transform.GetComponent<GameBoardManager>().backgroundAudioScared;
            bgAudio.Play();
            ChangeMode(Mode.Scared);

        }
        
        
    }

    void ModeUpdate () {

        if(currentMode != Mode.Scared) {

            modeChangeTimer += Time.deltaTime;
            SetMode(modeChangeIteration);

        } else if (currentMode == Mode.Scared) {

            scaredModeTimer += Time.deltaTime;

            if (scaredModeTimer >= scaredModeDuration) {
                
                bgAudio.clip = GameObject.Find("GameManager").transform.GetComponent<GameBoardManager>().backgroundAudioNormal;
                bgAudio.Play();
                
                scaredModeTimer = StaticsAndConstants.ResetToZero;
                ChangeMode(previousMode);

            }

            if (scaredModeTimer >= blinkingTimer) {
                blinkingTimer += Time.deltaTime;

                if (blinkingTimer >= 0.1f) {
                    blinkingTimer = StaticsAndConstants.ResetToZero;

                    if (isScaredModeWhite) {
                        //turn blue
                        isScaredModeWhite = false;
                    } else {
                        //turn white
                        isScaredModeWhite = true;
                    }
                }
            }
        }
    }
    void SetMode (int iteration) {

        if(currentMode == Mode.Scatter 
                && (modeChangeTimer > scatterModetimer[iteration - 1])) {
            
            ChangeMode(Mode.Chase);
            Reset();
        }
        else if(currentMode == Mode.Chase 
                &&  (modeChangeTimer>scatterModetimer[iteration-1]))
        {
            modeChangeIteration = iteration;
            ChangeMode(Mode.Scatter);
            Reset();
        }   
    }

    GameObject getTileAtPosition (Vector2 _position) {
        
        int tileX = Mathf.RoundToInt (_position.x);
        int tileY = Mathf.RoundToInt (_position.y);

        GameObject _tile = GameObject.Find("GameManager").GetComponent<GameBoardManager>().gameBoard[(int)tileX , (int)tileY];

        if (_tile != null) {
            return _tile;
        }

        return null;
    }

    GameObject GetPortal (Vector2 pos) {

        tile = GameObject.Find("GameManager").GetComponent<GameBoardManager>().gameBoard[(int)pos.x, (int)pos.y];
        return (tile.GetComponent<Tile>().isPortal) ? tile.GetComponent<Tile>().receiverPortal : null;
        
    }

    /*** Utilities */
    #region  utilities_methods
    float LengthFromNode (Vector2 targetPos) 
        => (targetPos - (Vector2)previousNode.transform.position).sqrMagnitude;
    
    bool OverShotTarget () {

        float nodeToTarget = LengthFromNode(targetNode.transform.position);
        float nodeToSelf = LengthFromNode(this.transform.position);
        return nodeToSelf >  nodeToTarget;
    }
    float GetDistance (Vector2 firstPos, Vector2 secPosition) 
    {
        float distanceX = firstPos.x-secPosition.x;
        float distanceY = firstPos.y-secPosition.y;
        return Mathf.Sqrt((distanceX*distanceX) + (distanceY*distanceY));
    }
    void Reset() =>  modeChangeTimer = StaticsAndConstants.TimerResetToZero;
    #endregion
    
    Node NextNode () {

        Vector2 targetTile = Vector2.zero;

        if(currentMode == Mode.Chase){

            targetTile = GetTargetTile();
        }
        else if (currentMode == Mode.Scatter) {

            targetTile = homeNode.transform.position;

        } else if (currentMode == Mode.Scared) {

            targetTile = GetRandomTile();

        } else if (currentMode == Mode.Consumed) {

            targetTile = ghostHouse.transform.position;

        }

        

        Node moveToNode = null;
        Node[] foundNodes = new Node[StaticsAndConstants.FoundNodesCount];
        Vector2[] foundNodePositions = new Vector2[StaticsAndConstants.FoundNodePositionsCount];
        int nodeCounter =0;
        for(int i = 0; i < currentNode.neighbors.Length; i++) {
            if(currentNode.validDirections[i] != (direction * -1)){
                
                if (currentMode != Mode.Consumed) {
                    GameObject _tile = getTileAtPosition (currentNode.transform.position);
                    if (_tile.transform.GetComponent<Tile>().isGhostInHouseEntrance == true) {
                        
                        if (currentNode.validDirections[i] != Vector2.down) {

                            foundNodes[nodeCounter] = currentNode.neighbors[i];
                            foundNodePositions[nodeCounter ] = currentNode.validDirections[i];
                            nodeCounter++;

                        } 

                    } else {
                            foundNodes[nodeCounter] = currentNode.neighbors[i];
                            foundNodePositions[nodeCounter ] = currentNode.validDirections[i];
                            nodeCounter++;
                    }

                } else {
                            foundNodes[nodeCounter] = currentNode.neighbors[i];
                            foundNodePositions[nodeCounter ] = currentNode.validDirections[i];
                            nodeCounter++;
                }
            }
        }
        if(foundNodes.Length == StaticsAndConstants.OnlyOneNode)
        {
            moveToNode=foundNodes[StaticsAndConstants.ZerothNode];
            direction = foundNodePositions[StaticsAndConstants.ZerothNodePosition];
        }
        else if(foundNodes.Length > StaticsAndConstants.OnlyOneNode)
        {
            float leastDistance = StaticsAndConstants.LeastDistance;
            for(int i=0; i<foundNodes.Length; i++)
            {
                if(foundNodePositions[i]!=Vector2.zero)
                {
                    float dis = GetDistance(foundNodes[i].transform.position, targetTile);
                    if(dis<leastDistance)
                    {
                        leastDistance = dis;
                        moveToNode= foundNodes[i];
                        direction=foundNodePositions[i];
                    }
                }
            }
        }

        return moveToNode;
    }

    void ReleaseGhost() {

        ghostReleaseTimer += Time.deltaTime;
        if(ghostReleaseTimer>currentGhostReleaseTimer)
            ReleaseCurrentGhost();
    }
    
    void ReleaseCurrentGhost() {
        if(isInGhostHouse) 
            isInGhostHouse = false;
    }

    Vector2 GetTargetTile () =>
         (ghostType== GhostType.Red)?GetRedGhostTargetTile(): GetGhostTargetTile(ghostType);

    Vector2 GetRandomTile () {

        int xPos = Random.Range (0, 28);
        int yPos = Random.Range (0, 36);

        return new Vector2 (xPos, yPos);

    }

    Vector2 GetRedGhostTargetTile () 
        => new Vector2(Mathf.RoundToInt(pacMan.transform.localPosition.x), Mathf.RoundToInt(pacMan.transform.localPosition.y));
  
    Vector2 GetPinkGhostTargetTile () {
        // four head of pacman 
        // taking account position and orientation
        Vector2 pacManOrien = pacMan.GetComponent<PacMan>().orientation;
        int posX= Mathf.RoundToInt(pacMan.transform.localPosition.x);
        int posY = Mathf.RoundToInt(pacMan.transform.localPosition.y);
        Vector2 pacManTile = new Vector2 (posX, posY);
        
        return pacManTile + (4*pacManOrien);
    }

    Vector2 GetGhostTargetTile (GhostType m) {
        // four head of pcman 
        // taking account position and orientation
        Vector2 targetTile = Vector2.zero;
        Vector2 pacManOrien = pacMan.GetComponent<PacMan>().orientation;
        int posX= Mathf.RoundToInt(pacMan.transform.localPosition.x);
        int posY = Mathf.RoundToInt(pacMan.transform.localPosition.y);
        Vector2 pacManTile = new Vector2 (posX, posY);
        if(m == GhostType.Pink)
        {
            targetTile =  pacManTile + (4 * pacManOrien);
        }
        else if(m == GhostType.Blue)
        {
            targetTile = pacManTile + (2 * pacManOrien);
            Vector2 tempRedGhostPos = GameObject.Find("redGhost").transform.localPosition;
            tempRedGhostPos = new Vector2 (tempRedGhostPos.x,tempRedGhostPos.y);
            float distance  = GetDistance(tempRedGhostPos, targetTile);
            distance = distance * 2;
            targetTile = new Vector2 (tempRedGhostPos.x + distance, tempRedGhostPos.y + distance);
        }
        else if(m == GhostType.Orange)
        {
            float distance  = GetDistance(transform.localPosition, pacMan.transform.localPosition);
            if(distance > 0){
                targetTile = new Vector2 (posX,posY);
            }
            else {
                targetTile = homeNode.transform.position;
            }
 
        }

        return targetTile;
    }

    void CheckCollision() {

        ghostRect = new Rect(this.transform.position, this.transform.GetComponent<SkinnedMeshRenderer>().bounds.size/4);
        pacManRect = new Rect(pacMan.transform.position, pacMan.transform.GetComponent<SpriteRenderer>().bounds.size/4);

        int overlap = 0;

        if(ghostRect.Overlaps(pacManRect))
        {
            // Debug.Log ("Overlaps called");

            if (overlap == 0) {
                if (currentMode == Mode.Scared) {
                    //Ghost gets consumed and goes back to the House
                    // Debug.Log ("ghost Scared");
                    consumeGhost ();
                } else {
                    //Pacman dies and respawns
                    // Debug.Log ("pacman respawned");
                    if (currentMode != Mode.Scared) {
                        GameObject.Find("GameManager").
                                GetComponent<GameBoardManager>().
                                    StartPacManDeath ();
                    }
                    
                }
            }

            overlap++;
            
        }
    }

    void consumeGhost () {
        currentMode = Mode.Consumed;
        previousMoveSpeed = moveSpeed;
        moveSpeed = consumedModeMoveSpeed;
        updateGhostOrientation ();
    }

    void checkIfInGhostouse () {
        if (currentMode == Mode.Consumed) {
            
            GameObject tile = getTileAtPosition (this.gameObject.transform.position);

            if (tile != null) {
                if (tile.transform.GetComponent<Tile>() != null) {
                    if (tile.transform.GetComponent<Tile>().isGhostHouse) {
                        
                        moveSpeed = normalMoveSpeed;

                        Node node = GetNodeAtPosition (this.gameObject.transform.position);

                        if (node != null) {
                            currentNode = node;

                            direction = Vector2.up;
                            targetNode = currentNode.neighbors [StaticsAndConstants.ZerothNeighbor];
                            previousNode = currentNode;

                            currentMode = Mode.Chase;

                            updateGhostOrientation ();

                        }
                    }
                }
            }
        }
    }
}

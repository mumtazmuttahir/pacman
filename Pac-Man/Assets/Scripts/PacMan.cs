using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : MonoBehaviour {

	#region private_variables
	[SerializeField]
	private float speed = 4.0f;
	private Vector2 direction = Vector2.zero;
	private Vector2 nextDirection;
	private Node currentNodeOnWhichPacmanIsStanding, previousNode, targetNode;
	private int pelletsConsumed = 0;
	private GameObject pellet;
	private Node initPosition;
	private bool isChomp1Played = false;
	private Node pacmanStartPosition;
	#endregion

	#region public_variables
	public Vector2 orientation;
	public AudioSource audiosSource;
	public AudioClip chomp1, chomp2;
	//public AudioClip moveFX, munchFX, winFX;
	public bool canMove = true;
	#endregion


	

	// Use this for initialization
	void Start () {

		//Assigning Pacman speed
		speed = StaticsAndConstants.PacManSpeed;

		//Audio Source
		audiosSource = this.GetComponent<AudioSource>();

		Node node = getNodeAtPositionOfPacman(this.transform.localPosition);

		initPosition = node;

		//Debug.Log ("this.transform.localPosition is = " +this.transform.localPosition);
		if (node != null) {
			currentNodeOnWhichPacmanIsStanding = node;
		}

		//Start the PacMan movement
		direction = Vector2.left;
		changePacmanPosition (direction);
	}

	public void MoveToStartPosition () {

		transform.position = initPosition.transform.position;

		direction = Vector2.left;
		orientation = Vector2.left;

	}

	public void RestartPacMan() {

		canMove = true;
		//this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
		currentNodeOnWhichPacmanIsStanding = initPosition;
		nextDirection = Vector2.zero;

		changePacmanPosition (direction);
	}

	void PlayChompSfx () {
		if (isChomp1Played) {
			audiosSource.PlayOneShot (chomp1);
			isChomp1Played = false;
		} else {
			audiosSource.PlayOneShot (chomp2);
			isChomp1Played = true;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (canMove) {

			checkPacmanDirectionInput ();

			movePacman ();

			updateOrientation ();

			consumePellet ();

		}

		
	}

	private void checkPacmanDirectionInput () {

		if (Input.GetKeyDown (KeyCode.LeftArrow)) {

			changePacmanPosition (Vector2.left);

		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {

			changePacmanPosition (Vector2.right);

		} else if (Input.GetKeyDown (KeyCode.UpArrow)) {

			changePacmanPosition (Vector2.up);

		} else if (Input.GetKeyDown (KeyCode.DownArrow)) {

			changePacmanPosition (Vector2.down);
		}
	}

	public void MoveUP () {
		if (canMove) {
			changePacmanPosition (Vector2.up);
		}
	}
	public void MoveDown () {
		if (canMove) {
			changePacmanPosition (Vector2.down);
		}
	}
	public void MoveRight () {
		if (canMove) {
			changePacmanPosition (Vector2.right);
		}
	}
	public void MoveLeft () {
		if (canMove) {
			changePacmanPosition (Vector2.left);
		}
	}

	private void movePacman () {

		if (targetNode != currentNodeOnWhichPacmanIsStanding && targetNode != null ) {

			if (nextDirection == (direction * (-1))) {
				direction *= -1;
				Node tempNode = targetNode;
				targetNode = previousNode;
				previousNode = tempNode;
			}

			if (isPacmanOverShotTheTarget()) {
				currentNodeOnWhichPacmanIsStanding = targetNode;
				this.gameObject.transform.localPosition = (Vector2)currentNodeOnWhichPacmanIsStanding.transform.position;

				GameObject oppositePortal = getPortal (currentNodeOnWhichPacmanIsStanding.transform.position);

				if (oppositePortal != null) {
					this.gameObject.transform.localPosition = oppositePortal.transform.position;
					currentNodeOnWhichPacmanIsStanding = oppositePortal.GetComponent<Node>();
				}

				Node moveToNextNode = pacmanCanMove (nextDirection);

				if (moveToNextNode != null) {
					direction = nextDirection;
				}

				if (moveToNextNode == null) {
					moveToNextNode = pacmanCanMove (direction);
				}

				if (moveToNextNode != null) {
					targetNode = moveToNextNode;
					previousNode = currentNodeOnWhichPacmanIsStanding;
					currentNodeOnWhichPacmanIsStanding = null;
				} else {
					direction = Vector2.zero;
				}

			} else {

				transform.localPosition += (Vector3)(direction * speed) * Time.deltaTime;
			
			}
		}

	}

	private void updateOrientation () {

		if (direction == Vector2.left) {

			orientation = Vector2.left;
			transform.localScale = new Vector3 (-1, 1, 1);
			transform.localRotation = Quaternion.Euler (0, 0, 0);

		} else if (direction == Vector2.right) {

			orientation = Vector2.right;
			transform.localScale = new Vector3 (1, 1, 1);
			transform.localRotation = Quaternion.Euler (0, 0, 0);

		} else if (direction == Vector2.up) {

			orientation = Vector2.up;
			transform.localScale = new Vector3 (1, 1, 1);
			transform.localRotation = Quaternion.Euler (0, 0, 90);

		} else if (direction == Vector2.down) {

			orientation = Vector2.down;
			transform.localScale = new Vector3 (1, 1, 1);
			transform.localRotation = Quaternion.Euler (0, 0, 270);
		}
	}

	private Node getNodeAtPositionOfPacman (Vector2 _pacmanPos) {
		GameObject pellet = GameObject
								.Find("GameManager")
								.GetComponent<GameBoardManager>()
								.gameBoard[(int)_pacmanPos.x, (int)_pacmanPos.y];

		if (pellet != null) {
			return pellet.GetComponent<Node>();
		} else {
			return null;
		}
	}

	private void changePacmanPosition (Vector2 _dir) {
		if (_dir != direction){
			nextDirection = _dir;
		}

		if (currentNodeOnWhichPacmanIsStanding != null) {
			Node moveToNextNode = pacmanCanMove (_dir);

			if (moveToNextNode != null) {
				direction = _dir;
				targetNode = moveToNextNode;
				previousNode = currentNodeOnWhichPacmanIsStanding;
				currentNodeOnWhichPacmanIsStanding = null;
			}
		}
	}

	private Node pacmanCanMove (Vector2 _pacmanPos) {
		Node moveToNextNode = null;

		for (int index = 0; index < currentNodeOnWhichPacmanIsStanding.neighbors.Length; index++) {
			if (currentNodeOnWhichPacmanIsStanding.validDirections[index] == _pacmanPos) {
				moveToNextNode = currentNodeOnWhichPacmanIsStanding.neighbors[index];
				break;
			}
		}

		return moveToNextNode;
	}

	private void moveToNode (Vector2 _nextPos) {
		Node moveToNextNode = pacmanCanMove (_nextPos);

		if (currentNodeOnWhichPacmanIsStanding != null) {
			this.gameObject.transform.localPosition = moveToNextNode.transform.position;
			currentNodeOnWhichPacmanIsStanding = moveToNextNode;
		}
	}

	private float distanceForCurrentNode (Vector2 _targetNodePosition) {

		Vector2 vector = _targetNodePosition - (Vector2)previousNode.transform.position;
		return vector.sqrMagnitude;
	}

	private bool isPacmanOverShotTheTarget () {
		float nodeToTarget = distanceForCurrentNode (targetNode.transform.position);
		float nodeToPacman = distanceForCurrentNode (this.gameObject.transform.position);

		return nodeToPacman > nodeToTarget;
	}

	private GameObject getPortal (Vector2 _pacmanPosition) {
		GameObject tile = GameObject
							.Find("GameManager")
							.GetComponent<GameBoardManager>()
							.gameBoard[(int)_pacmanPosition.x,(int)_pacmanPosition.y];

		if (tile != null) {
			if (tile.GetComponent<Tile>() != null) {
				if (tile.GetComponent<Tile>().isPortal) {
					GameObject oppositePortal = tile.GetComponent<Tile>().receiverPortal;
					return oppositePortal;
				}
			}
		}

		return null;
	}

	private GameObject getTileAtPacmanPosition (Vector2 _pacmanPosition) {
		int tilePositionX = Mathf.RoundToInt (_pacmanPosition.x);
		int tilePositionY = Mathf.RoundToInt (_pacmanPosition.y);

		GameObject tile = GameObject
							.Find("GameManager")
							.GetComponent<GameBoardManager>()
							.gameBoard[(int)_pacmanPosition.x,(int)_pacmanPosition.y];

		if (tile != null) {
			return tile;
		}

		return null;
	}

	private void consumePellet () {
		GameObject pellet = getTileAtPacmanPosition (this.gameObject.transform.position);
		if (pellet != null) {
			Tile tile = pellet.GetComponent<Tile>();
			if (tile != null) {
				if (!tile.isPelletConsumed && (tile.isPellet || tile.isSuperPellet)) {
					pellet.GetComponent<SpriteRenderer>().enabled = false;
					tile.isPelletConsumed = true;
					GameObject.Find("GameManager").GetComponent<GameBoardManager>().ScoreChange(1);

					pelletsConsumed++;
					PlayChompSfx ();

					if (tile.isSuperPellet) {
						GameObject[] ghosts = GameObject. FindGameObjectsWithTag("Ghost");

						foreach (GameObject o in ghosts) {
							o.GetComponent<Ghost>().StartScaredMode();
						}
					}
				}
			}
		}
	}

}

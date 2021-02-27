using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : MonoBehaviour {

	#region private_variables
	[SerializeField]
	private float speed = 4.0f;
	private Node currentNodeOnWhichPacmanIsStanding;
	#endregion


	private Vector2 direction = Vector2.zero;

	// Use this for initialization
	void Start () {
		Node node = getNodeAtPositionOfPacman(this.transform.localPosition);
		//Debug.Log ("this.transform.localPosition is = " +this.transform.localPosition);
		if (node != null) {
			currentNodeOnWhichPacmanIsStanding = node;
			Debug.Log ("Node is = " + node.name);
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		CheckInput ();

		//Move ();

		UpdateOrientation ();
	}

	void CheckInput () {

		if (Input.GetKeyDown (KeyCode.LeftArrow)) {

			direction = Vector2.left;
			moveToNode(direction);

		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {

			direction = Vector2.right;
			moveToNode(direction);

		} else if (Input.GetKeyDown (KeyCode.UpArrow)) {

			direction = Vector2.up;
			moveToNode(direction);

		} else if (Input.GetKeyDown (KeyCode.DownArrow)) {

			direction = Vector2.down;
			moveToNode(direction);
		}
	}

	void Move () {

		transform.localPosition += (Vector3)(direction * speed) * Time.deltaTime;
	}

	void UpdateOrientation () {

		if (direction == Vector2.left) {

			transform.localScale = new Vector3 (-1, 1, 1);
			transform.localRotation = Quaternion.Euler (0, 0, 0);

		} else if (direction == Vector2.right) {

			transform.localScale = new Vector3 (1, 1, 1);
			transform.localRotation = Quaternion.Euler (0, 0, 0);

		} else if (direction == Vector2.up) {

			transform.localScale = new Vector3 (1, 1, 1);
			transform.localRotation = Quaternion.Euler (0, 0, 90);

		} else if (direction == Vector2.down) {

			transform.localScale = new Vector3 (1, 1, 1);
			transform.localRotation = Quaternion.Euler (0, 0, 270);
		}
	}

	Node getNodeAtPositionOfPacman (Vector2 _pacmanPos) {
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

	Node pacmanCanMove (Vector2 _pacmanPos) {
		Node moveToNextNode = null;

		for (int index = 0; index < currentNodeOnWhichPacmanIsStanding.neighbors.Length; index++) {
			if (currentNodeOnWhichPacmanIsStanding.validDirections[index] == _pacmanPos) {
				moveToNextNode = currentNodeOnWhichPacmanIsStanding.neighbors[index];
				break;
			}
		}

		return moveToNextNode;
	}

	void moveToNode (Vector2 _nextPos) {
		Node moveToNextNode = pacmanCanMove (_nextPos);

		if (currentNodeOnWhichPacmanIsStanding != null) {
			this.gameObject.transform.localPosition = moveToNextNode.transform.position;
			currentNodeOnWhichPacmanIsStanding = moveToNextNode;
		}
	}
}

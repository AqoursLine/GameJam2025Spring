using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.PlayerLoop;

public class MoveWallScript : MonoBehaviour {
	private float checkRadius = 0.1f;
	private float checkDistance = 0.98f;

	[SerializeField]
	LayerMask wallLayers;

	private bool moveLR = true;

	[SerializeField]
	int removeCount = 9;

	private int moveCount = 0;

	Vector3 oldPos = Vector3.zero;
	Vector3 newPos = Vector3.zero;

	bool isFall = false;

	enum MoveWallState {
		None = 0,
		MoveWait,		//待機
		MoveFloar,		//床移動
		MoveFall,		//落下
	}

	private MoveWallState state = MoveWallState.None;

	private bool isMoveEnd = false;

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		if (state == MoveWallState.MoveWait && isFall) {
			//自然落下
			Vector3 pos = transform.position;
			pos.y -= 1;
			newPos = pos;

			state = MoveWallState.MoveFall;
			return;
		}

		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			Debug.Log(Move(2));
		}

		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			Debug.Log(Move(6));
		}

	}

	void FixedUpdate() {
		if (state == MoveWallState.None || state == MoveWallState.MoveWait) {
			return;
		}
		moveCount++;

		Vector3 pos = new Vector3();

		switch (state) {
			case MoveWallState.None:
				break;
			case MoveWallState.MoveWait:
				break;
			case MoveWallState.MoveFloar:
				pos = MoveFloor(pos);
				isMoveEnd = removeCount <= moveCount;
				break;
			case MoveWallState.MoveFall:
				pos = MoveFall(pos);
				isMoveEnd = removeCount <= moveCount;
				break;
			default:
				break;
		}

		if (isMoveEnd) {
			transform.position = newPos;

			isFall = checkObjectStatus(4).collider == null;

			isMoveEnd = false;
			state = MoveWallState.MoveWait;
			moveCount = 0;

			oldPos = transform.position;
		}
	}

	public bool Move(int direction) {

		RaycastHit2D hit = checkObjectStatus(direction);

		oldPos = transform.position;

		if (hit.collider != null) {
			return false;
		}

		if (direction == 2) {
			moveLR = true;
			state = MoveWallState.MoveFloar;
			Vector3 pos = transform.position;
			pos.x += 1;
			newPos = pos;
		} else if (direction == 6) {
			moveLR= false;
			state = MoveWallState.MoveFloar;
			Vector3 pos = transform.position;
			pos.x -= 1;
			newPos = pos;
		}

		return true;
	}


	// 接触判定
	RaycastHit2D checkObjectStatus(int direction) {
		RaycastHit2D hit = new RaycastHit2D();
		switch (direction) {
			case 0: //上
				hit = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.up, checkDistance, wallLayers);
				break;
			case 1: //右上
				hit = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.up + Vector2.right, checkDistance, wallLayers);
				break;
			case 2: //右
				hit = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.right, checkDistance, wallLayers);
				break;
			case 3: //右下
				hit = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.down + Vector2.right, checkDistance, wallLayers);
				break;
			case 4: //下
				hit = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.down, checkDistance, wallLayers);
				break;
			case 5: //左下
				hit = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.down + Vector2.left, checkDistance, wallLayers);
				break;
			case 6: //左
				hit = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.left, checkDistance, wallLayers);
				break;
			case 7: //左上
				hit = Physics2D.CircleCast((Vector2)transform.position, checkRadius, Vector2.up + Vector2.left, checkDistance, wallLayers);
				break;
			default:
				break;
		}
		return hit;
	}

	//床を左右移動
	Vector3 MoveFloor(Vector3 pos) {
		if (!moveLR) {
			pos.x = (newPos.x + oldPos.x) / 2 + 0.5f * Mathf.Cos(Mathf.PI / removeCount * moveCount);
		} else if (moveLR) {
			pos.x = (newPos.x + oldPos.x) / 2 - 0.5f * Mathf.Cos(Mathf.PI / removeCount * moveCount);
		}
		pos.y = (newPos.y + oldPos.y) / 2 + 0.5f * Mathf.Sin(Mathf.PI / removeCount * moveCount);

		transform.localScale = new Vector3(1, 1, 1);
		return pos;
	}

	//落下モーション
	Vector3 MoveFall(Vector3 pos) {
		pos.y += ((newPos.y - oldPos.y) / removeCount);

		return pos;
	}
}

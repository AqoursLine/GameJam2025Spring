using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class GoalScript : MonoBehaviour {
	[SerializeField]
	LayerMask goalMask;

	[SerializeField]
	ClearScript clearScript;

	MovePlayer movePlayer = null;

	bool isGoal = false;

	bool isFirst = true;
	// Start is called before the first frame update
	void Start() {
		movePlayer = GetComponent<MovePlayer>();
	}

	// Update is called once per frame
	void Update() {
		if (!isGoal) {
			RaycastHit2D hit = Physics2D.CircleCast((Vector2)transform.position, 0.3f, Vector2.up, 0.1f, goalMask);
			if (hit.collider != null) {
				isGoal = true;
			}
		}

		//プレイヤーのスクリプトがある
		if (isGoal && isFirst) {
			//プレイヤーの移動が終わったら
			if (!movePlayer.GetPlayerState()) {
				clearScript.IsClearEffect();
			}

			isGoal = false;

			isFirst = false;
		}
	}
}

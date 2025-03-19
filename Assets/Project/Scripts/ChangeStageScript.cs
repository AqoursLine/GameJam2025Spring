using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStageScript : MonoBehaviour {
	[SerializeField]
	GameObject player;

	[SerializeField]
	GameObject[] stages;

	int stageNum;

	// Start is called before the first frame update
	void Start() {
		stageNum = 0;
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Return)) {
			Vector3 pos = player.transform.position - stages[stageNum].transform.position;

			stageNum++;
			stageNum %= stages.Length;
			player.transform.position = pos + stages[stageNum].transform.position;

			pos = stages[stageNum].transform.position;
			pos.z = -10;
			Camera.main.transform.position = pos;
		}
	}
}

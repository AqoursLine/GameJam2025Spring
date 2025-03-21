using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectManagerScript : MonoBehaviour {
	//選択肢のアイコン
	[SerializeField]
	GameObject selectIcon;

	//アイコンの移動先ポジション
	[SerializeField]
	Transform[] positions;

	//選択中の番号
	int selectNum;

	// Start is called before the first frame update
	void Start() {
		selectNum = 0;

		//どこにいても最初のポジションに移動
		selectIcon.transform.position = positions[0].position;
	}

	// Update is called once per frame
	void Update() {
		//移動するか
		bool isMoved = false;

		//右に移動
		if (Input.GetKeyDown(KeyCode.D)) {
			selectNum++;
			selectNum %= positions.Length;

			isMoved = true;
		}

		//左に移動
		if (Input.GetKeyDown(KeyCode.A)) {
			selectNum += positions.Length - 1;
			selectNum %= positions.Length;

			isMoved = true;
		}

		//ボタン押されたら
		if (isMoved) {
			//ポジション移動
			selectIcon.transform.position = positions[selectNum].position;
		}

		//決定されたら
		if (Input.GetKeyDown(KeyCode.Return)) {
			//番号毎に移動
			switch (selectNum) {
				default:
					SceneManager.LoadScene("Miyata_ChangeStage");
					break;
			}
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStageScript : MonoBehaviour {

	//プレイヤー
	[SerializeField]
	GameObject player;

	//移動先
	[SerializeField]
	GameObject[] stages;

	//現在のステージ番号
	int stageNum;

	// Start is called before the first frame update
	void Start() {
		stageNum = 0;
	}

	// Update is called once per frame
	void Update()
	{
		// シーン内のPlayerスクリプトを持つオブジェクトを探す
		MovePlayer movePlayer = FindObjectOfType<MovePlayer>();

		// 見つかった場合のみ関数を実行
		if (movePlayer != null)
		{
			if (Input.GetKeyDown(KeyCode.Space) && !movePlayer.GetPlayerState())
			{
				//プレイヤーの相対ポジションを取得
				Vector3 pos = player.transform.position - stages[stageNum].transform.position;

				//ステージ番号を加算
				stageNum++;
				stageNum %= stages.Length;

				//プレイヤーのポジションを変更
				player.transform.position = pos + stages[stageNum].transform.position;

				// 見つかった場合のみ関数を実行
				if (movePlayer != null)
				{
					movePlayer.GetAroudObject();
					movePlayer.CatchWallFlg();
					movePlayer.CheckFall();
				}

				//カメラのポジションを変更
				pos = stages[stageNum].transform.position;
				pos.z = -10;
				Camera.main.transform.position = pos;
			}
		}
	}
}

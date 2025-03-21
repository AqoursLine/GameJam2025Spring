using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

struct PlayerData {
	public Vector2 pos { set; get; }
	public int level { set; get; }
}

struct TurnData {
	public PlayerData player;
	public List<bool> isActives;
}

public class OneTurnSave : MonoBehaviour {
	List<TurnData> data;

	// Start is called before the first frame update
	void Start() {
		IncrementTurn();
	}

	// Update is called once per frame
	void Update() {
	}

	void IncrementTurn() {
		data.Add(new TurnData());
	}

	void DecrementTurn() {
		data.RemoveAt(data.Count - 1);
	}

	int GetTurnNum() {
		return data.Count;
	}

	void SetPlayerData(Vector2 pos, int level) {
		TurnData turn = data.Last();
		turn.player.pos = pos;
		turn.player.level = level;
	}
}


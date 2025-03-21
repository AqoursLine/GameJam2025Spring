using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearScript : MonoBehaviour {
	[SerializeField]
	GameObject[] clearEffects;

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Alpha0)) {
			IsClearEffect();
		}

	}

	public void IsClearEffect() {
		for (int i = 0; i < clearEffects.Length; i++) {
			if (i != 0) {
				clearEffects[i].transform.position += Camera.main.transform.position;
			}
			clearEffects[i].SetActive(true);
		}


	}
}

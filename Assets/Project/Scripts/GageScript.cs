using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GageScript : MonoBehaviour {
	[SerializeField]
	Image image;

	int current;

	// Start is called before the first frame update
	void Start() {
		current = 0;

	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Alpha9)) {
			current++;

			current %= 11;

			ChangeGage(current, 10);
		}
	}

	public void ChangeGage(int currentNum, int maxNum) {
		image.fillAmount = (float)currentNum / maxNum;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsorptionScript : MonoBehaviour
{
	bool isAdsorption;

	float adsorptionTime;

	Rigidbody2D rb;

	// Start is called before the first frame update
	void Start()
    {
		isAdsorption = false;

		adsorptionTime = 0.0f;

		rb = this.GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update()
    {
		if (isAdsorption) {
			adsorptionTime += Time.deltaTime;
		}

		if (adsorptionTime >= 3.0f) {
			rb.gravityScale = 1.0f;

			adsorptionTime = 0.0f;
			isAdsorption = false;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.CompareTag("Field")) {
			Vector2 objectUp = collision.transform.up;
			Vector2 gravityDirection = Physics2D.gravity;

			float dotProduct = Vector2.Dot(objectUp, gravityDirection);

			if (dotProduct >= 0) {
				isAdsorption = true;
				rb.gravityScale = 0.0f;
			}
		}
	}
}

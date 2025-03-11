using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObjectScript : MonoBehaviour
{
	Rigidbody2D rb;

	Vector2 force;
	// Start is called before the first frame update
	void Start()
	{
		rb = this.GetComponent<Rigidbody2D>();

		force = new Vector2(0.0f, 10.0f);
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			rb.AddForce(force);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class TestObjectScript : MonoBehaviour
{
	Rigidbody2D rb;

	float force;

	// Start is called before the first frame update
	void Start()
	{
		rb = this.GetComponent<Rigidbody2D>();

		force = 10.0f;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey(KeyCode.A)) {
			rb.AddForce(Camera.main.transform.right * -5.0f);
		}
		if (Input.GetKey(KeyCode.D)) {
			rb.AddForce(Camera.main.transform.right * 5.0f);
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			Vector2 var = (Camera.main.transform.up * force);
			rb.AddForce(var, ForceMode2D.Impulse);
			Debug.Log(var);
		}
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityChangeScript : MonoBehaviour
{

    float angle;

    // Start is called before the first frame update
    void Start()
    {
        angle = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        bool isKeyDown = false;

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            angle -= 90.0f;

            isKeyDown = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            angle += 90.0f;

            isKeyDown = true;
        }

        if (isKeyDown) {
            float radians = (angle + 90.0f) * Mathf.Deg2Rad;
            Physics2D.gravity = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * -9.8f;
            Camera.main.transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
            isKeyDown = false;

            Debug.Log(angle);
            Debug.Log(Physics2D.gravity);
        }
    }
}

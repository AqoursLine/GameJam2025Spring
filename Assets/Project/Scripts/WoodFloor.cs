using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodFloor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKey(KeyCode.Space))
		{
			this.gameObject.SetActive(false);
		}   
    }

	public void SetBurnCount(int count, int removeCount)
	{
		if(removeCount <= count)
		{
			this.gameObject.SetActive(false);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour

{
	public Rigidbody2D rb;
	public int force;
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
		{
			rb.AddForce(new Vector2(force,0));
		}

    }
}

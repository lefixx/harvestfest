// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class Movementtest2 : MonoBehaviour
// {
// 	public float moveSpeed;
// 	public Rigidbody2D rb;
// 	private Vector2 moveDirection;
//
//     // Update is called once per frame
//     void Update()
//     {
// 		ProccessInputs();
//     }
//
// 	void FixedUpdate()
// 	{
// 		Move();
// 	}
//
// 	void ProccessInputs()
// 	{
// 		float moveX = Input.GetAxisRaw("Horizontal");
// 		float moveY = Input.GetAxisRaw("Vertical");
//
// 		moveDirection = new Vector2(moveX, moveY).normalized;
//
// 	}
//
// 	void Move()
// 	{
// 		// collect info
// 			//current velocity
//
// 			//target velocity
//
// 		// caclulate new velocity
// 			//new velocity = old  velocity *
// 		rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
// 	}
// }



//-----------------------------------------------------------


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movementtest2 : MonoBehaviour
{
	public float moveSpeed;
	public Rigidbody2D rb;
	private Vector2 moveDirection;
	private Vector2 targetSpeed;
	private Vector2 targetDirection;
	// float acceleration = 0.6;
	public float maxSpeed = 10;
	// float speed = 0;

    void Update()
    {
		ProccessInputs3();
    }

	void FixedUpdate()
	{
		Move3();
	}

	void ProccessInputs3()
	{
		float moveX = Input.GetAxisRaw("Horizontal");
		float moveY = Input.GetAxisRaw("Vertical");

		targetDirection = new Vector2(moveX, moveY).normalized;
		targetSpeed = new Vector2(targetDirection.x*maxSpeed, targetDirection.y*maxSpeed);
		if (moveX == 0)
			if(moveY == 0)
			{
				targetSpeed = new Vector2(0,0);
			}
	}

	void Move3()
	{
		rb.velocity =  Vector2.Lerp(rb.velocity, targetSpeed, 0.2f);
	}
}

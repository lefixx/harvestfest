// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class Movementtest2 : MonoBehaviour
// {
// 	public float moveSpeed;
// 	public Rigidbody2D rb;
// 	private Vector2 moveDirection;
// 	float acceleration = 0.6;
// 	float maxSpeed = 30;
// 	float speed = 0;
//
//     void Update()
//     {
// 		ProccessInputs3();
//     }
//
// 	void FixedUpdate()
// 	{
// 		Move3();
// 	}
//
// 	void ProccessInputs3()
// 	{
// 		float moveX = Input.GetAxisRaw("Horizontal");
// 		float moveY = Input.GetAxisRaw("Vertical");
//
// 		targetDirection = new Vector2(moveX, moveY).normalized;
// 		targetSpeed = new Vector2(targetDirection.x*maxSpeed, targetDirection.y*maxSpeed);
// 	}
//
// 	void Move3()
// 	{
// 		rb.velocity =  Vector2.Lerp(transform.position, targetSpeed, 0.5);
// 	}
// }

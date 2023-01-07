using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotateScythe : MonoBehaviour
{
    // public Material fastWheelMaterial;
    // public Material slowWheelMaterial;
    public Rigidbody2D ScytheRigidBody;
	private int TorgueDirection;
	private float rot;
    // public MeshRenderer rend;

    void Start()
    {
        ScytheRigidBody = GetComponent<Rigidbody2D>();
        // rend = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        // if (ScytheRigidBody.rotation > 0)
		// {
		// 	TorgueDirection = -1;
		// }
		// else
		// {
		// 	TorgueDirection = 1;
		// }

		rot = ScytheRigidBody.rotation*0.1f;
		ScytheRigidBody.AddTorque(-rot);

        //     rend.sharedMaterial = slowWheelMaterial;
        // else
        //     rend.sharedMaterial = fastWheelMaterial;
    }
}

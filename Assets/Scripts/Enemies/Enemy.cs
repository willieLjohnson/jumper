using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Enemy : MonoBehaviour
{
  Controller2D controller;
  AttackController attackController;

  public Transform target;

  float gravity = 12f;

  float moveSpeed = 13f;

  float accelerationTimeGrounded = 0.1f;
  float accelerationTimeAirborne = 0.2f;

  Vector3 velocity;


  float velocityXSmoothing;

  void Start()
  {
    controller = GetComponent<Controller2D>();
    attackController = GetComponent<AttackController>();

    if (target == null)
    {
      target = GameObject.FindGameObjectWithTag("Player").transform;
    }
  }

  // Update is called once per frame
  void Update()
  {
    CalculateVelocity();
    controller.Move(velocity * Time.deltaTime, false);

    if (controller.collisions.below)
    {
      velocity.y = 0;
    }
  }

  void CalculateVelocity()
  {
    if (target)
    {
      Vector2 distance = (target.position - transform.position);

      // Chase target
      if (Mathf.Abs(distance.x) < 10 && Mathf.Abs(distance.y) < 10)
      {
        float targetVelocityX = Mathf.Sign(distance.x) * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

        if (distance.y >= 0.01 && controller.collisions.below)
        {
          velocity.y = 15f;
          Debug.Log("Jump");
        }


        // Check if the position of the cube and sphere are approximately equal.
        if (distance.x < attackController.range && distance.y < attackController.range)
        {
          // Swap the position of the cylinder.
          attackController.Attack(ref velocity, distance, controller.collisions.below, target.position);
        }
      }
    }

    velocity += Vector3.down * gravity * Time.deltaTime;
  }
}

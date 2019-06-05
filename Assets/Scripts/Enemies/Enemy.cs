using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Enemy : MonoBehaviour
{
  Controller2D controller;
  AttackController attackController;

  public Transform target;

  float gravity = -12;

  public float moveSpeed = 20;

  float accelerationTimeGrounded = 0.1f;
  float accelerationTimeAirborne = 0.2f;

  Vector2 velocity;


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
    controller.Move(velocity, false);

    // Check if the position of the cube and sphere are approximately equal.
    if (Vector3.Distance(transform.position, target.position) < 0.001f)
    {
      // Swap the position of the cylinder.
      target.position *= -1.0f;
    }
  }

  void CalculateVelocity()
  {
    Vector2 direction = (target.position - transform.position).normalized;
    Vector2 moveAmount = direction * moveSpeed * Time.deltaTime; // calculate distance to move

    float targetVelocityX = direction.x * moveSpeed;
    velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

    if (direction.y > 0 && controller.collisions.below)
    {
      velocity.y = 1.5f;
    }

    velocity.y += gravity * Time.deltaTime;

  }
}

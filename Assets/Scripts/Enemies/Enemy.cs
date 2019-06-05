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

  public float moveSpeed = 50;

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

    // Check if the position of the cube and sphere are approximately equal.
    if (Vector3.Distance(transform.position, target.position) < 0.01f)
    {
      // Swap the position of the cylinder.
      target.gameObject.GetComponent<Destructable>().Damage(50);
    }

  }

  void CalculateVelocity()
  {
    Vector2 distance = (target.position - transform.position).normalized;

    float targetVelocityX = Mathf.Sign(distance.x) * moveSpeed;
    velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

    if (distance.y >= 0.01 && controller.collisions.below)
    {
      velocity.y = 15f;
      Debug.Log("Jump");
    }

    velocity += Vector3.down * gravity * Time.deltaTime;
  }
}

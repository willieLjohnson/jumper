using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Enemy : MonoBehaviour
{
  public LayerMask targetMask;
  public Transform target;
  public float moveSpeed = 13f;

  Controller2D controller;
  AttackController attackController;

  float gravity = 12f;

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
      target = Player.Instance.transform;
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
      RaycastHit2D lineOfSight = Physics2D.Raycast(transform.position, distance.normalized, distance.magnitude, targetMask);
      Collider2D colliderInView = null;

      Debug.DrawRay(transform.position, distance, Color.blue);

      if (lineOfSight)
      {
        colliderInView = lineOfSight.collider;
      }

      if (colliderInView && colliderInView.tag == target.gameObject.tag)
      {
        Debug.DrawRay(transform.position, distance, Color.green);
        // Chase target
        if (Mathf.Abs(distance.x) < 15 && Mathf.Abs(distance.y) < 15)
        {
          float targetVelocityX = Mathf.Sign(distance.x) * moveSpeed;
          velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

          if (distance.y >= 0.01 && controller.collisions.below)
          {
            velocity.y = 15f;
          }

          // Check if the position of the cube and sphere are approximately equal.
          if (distance.x < attackController.range && distance.y < attackController.range)
          {
            // Swap the position of the cylinder.
            attackController.Attack(ref velocity, distance, controller.collisions.below, target.position);
          }
        }
      }

    }

    velocity += Vector3.down * gravity * Time.deltaTime;
  }
}

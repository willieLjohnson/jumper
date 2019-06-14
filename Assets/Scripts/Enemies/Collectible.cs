using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Collectible : MonoBehaviour
{
  Controller2D controller;

  public Transform target;

  float gravity = 12f;

  float moveSpeed = 13f;

  // Range at which the player will collect.
  float collectionRange = 5f;

  float accelerationTimeGrounded = 0.1f;
  float accelerationTimeAirborne = 0.2f;

  Vector3 velocity;

  float velocityXSmoothing;
  float velocityYSmoothing;

  void Start()
  {
    controller = GetComponent<Controller2D>();

    if (target == null)
    {
      target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    velocity = Random.insideUnitSphere * moveSpeed;
  }

  // Update is called once per frame
  void Update()
  {
    CalculateVelocity();
    controller.Move(velocity * Time.deltaTime, false);
  }

  void CalculateVelocity()
  {
    if (target)
    {
      Vector2 distance = (target.position - transform.position);

      // Chase target
      if (Mathf.Abs(distance.x) < collectionRange && Mathf.Abs(distance.y) < collectionRange)
      {
        float targetVelocityX = Mathf.Sign(distance.x) * moveSpeed;
        float targetVelocityY = Mathf.Sign(distance.y) * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref velocityYSmoothing, (controller.collisions.left || controller.collisions.right) ? accelerationTimeGrounded : accelerationTimeAirborne);
      }
    }

    velocity += Vector3.down * gravity * Time.deltaTime;
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
  public float jumpHeight = 4;
  public float timeToJumpApex = 0.4f;
  float accelerationTimeAirborne = 0.2f;
  float accelerationTimeGrounded = 0.1f;
  float movesSpeed = 6;

  public float wallSlideSpeedMax = 3;

  float gravity;
  float jumpVelocity;
  Vector3 velocity;
  float velocityXSmoothing;

  Controller2D controller;
  // Start is called before the first frame update
  void Start()
  {
    controller = GetComponent<Controller2D>();

    gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
    jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    print("Gravity" + gravity + "jumpvelocity" + jumpVelocity);
  }

  void Update()
  {
    bool wallSliding = false;
    if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
    {
      wallSliding = true;

      if (velocity.y < -wallSlideSpeedMax)
      {
        velocity.y = -wallSlideSpeedMax;
      }
    }

    if (controller.collisions.above || controller.collisions.below)
    {
      velocity.y = 0;
    }

    Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

    if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
    {
      velocity.y = jumpVelocity;
    }

    float targetVelocityX = input.x * movesSpeed;
    velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
    velocity.y += gravity * Time.deltaTime;
    controller.Move(velocity * Time.deltaTime);
  }
}

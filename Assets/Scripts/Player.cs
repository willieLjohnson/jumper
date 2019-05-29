using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
  public float jumpHeight = 4;
  public float timeToJumpApex = 0.4f;

  float movesSpeed = 6;

  float gravity;
  float jumpVelocity;
  Vector3 velocity;

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
    if (controller.collisions.above || controller.collisions.below)
    {
      velocity.y = 0;
    }

    Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

    if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
    {
      velocity.y = jumpVelocity;
    }

    velocity.x = input.x * movesSpeed;
    velocity.y += gravity * Time.deltaTime;
    controller.Move(velocity * Time.deltaTime);
  }
}

﻿using UnityEngine;
using System.Collections;

public class Controller2D : RaycastController
{
  public float maxSlopeAngle = 80;

  public CollisionInfo collisions;
  Destructable destructable = null;
  bool isDestructable = false;
  [HideInInspector]
  public Vector2 playerInput;

  public override void Start()
  {
    base.Start();
    collisions.faceDir = 1;
    destructable = GetComponent<Destructable>();

    if (destructable)
    {
      isDestructable = true;
    }
  }

  public Vector2 Move(Vector2 moveAmount, bool standingOnPlatform)
  {
    return Move(moveAmount, Vector2.zero, standingOnPlatform);
  }

  public Vector2 Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
  {
    UpdateRaycastOrigins();

    collisions.Reset();
    collisions.moveAmountOld = moveAmount;
    playerInput = input;

    if (moveAmount.y < 0)
    {
      DescendSlope(ref moveAmount);
    }

    if (moveAmount.x != 0)
    {
      collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
    }

    HorizontalCollisions(ref moveAmount);
    if (moveAmount.y != 0)
    {
      VerticalCollisions(ref moveAmount);
    }

    transform.Translate(moveAmount);

    if (standingOnPlatform)
    {
      collisions.below = true;
    }

    return moveAmount;
  }

  void HorizontalCollisions(ref Vector2 moveAmount)
  {
    float originalMoveAmountX = moveAmount.x;
    Collider2D otherCollider = null;

    float directionX = collisions.faceDir;
    float rayLength = (Mathf.Abs(moveAmount.x) + skinWidth) * 2;

    if (Mathf.Abs(moveAmount.x) < skinWidth)
    {
      rayLength = 2 * skinWidth;
    }

    for (int i = 0; i < horizontalRayCount; i++)
    {
      Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
      rayOrigin += Vector2.up * (horizontalRaySpacing * i);
      RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

      Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

      if (hit)
      {

        if (hit.distance == 0)
        {
          continue;
        }

        otherCollider = hit.collider;

        float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

        if (otherCollider.tag != "Collectible")
        {
          if (i == 0 && slopeAngle <= maxSlopeAngle)
          {
            if (collisions.descendingSlope)
            {
              collisions.descendingSlope = false;
              moveAmount = collisions.moveAmountOld;
            }
            float distanceToSlopeStart = 0;
            if (slopeAngle != collisions.slopeAngleOld)
            {
              distanceToSlopeStart = hit.distance - skinWidth;
              moveAmount.x -= distanceToSlopeStart * directionX;
            }
            ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
            moveAmount.x += distanceToSlopeStart * directionX;
          }

          if ((!collisions.climbingSlope || slopeAngle > maxSlopeAngle))
          {
            moveAmount.x = (hit.distance - skinWidth) * directionX;
            rayLength = hit.distance;

            if (collisions.climbingSlope)
            {
              moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
            }

            collisions.left = directionX == -1;
            collisions.right = directionX == 1;
          }
        }
      }
    }

    if (otherCollider != null && otherCollider.gameObject != this.gameObject)
    {
      if (otherCollider.tag == "Pushable")
      {
        Vector2 pushAmount = otherCollider.gameObject.GetComponent<PushableObject>().Push(new Vector2(originalMoveAmountX, 0));
        //print (moveAmount.y);
        moveAmount = new Vector2(pushAmount.x, moveAmount.y + pushAmount.y);
        collisions.left = false;
        collisions.right = false;
      }

      if (isDestructable)
      {
        if (otherCollider.tag == "Fall Line")
        {
          destructable.isDead = true;
        }

        if (otherCollider.tag == "Hazard" && isDestructable)
        {
          destructable.Damage(9000);
        }
      }

      if (this.collider.tag == "Collectible" && otherCollider.tag == "Player")
      {
        Destructable otherDestructable = otherCollider.gameObject.GetComponent<Destructable>();
        otherDestructable.value += 1;
        gameObject.GetComponent<Collectible>().Collect();

      }
    }
  }

  void VerticalCollisions(ref Vector2 moveAmount)
  {
    float directionY = Mathf.Sign(moveAmount.y);
    float rayLength = (Mathf.Abs(moveAmount.y) + skinWidth) * 2;
    Collider2D otherCollider = null;

    for (int i = 0; i < verticalRayCount; i++)
    {

      Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
      rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
      RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

      Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

      if (hit)
      {
        otherCollider = hit.collider;

        if (otherCollider.tag == "Through")
        {
          if (directionY == 1 || hit.distance == 0)
          {
            continue;
          }
          if (collisions.fallingThroughPlatform)
          {
            continue;
          }
          if (playerInput.y == -1)
          {
            collisions.fallingThroughPlatform = true;
            Invoke("ResetFallingThroughPlatform", .5f);
            continue;
          }
        }

        if (otherCollider.tag != "Collectible")
        {
          moveAmount.y = (hit.distance - skinWidth) * directionY;


          rayLength = hit.distance;

          if (collisions.climbingSlope)
          {
            moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
          }

          collisions.below = directionY == -1;
          collisions.above = directionY == 1;
        }
      }

      if (otherCollider != null && otherCollider.gameObject != this.gameObject)
      {

        if (isDestructable)
        {
          if (otherCollider.tag == "Fall Line")
          {
            destructable.isDead = true;
          }

          if (otherCollider.tag == "Hazard")
          {
            destructable.Damage(9000);
          }
        }

        if (this.collider.tag == "Collectible" && otherCollider.tag == "Player")
        {
          Destructable otherDestructable = otherCollider.gameObject.GetComponent<Destructable>();
          otherDestructable.value += 1;
          gameObject.GetComponent<Collectible>().Collect();
        }
      }
    }

    if (collisions.climbingSlope)
    {
      float directionX = Mathf.Sign(moveAmount.x);
      rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
      Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
      RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

      if (hit)
      {
        float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        if (slopeAngle != collisions.slopeAngle)
        {
          moveAmount.x = (hit.distance - skinWidth) * directionX;
          collisions.slopeAngle = slopeAngle;
          collisions.slopeNormal = hit.normal;
        }
      }
    }
  }

  void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal)
  {
    float moveDistance = Mathf.Abs(moveAmount.x);
    float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

    if (moveAmount.y <= climbmoveAmountY)
    {
      moveAmount.y = climbmoveAmountY;
      moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
      collisions.below = true;
      collisions.climbingSlope = true;
      collisions.slopeAngle = slopeAngle;
      collisions.slopeNormal = slopeNormal;
    }
  }

  void DescendSlope(ref Vector2 moveAmount)
  {

    RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
    RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
    if (maxSlopeHitLeft ^ maxSlopeHitRight)
    {
      SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
      SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
    }

    if (!collisions.slidingDownMaxSlope)
    {
      float directionX = Mathf.Sign(moveAmount.x);
      Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
      RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

      if (hit)
      {
        float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
        {
          if (Mathf.Sign(hit.normal.x) == directionX)
          {
            if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
            {
              float moveDistance = Mathf.Abs(moveAmount.x);
              float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
              moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
              moveAmount.y -= descendmoveAmountY;

              collisions.slopeAngle = slopeAngle;
              collisions.descendingSlope = true;
              collisions.below = true;
              collisions.slopeNormal = hit.normal;
            }
          }
        }
      }
    }
  }

  void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
  {

    if (hit)
    {
      float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
      if (slopeAngle > maxSlopeAngle)
      {
        moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

        collisions.slopeAngle = slopeAngle;
        collisions.slidingDownMaxSlope = true;
        collisions.slopeNormal = hit.normal;
      }
    }

  }

  void ResetFallingThroughPlatform()
  {
    collisions.fallingThroughPlatform = false;
  }

  public struct CollisionInfo
  {
    public bool above, below;
    public bool left, right;

    public bool climbingSlope;
    public bool descendingSlope;
    public bool slidingDownMaxSlope;

    public float slopeAngle, slopeAngleOld;
    public Vector2 slopeNormal;
    public Vector2 moveAmountOld;
    public int faceDir;
    public bool fallingThroughPlatform;

    public void Reset()
    {
      above = below = false;
      left = right = false;
      climbingSlope = false;
      descendingSlope = false;
      slidingDownMaxSlope = false;
      slopeNormal = Vector2.zero;

      slopeAngleOld = slopeAngle;
      slopeAngle = 0;
    }
  }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : RaycastController
{
  public int damage = 110;
  public Vector2 pushForce = new Vector2(1f, 0.5f);
  public float range = 2;

  public float attackTime = .05f;

  AttackInfo attack;
  float timeToFinishAttacking;

  Vector2 moveAmount;

  public override void Start()
  {
    base.Start();
    range *= collider.bounds.size.x;
  }

  private void Update()
  {
    if (attack.isAttacking)
    {
      if (timeToFinishAttacking > 0)
      {
        timeToFinishAttacking -= Time.deltaTime;
        if (attack.horizontal)
        {
          HorizontalAttack();
        }

        if (attack.vertical)
        {
          VerticalAttack();
        }
      }
      else
      {
        timeToFinishAttacking = attackTime;
        attack.Reset();
      }
    }
  }

  public void Attack(ref Vector3 moveAmount, Vector2 input)
  {
    // attack.Reset();
    print(input);
    if (input.x != 0)
    {
      moveAmount = new Vector2(moveAmount.x + (pushForce.x * input.x), moveAmount.y);
      attack.horizontal = true;
      attack.moveAmount = moveAmount;
    }

    if (input.y != 0)
    {
      moveAmount = new Vector2(moveAmount.x, moveAmount.y + (pushForce.y * input.y));
      attack.vertical = true;
      attack.moveAmount = moveAmount;
    }

    attack.isAttacking = true;
    attack.input = input;

    timeToFinishAttacking = attackTime;
  }

  public void HorizontalAttack()
  {
    UpdateRaycastOrigins();
    Collider2D otherCollider = null;

    float directionX = Mathf.Sign(attack.input.x);

    for (int i = 0; i < horizontalRayCount; i++)
    {
      Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
      rayOrigin += Vector2.up * (horizontalRaySpacing * i);
      RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, range, collisionMask);

      Debug.DrawRay(rayOrigin, Vector2.right * directionX * range, Color.yellow);

      if (hit)
      {
        if (hit.distance == 0)
        {
          continue;
        }

        otherCollider = hit.collider;
      }

      if (otherCollider != null && otherCollider.gameObject != this.gameObject && i == 0) // && otherCollider.tag == "Pushable"
      {
        Destructable destructable = otherCollider.gameObject.GetComponent<Destructable>();
        if (destructable)
        {
          destructable.Damage(damage);
        }


        if (otherCollider.tag == "Pushable")
        {
          PushableObject pushable = otherCollider.gameObject.GetComponent<PushableObject>();
          Vector2 pushAmount = pushable.Launch(new Vector2(pushForce.x * directionX, pushForce.y));
          // moveAmount = new Vector2(pushAmount.x + moveAmount.x, moveAmount.y);
        }

        //print (moveAmount.y);
      }
    }
  }

  public void VerticalAttack()
  {
    UpdateRaycastOrigins();
    // float originalMoveAmountX = moveAmount.x;
    Collider2D otherCollider = null;

    float directionY = Mathf.Sign(attack.input.y);

    for (int i = 0; i < verticalRayCount; i++)
    {
      Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
      rayOrigin += Vector2.right * (verticalRaySpacing * i);
      RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, range, collisionMask);

      Debug.DrawRay(rayOrigin, Vector2.up * directionY * range, Color.yellow);

      if (hit)
      {
        if (hit.distance == 0)
        {
          continue;
        }

        otherCollider = hit.collider;
      }

      if (otherCollider != null && otherCollider.gameObject != this.gameObject && i == 0) // && otherCollider.tag == "Pushable"
      {
        Destructable destructable = otherCollider.gameObject.GetComponent<Destructable>();
        if (destructable)
        {
          destructable.Damage(damage);
        }


        if (otherCollider.tag == "Pushable")
        {
          PushableObject pushable = otherCollider.gameObject.GetComponent<PushableObject>();
          pushable.Launch(new Vector2(pushForce.x, pushForce.y * directionY));
          // moveAmount = new Vector2(moveAmount.x, moveAmount.y + pushForce.y);
        }

        //print (moveAmount.y);
      }
    }
  }

  public struct AttackInfo
  {
    public bool horizontal;
    public bool vertical;
    public bool isAttacking;
    public Vector2 moveAmount;
    public Vector2 input;

    public void Reset()
    {
      horizontal = false;
      vertical = false;
      isAttacking = false;
      input = Vector2.zero;
      moveAmount = Vector2.zero;
    }
  }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : RaycastController
{
  public int damage = 35;
  public float pushForce = 0.5f;

  public float range = 2;

  public override void Start()
  {
    base.Start();
    range *= collider.bounds.size.x;
  }

  public void HorizontalAttack(Vector2 moveAmount)
  {
    UpdateRaycastOrigins();
    // float originalMoveAmountX = moveAmount.x;
    Collider2D otherCollider = null;

    float directionX = Mathf.Sign(moveAmount.x);

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
          Vector2 pushAmount = otherCollider.gameObject.GetComponent<PushableObject>().Push(new Vector2(pushForce * directionX, pushForce / 2));
          destructable.Damage(damage);
        }

        //print (moveAmount.y);
        // moveAmount = new Vector2(pushAmount.x * 10, moveAmount.y + pushAmount.y);
      }
    }
  }
}
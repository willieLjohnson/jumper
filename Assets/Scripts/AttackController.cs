using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : RaycastController
{
  public void HorizontalAttack(Vector2 moveAmount)
  {
    UpdateRaycastOrigins();
    float originalMoveAmountX = moveAmount.x;
    Collider2D otherCollider = null;

    float directionX = Mathf.Sign(moveAmount.x);
    float rayLength = (Mathf.Abs(moveAmount.x) + skinWidth) * 4;

    for (int i = 0; i < horizontalRayCount; i++)
    {
      Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
      rayOrigin += Vector2.up * (horizontalRaySpacing * i);
      RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

      Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.yellow);

      if (hit)
      {

        if (hit.distance == 0)
        {
          continue;
        }

        otherCollider = hit.collider;
      }

      if (otherCollider != null && otherCollider.gameObject != this.gameObject && otherCollider.tag == "Pushable")
      {
        Vector2 pushAmount = otherCollider.gameObject.GetComponent<PushableObject>().Push(new Vector2(originalMoveAmountX, 0));
        //print (moveAmount.y);
        // moveAmount = new Vector2(pushAmount.x * 10, moveAmount.y + pushAmount.y);
      }
    }
  }
}
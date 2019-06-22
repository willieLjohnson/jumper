using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{

  Player player;
  Camera cam;

  void Start()
  {
    player = GetComponent<Player>();
    cam = Camera.main;
  }

  void Update()
  {
    Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    player.SetDirectionalInput(directionalInput);

    if (Input.GetKeyDown(KeyCode.Space))
    {
      player.OnJumpInputDown();
    }
    if (Input.GetKeyUp(KeyCode.Space))
    {
      player.OnJumpInputUp();
    }
    if (Input.GetMouseButtonDown(0))
    {
      player.OnAttackButtonDown(cam.ScreenToWorldPoint(Input.mousePosition));
    }

    if (Input.GetKeyDown(KeyCode.Escape))
    {
      LevelManager.Instance.HandlePause();
    }
  }
}

using ExplosionJumping.PlayerControl.Movement.AirControl;
using UnityEngine;

[AddComponentMenu("Player Control/Air Control/No Air Control")]
public class NoAirControl : PlayerAirController {
    public override void AirStafe(Vector2 input) {
        // lol do nothing
    }
}

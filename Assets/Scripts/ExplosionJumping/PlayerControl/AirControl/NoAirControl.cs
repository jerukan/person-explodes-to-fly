using UnityEngine;

namespace ExplosionJumping.PlayerControl.AirControl {
    [AddComponentMenu("Player Control/Air Control/No Air Control")]
    public class NoAirControl : PlayerAirController {
        public override void AirStafe(Vector2 input) {
            // lol do nothing
        }
    }
}
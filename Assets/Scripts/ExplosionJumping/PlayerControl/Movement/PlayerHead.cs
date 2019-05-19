using UnityEngine;
using System.Collections;

namespace ExplosionJumping.PlayerControl.Movement {
    public class PlayerHead : MonoBehaviour {

        public Camera cam;

        public void ResetHeadRotation() {
            transform.localEulerAngles = Vector3.zero;
        }
    }
}
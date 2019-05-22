using UnityEngine;

namespace ExplosionJumping.PlayerControl.CameraControl {
    /// <summary>
    /// Represents the gameobject, usually child, of the player that is the center of rotation for the camera.
    /// </summary>
    [AddComponentMenu("Player Control/Camera Control/Player Head")]
    public class PlayerHead : MonoBehaviour {

        public Camera cam;

        public void ResetHeadRotation() {
            transform.localEulerAngles = Vector3.zero;
        }
    }
}
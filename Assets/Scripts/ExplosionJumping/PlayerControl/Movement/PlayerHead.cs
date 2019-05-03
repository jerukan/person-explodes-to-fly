using UnityEngine;
using System.Collections;

namespace ExplosionJumping.PlayerControl.Movement {
    public class PlayerHead : MonoBehaviour {

        public Camera cam;
        public CameraLook cameraLook = new CameraLook();

        private void Awake() {
            cameraLook.Init(transform.parent, transform);
        }

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        public void ResetHeadRotation() {
            transform.localEulerAngles = Vector3.zero;
            cameraLook.Init(transform.parent, transform);
        }
    }
}
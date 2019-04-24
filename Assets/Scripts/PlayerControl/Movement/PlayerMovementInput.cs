using UnityEngine;
using System.Collections;

namespace ExplosionJumping.PlayerControl.Movement {
    [RequireComponent(typeof(RigidbodyFPController))]
    public class PlayerMovementInput : MonoBehaviour {

        public KeyCode crouchKey = KeyCode.LeftControl;
        public MouseLook mouseLook = new MouseLook();

        private RigidbodyFPController charController;
        private Rigidbody rigidBody;

        private Vector2 currentInput;

        public Vector2 CurrentInput {
            get { return currentInput; }
        }

        private void Awake() {
            charController = GetComponent<RigidbodyFPController>();
            rigidBody = GetComponent<Rigidbody>();
        }

        private void Start() {
            mouseLook.cameraLook = charController.head.cameraLook;
            mouseLook.Init(transform, charController.head.transform);
        }

        void Update() {
            RotateView();
            GetInput();
            if (charController.autoBunnyHop) {
                charController.Jump = Input.GetButton("Jump");
            }
            else {
                charController.Jump = Input.GetButtonDown("Jump");
            }
            charController.Crouch = Input.GetKey(crouchKey);
            charController.currentTargetDirection = currentInput;
        }

        private void GetInput() {
            // raw axis makes keyboard actually work properly
            currentInput = new Vector2 {
                x = Input.GetAxisRaw("Horizontal"),
                y = Input.GetAxisRaw("Vertical")
            };
        }

        private void RotateView() {
            // avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;

            mouseLook.LookRotation(transform, charController.head.transform, true);

            if (charController.Grounded) {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
                rigidBody.velocity = velRotation * rigidBody.velocity;
            }
        }
    }
}
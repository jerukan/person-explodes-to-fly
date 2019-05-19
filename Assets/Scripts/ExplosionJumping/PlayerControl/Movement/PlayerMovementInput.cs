using UnityEngine;
using System.Collections;

namespace ExplosionJumping.PlayerControl.Movement {
    [RequireComponent(typeof(RigidbodyFPController))]
    public class PlayerMovementInput : MonoBehaviour {

        public KeyCode crouchKey = KeyCode.LeftControl;

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

        void Update() {
            //RotateView();
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
    }
}
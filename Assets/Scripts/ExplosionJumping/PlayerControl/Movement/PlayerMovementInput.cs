using UnityEngine;

namespace ExplosionJumping.PlayerControl.Movement {
    /// <summary>
    /// Allows keyboard control of the player if this component is added.
    /// </summary>
    [RequireComponent(typeof(RigidbodyFPController))]
    [AddComponentMenu("Player Control/Input/Player Movement Input")]
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
            GetMovementInput();
            if (charController.autoBunnyHop) {
                charController.ShouldJump = Input.GetButton("Jump");
            }
            else {
                charController.ShouldJump = Input.GetButtonDown("Jump");
            }
            charController.ShouldCrouch = Input.GetKey(crouchKey);
            charController.currentTargetMovementDirection = currentInput;
        }

        private void GetMovementInput() {
            // raw axis makes keyboard actually work properly
            currentInput = new Vector2 {
                x = Input.GetAxisRaw("Horizontal"),
                y = Input.GetAxisRaw("Vertical")
            };
        }
    }
}
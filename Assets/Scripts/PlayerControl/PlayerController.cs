using ExplosionJumping.Weapon;
using ExplosionJumping.Weapon.Projectile;
using UnityEngine;

namespace ExplosionJumping.PlayerControl {
    [RequireComponent(typeof(RigidbodyFPControllerCustom))]
    public class PlayerController : MonoBehaviour {

        private RigidbodyFPControllerCustom charController;

        private void Awake() {
            charController = GetComponent<RigidbodyFPControllerCustom>();
        }

        private void Update() {
            Transform camTransform = Camera.main.transform;
            if(Input.GetKeyDown(KeyCode.LeftShift)) {
                // todo make it not modified by vertical tilt
                charController.GetComponent<Rigidbody>().AddForce(camTransform.forward * 40, ForceMode.VelocityChange);
            }
        }
    }
}
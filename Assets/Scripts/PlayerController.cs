using UnityEngine;

namespace ExplosionJumping {
    [RequireComponent(typeof(RigidbodyFPControllerCustom))]
    public class PlayerController : MonoBehaviour {

        private RigidbodyFPControllerCustom charController;

        // Use this for initialization
        private void Start() {
            charController = GetComponent<RigidbodyFPControllerCustom>();
        }

        // Update is called once per frame
        private void Update() {
            Transform camTransform = Camera.main.transform;
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                CreateRocket(camTransform);
            }
            if(Input.GetKeyDown(KeyCode.LeftShift)) {
                charController.GetComponent<Rigidbody>().AddForce(camTransform.forward * 60, ForceMode.VelocityChange);
            }
        }

        private void CreateRocket(Transform spawnTransform) {
            GameObject rocket = Instantiate(Resources.Load("Prefabs/BasicRocket"), spawnTransform.position, spawnTransform.rotation) as GameObject;
            rocket.GetComponent<ExplosiveProjectileController>().projectileOwner = this;
            rocket.GetComponent<Rigidbody>().velocity = spawnTransform.forward * rocket.GetComponent<ExplosiveProjectileController>().speed;
        }
    }
}
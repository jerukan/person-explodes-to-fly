using UnityEngine;

namespace ExplosionJumping {
    [RequireComponent(typeof(RigidbodyFPControllerCustom))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour {

        public RigidbodyFPControllerCustom charController;

        private Transform playerTransform;

        // Use this for initialization
        void Start() {
            charController = GetComponent<RigidbodyFPControllerCustom>();
            playerTransform = GetComponent<Transform>();
        }

        // Update is called once per frame
        void Update() {
            Transform camTransform = charController.cam.transform;
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                GameObject rocket = Instantiate(Resources.Load("Prefabs/BasicRocket"), camTransform.position, camTransform.rotation) as GameObject;
                rocket.GetComponent<Rigidbody>().velocity = camTransform.forward * 10;
            }
            if(Input.GetKeyDown(KeyCode.LeftShift)) {
                charController.GetComponent<Rigidbody>().AddForce(camTransform.forward * 60, ForceMode.VelocityChange);
            }
        }
    }
}
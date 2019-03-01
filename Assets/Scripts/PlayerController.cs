using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace ExplosionJumping {
    [RequireComponent(typeof(RigidbodyFirstPersonController))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour {

        public RigidbodyFirstPersonController charController;

        // Use this for initialization
        void Start() {
            charController = GetComponent<RigidbodyFirstPersonController>();
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                Debug.Log("ye");
                Transform camTransform = charController.cam.transform;
                GameObject rocket = Instantiate(Resources.Load("Prefabs/BasicRocket"), camTransform.position, camTransform.rotation) as GameObject;
                rocket.GetComponent<Rigidbody>().velocity = camTransform.forward * 10;
            }
        }
    }
}
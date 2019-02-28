using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof (RigidbodyFirstPersonController))]
[RequireComponent(typeof (Rigidbody))]
public class PlayerController : MonoBehaviour {

    public RigidbodyFirstPersonController charController;

    // Use this for initialization
    void Start() {
        charController = GetComponent<RigidbodyFirstPersonController>();
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.Mouse0)) {
            Vector3 vector = charController.cam.transform.forward;
        }
    }
}

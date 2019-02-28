using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof (Rigidbody))]
public class ExplosiveProjectile : MonoBehaviour {

    public float explosionForce;
    public float explosionRadius;
    public bool gravity;
    public float speed;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    void OnCollisionEnter(Collision collision) {
    }
}

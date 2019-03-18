using System;
using ExplosionJumping.Util;
using UnityEngine;

namespace ExplosionJumping.PlayerControl.AirControl {
    public abstract class AirStrafeController : MonoBehaviour {

        protected Rigidbody rigidBody;
        protected Camera cam;

        private void Awake() {
            rigidBody = GetComponent<Rigidbody>();
        }

        // Use this for initialization
        private void Start() {
            cam = Camera.main;
        }

        public virtual void AirStafe(Vector2 input) { }
    }
}
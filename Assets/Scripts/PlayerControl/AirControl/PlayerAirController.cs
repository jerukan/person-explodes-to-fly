﻿using System;
using ExplosionJumping.Util;
using UnityEngine;

namespace ExplosionJumping.PlayerControl.AirControl {
    /// <summary>
    /// Controls how the player moves in the air.
    /// </summary>
    [RequireComponent(typeof(RigidbodyFPControllerCustom))]
    public abstract class PlayerAirController : MonoBehaviour {

        protected Rigidbody rigidBody;
        protected Camera cam;

        private void Awake() {
            rigidBody = GetComponent<Rigidbody>();
        }

        // Use this for initialization
        private void Start() {
            cam = Camera.main;
        }

        /// <summary>
        /// Takes player input and modifies the movement of the rigidbody accordingly.
        /// Only called while the player is considered in the air.
        /// </summary>
        /// <param name="input">Player input.</param>
        public virtual void AirStafe(Vector2 input) { }
    }
}
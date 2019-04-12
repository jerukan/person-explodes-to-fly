﻿using UnityEngine;
using System.Collections;
using ExplosionJumping.Util;
using ExplosionJumping.PlayerControl.Movement;

namespace ExplosionJumping.PlayerControl {
    [RequireComponent(typeof(PlayerController))]
    public class PlayerRespawner : MonoBehaviour {

        public Transform respawnPosition;
        public float respawnTime;
        private float timeOfDeath;
        private bool prevState;
        private PlayerController playerController;
        private Camera playerCamera;
        private MouseLook coolMouseLook = new MouseLook();

        void Awake() {
            playerController = GetComponent<PlayerController>();
            playerCamera = GetComponent<RigidbodyFPControllerCustom>().cam;
            coolMouseLook.Init(transform, playerCamera.transform);
            coolMouseLook.clampVerticalRotation = false;
        }

        // Update is called once per frame
        void Update() {
            if(!prevState && playerController.Dead) {
                timeOfDeath = Time.time;
            }
            if(prevState) {
                if (Time.time - timeOfDeath >= respawnTime) {
                    playerController.SetAlive(true);
                    playerController.transform.position = respawnPosition.position;
                }
                else {
                    if (!(Mathf.Abs(Time.timeScale) < float.Epsilon)) {
                        coolMouseLook.LookRotation(transform, playerCamera.transform, false);
                    }
                }
            }
            prevState = playerController.Dead;
        }

        public int TimeUntilRespawn() {
            return (int)(timeOfDeath + respawnTime - Time.time);
        }
    }
}
using UnityEngine;
using System.Collections;
using System;

namespace ExplosionJumping.PlayerControl.Movement {
    [Serializable]
    public class CameraLook {
        public bool clampVerticalRotation = true;
        public float MinimumX = -89F;
        public float MaximumX = 89F;

        private Transform characterTransform;
        private Transform cameraTransform; // Doesn't actually have to be the camera itself, maybe its parent or something.

        public void Init(Transform character, Transform camera) {
            characterTransform = character;
            cameraTransform = camera;
        }

        public void LookRotation(float xRotDelta, float yRotDelta, bool rotateCharacter) {
            if (rotateCharacter) {
                cameraTransform.Rotate(new Vector3(-xRotDelta, 0f, 0f), Space.Self);
            }
            else {
                cameraTransform.Rotate(new Vector3(-xRotDelta, yRotDelta, 0f), Space.Self);
            }

            if (clampVerticalRotation) {
                cameraTransform.localRotation = ClampRotationAroundXAxis(cameraTransform.localRotation);
            }
            if (rotateCharacter) {
                characterTransform.Rotate(new Vector3(0f, yRotDelta, 0f), Space.Self);
            }
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q) {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }
    }
}
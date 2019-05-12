using UnityEngine;
using System.Collections;
using System;

namespace ExplosionJumping.PlayerControl.Movement {
    [Serializable]
    public class CameraLook {
        public bool clampVerticalRotation = true;
        public float MinimumX = -89F;
        public float MaximumX = 89F;

        private Transform m_CharacterTargetRot;
        private Transform m_CameraTargetRot;

        public void Init(Transform character, Transform camera) {
            m_CharacterTargetRot = character;
            m_CameraTargetRot = camera;
        }

        public void LookRotation(float xRotDelta, float yRotDelta, bool rotateCharacter) {
            if (rotateCharacter) {
                m_CameraTargetRot.Rotate(new Vector3(-xRotDelta, 0f, 0f), Space.Self);
            }
            else {
                m_CameraTargetRot.Rotate(new Vector3(-xRotDelta, yRotDelta, 0f), Space.Self);
            }

            if (clampVerticalRotation) {
                m_CameraTargetRot.localRotation = ClampRotationAroundXAxis(m_CameraTargetRot.localRotation);
            }
            if (rotateCharacter) {
                m_CharacterTargetRot.Rotate(new Vector3(0f, yRotDelta, 0f), Space.Self);
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
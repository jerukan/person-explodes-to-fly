using UnityEngine;
using System.Collections;
using System;

namespace ExplosionJumping.PlayerControl.Movement {
    [Serializable]
    public class CameraLook {
        public bool clampVerticalRotation = true;
        public float MinimumX = -89F;
        public float MaximumX = 89F;
        public bool smooth;
        public float smoothTime = 5f;

        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;

        public void Init(Transform character, Transform camera) {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
        }

        public void LookRotation(Transform character, Transform camera, float xRotDelta, float yRotDelta, bool rotateCharacter) {
            m_CharacterTargetRot *= Quaternion.Euler(0f, yRotDelta, 0f);
            if (rotateCharacter) {
                m_CameraTargetRot *= Quaternion.Euler(-xRotDelta, 0f, 0f);
            }
            else {
                m_CameraTargetRot *= Quaternion.Euler(-xRotDelta, yRotDelta, 0f);
            }

            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            if (smooth) {
                character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else {
                if (rotateCharacter) {
                    character.localRotation = m_CharacterTargetRot;
                }
                camera.localRotation = m_CameraTargetRot;
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
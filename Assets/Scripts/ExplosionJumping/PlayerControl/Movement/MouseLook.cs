using System;
using UnityEngine;

namespace ExplosionJumping.PlayerControl.Movement {
    /// <summary>
    /// Ripped straight from the Unity standard assets.
    /// </summary>
    [Serializable]
    public class MouseLook {
        public CameraLook cameraLook;
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool lockCursor = true;

        private bool m_cursorIsLocked = true;

        public MouseLook() {}

        public MouseLook(CameraLook cameraLook) {
            this.cameraLook = cameraLook;
        }

        public void Init(Transform character, Transform camera) {
            cameraLook.Init(character, camera);
        }

        public void LookRotation(bool rotateCharacter) {
            float yRot = Input.GetAxis("Mouse X") * XSensitivity;
            float xRot = Input.GetAxis("Mouse Y") * YSensitivity;

            cameraLook.LookRotation(xRot, yRot, rotateCharacter);

            UpdateCursorLock();
        }

        public void SetCursorLock(bool value) {
            lockCursor = value;
            if (!lockCursor) {//we force unlock the cursor if the user disable the cursor locking helper
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void UpdateCursorLock() {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (lockCursor)
                InternalLockUpdate();
        }

        private void InternalLockUpdate() {
            if (Input.GetKeyUp(KeyCode.Escape)) {
                m_cursorIsLocked = false;
            }
            else if (Input.GetMouseButtonUp(0)) {
                m_cursorIsLocked = true;
            }

            if (m_cursorIsLocked) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!m_cursorIsLocked) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}

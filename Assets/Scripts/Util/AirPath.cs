using System;
using UnityEngine;

namespace ExplosionJumping.Util {
    public struct AirPath {

        private readonly Vector3 originalPosition;
        private readonly Vector3 velocity;

        public AirPath(Vector3 originalPosition, Vector3 velocity) {
            this.originalPosition = originalPosition;
            this.velocity = velocity;
        }

        public AirPath(Vector3 originalPosition, float x, float y, float z) {
            this.originalPosition = originalPosition;
            velocity = new Vector3(x, y, z);
        }

        public Vector3 GetPosition(float time) {
            float futureX = originalPosition.x + time * velocity.x;
            float futureY = originalPosition.y + 0.5f * Physics.gravity.y * time * time + velocity.y * time;
            float futureZ = originalPosition.z + time * velocity.z;
            return new Vector3(futureX, futureY, futureZ);
        }

        public float GetTimeToGround(float height) {
            return (-velocity.y - Mathf.Sqrt(velocity.y * velocity.y - 4f * 0.5f * Physics.gravity.y * height)) / (2 * 0.5f * Physics.gravity.y);
        }
    }
}

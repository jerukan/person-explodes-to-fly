using System;
using UnityEngine;

namespace ExplosionJumping.Util {
    public struct AirPath {

        private readonly Vector3 originalPosition;
        private readonly Vector3 velocity;
        private readonly Vector3 gravity;

        public AirPath(Vector3 originalPosition, Vector3 velocity, Vector3 gravity) {
            this.originalPosition = originalPosition;
            this.velocity = velocity;
            this.gravity = gravity;
        }

        public AirPath(Vector3 originalPosition, Vector3 velocity) : this(originalPosition, velocity, Physics.gravity) {}

        public Vector3 GetPosition(float time) {
            float futureX = originalPosition.x + time * velocity.x;
            float futureY = originalPosition.y + 0.5f * gravity.y * time * time + velocity.y * time;
            float futureZ = originalPosition.z + time * velocity.z;
            return new Vector3(futureX, futureY, futureZ);
        }

        public Vector3 GetVelocity(float time) {
            float futureY = velocity.y + Physics.gravity.y * time;
            return new Vector3(velocity.x, futureY, velocity.z);
        }

        public float GetTimeToGround(float height) {
            // quadratic equation, solve for t, 1/2at^2 + v0t + s
            return (-velocity.y - Mathf.Sqrt(velocity.y * velocity.y - 4f * 0.5f * gravity.y * height)) / (2 * 0.5f * gravity.y);
        }

        public float TimeUntilPeak() {
            return -velocity.y / (gravity.y);
        }

    }
}

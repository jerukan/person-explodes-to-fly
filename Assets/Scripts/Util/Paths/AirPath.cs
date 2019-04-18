using System;
using UnityEngine;

namespace ExplosionJumping.Util.Paths {
    /// <summary>
    /// Represents a path affected by gravity.
    /// Currently only handles gravity with ONLY a y-component.
    /// TODO make it work with actual Vector3 gravity.
    /// </summary>
    public struct AirPath: IPath {

        public readonly Vector3 originalPosition;
        public readonly Vector3 velocity;
        public readonly Vector3 gravity;

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
            float discriminant = velocity.y * velocity.y - 4f * 0.5f * gravity.y * height;
            if(discriminant < 0f) {
                return 0f;
            }
            // quadratic equation, solve for t, 1/2at^2 + v0t + s
            return (-velocity.y - Mathf.Sqrt(discriminant)) / gravity.y;
        }

        public float TimeUntilPeak() {
            return -velocity.y / (gravity.y);
        }

    }
}

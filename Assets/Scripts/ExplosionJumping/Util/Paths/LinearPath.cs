using System;
using UnityEngine;

namespace ExplosionJumping.Util.Paths {
    /// <summary>
    /// Represents a linear path with an acceleration.
    /// Untested, so I might've botched the math.
    /// </summary>
    public struct LinearPath: IPath {

        public readonly Vector3 originalPosition;
        public readonly Vector3 velocity;
        public readonly float acceleration;

        public LinearPath(Vector3 originalPosition, Vector3 velocity, float acceleration) {
            this.originalPosition = originalPosition;
            this.velocity = velocity;
            this.acceleration = acceleration;
        }

        public LinearPath(Vector3 originalPosition, Vector3 velocity) : this(originalPosition, velocity, 0f) { }

        public Vector3 GetPosition(float time) {
            return originalPosition + 0.5f * acceleration * velocity.normalized * time * time + velocity * time;
        }

        public Vector3 GetVelocity(float time) {
            if(Math.Abs(acceleration) < float.Epsilon) {
                return velocity;
            }
            return velocity + acceleration * velocity.normalized * time;
        }

        public float TimeUntilDistance(float distance) {
            if(Math.Abs(acceleration) < float.Epsilon) {
                return distance / velocity.magnitude;
            }
            float discriminant = velocity.sqrMagnitude - 4f * 0.5f * acceleration * distance;
            if(discriminant < 0f) {
                return 0f;
            }
            return (velocity.magnitude - Mathf.Sqrt(discriminant)) / acceleration;
        }
    }
}

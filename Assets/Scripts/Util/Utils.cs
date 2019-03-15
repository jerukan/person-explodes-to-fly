using System;
using UnityEngine;

namespace ExplosionJumping.Util {
    public static class Utils {

        public static Vector2 RotateVector(Vector2 vector, float degrees) {
            double vectorRad = GetVectorRotation(vector);
            double resultRad = vectorRad + degrees * Math.PI / 180;
            return new Vector2((float)(vector.magnitude * Math.Cos(resultRad)), (float)(vector.magnitude * Math.Sin(resultRad)));
        }

        public static double GetVectorRotation(Vector2 vector) {
            return Math.Atan2(vector.y, vector.x);
        }

        /// <summary>
        /// Whether a vector is clockwise or counterclockwise relative to the reference vector.
        /// Sort of like whether a vector is to the left or right of the reference.
        /// </summary>
        /// <returns>1 if clockwise, -1 if counterclockwise, or 0 if they're parallel.</returns>
        public static int VectorIsClockwise(Vector2 reference, Vector2 vector) {
            Vector2 rotatedReference = RotateVector(reference, -90);
            float dot = Vector2.Dot(rotatedReference, vector);
            if(Math.Abs(dot) < float.Epsilon) {
                return 0;
            }
            if(dot < 0) {
                return -1;
            }
            return 1;
        }

        public static float WrapDegree(float theta) {
            float num = (theta - 180f) % 360f;
            if (num < 0) {
                num += 180f;
            } else {
                num -= 180f;
            }
            return num;
        }
    }
}
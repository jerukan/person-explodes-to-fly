using UnityEngine;

namespace ExplosionJumping.Util.Paths {
    public interface IPath {

        Vector3 GetPosition(float time);

        Vector3 GetVelocity(float time);
    }
}

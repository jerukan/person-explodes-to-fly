using UnityEngine;
using System.Collections;
using ExplosionJumping.PlayerControl;

namespace ExplosionJumping.Weapon.Projectile.Explosion {
    public class StandardExplosionController : ExplosionController {
        
        public override void Explode() {
            Vector3 explosionPos = transform.position;
            Collider[] hit = Physics.OverlapSphere(explosionPos, explosionRadius);
            foreach (Collider hitCollider in hit) {
                if (hitCollider.GetComponent<PlayerController>() != null) {
                    Rigidbody hitBody = hitCollider.GetComponent<Rigidbody>();
                    hitBody.AddExplosionForce(explosionForce, explosionPos, explosionRadius, 0f, ForceMode.Impulse);
                    hitCollider.GetComponent<PlayerController>().AddHealth(-(explosionRadius - (explosionPos - hitCollider.ClosestPoint(explosionPos)).magnitude) / explosionRadius * projectileController.damage);
                }
            }
            rigidBody.isKinematic = true;
            GetComponent<Collider>().enabled = false;
            GetComponentInChildren<MeshRenderer>().enabled = false;
            explosionParticles.Play();
            Destroy(gameObject, explosionParticles.main.duration);
        }
    }
}
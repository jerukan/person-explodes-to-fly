using UnityEngine;
using System.Collections;
using ExplosionJumping.PlayerControl;

namespace ExplosionJumping.Weapon.Projectile.Explosion {
    public class StandardExplosionController : ExplosionController {

        public ParticleSystem explosionParticles;
        
        public override void Explode() {
            Vector3 explosionPos = transform.position;
            Collider[] hit = Physics.OverlapSphere(explosionPos, explosionRadius);
            foreach (Collider hitCollider in hit) {
                if (hitCollider.GetComponent<PlayerController>() != null) {
                    Rigidbody hitBody = hitCollider.GetComponent<Rigidbody>();
                    hitCollider.GetComponent<PlayerController>().TakeDamageFromExplosion(explosionPos, explosionRadius, projectileController);
                    hitBody.AddExplosionForce(explosionForce, explosionPos, explosionRadius, 0f, ForceMode.Impulse);
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
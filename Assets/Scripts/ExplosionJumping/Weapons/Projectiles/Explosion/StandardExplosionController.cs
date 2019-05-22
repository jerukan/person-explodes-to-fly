using UnityEngine;
using ExplosionJumping.PlayerControl;

namespace ExplosionJumping.Weapons.Projectiles.Explosion {
    public class StandardExplosionController : ExplosionController {

        public ParticleSystem explosionParticles;
        
        public override void Explode() {
            Vector3 explosionPos = transform.position;
            Collider[] hit = Physics.OverlapSphere(explosionPos, explosionRadius);
            foreach (Collider hitCollider in hit) {
                if (hitCollider.GetComponent<Rigidbody>() != null) {
                    bool affectedByExplosion = true;
                    Rigidbody hitBody = hitCollider.GetComponent<Rigidbody>();
                    if (hitCollider.GetComponent<PlayerController>() != null) {
                        hitCollider.GetComponent<PlayerController>().TakeDamageFromExplosion(explosionPos, explosionRadius, projectileController);
                    }
                    else if(hitCollider.GetComponent<ExplosiveProjectileController>() != null) {
                        affectedByExplosion &= hitCollider.GetComponent<ExplosiveProjectileController>().affectedByOtherExplosions;
                    }
                    if (affectedByExplosion) {
                        hitBody.AddExplosionForce(explosionForce, explosionPos, explosionRadius, 0f, ForceMode.Impulse);
                    }
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
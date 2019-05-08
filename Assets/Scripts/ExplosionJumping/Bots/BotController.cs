using UnityEngine;
using System.Collections;
using UnityEngine.AI;

namespace ExplosionJumping.Bots {
    [RequireComponent(typeof(NavMeshAgent))]
    public class BotController : MonoBehaviour {

        private NavMeshAgent agent;

        private void Awake() {
            agent = GetComponent<NavMeshAgent>();
        }

        // Update is called once per frame
        void Update() {
            agent.SetDestination(GameObject.FindWithTag("Player").transform.position);
        }
    }
}
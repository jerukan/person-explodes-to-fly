using UnityEngine;
using System.Collections;
using ExplosionJumping.UI;

namespace ExplosionJumping.LevelEnvironment {
    [RequireComponent(typeof(Collider))]
    public class DisplayMessageOnTrigger : MonoBehaviour {

        public string message;
        public float timeToDisplay;
        public bool overrideOtherMessages;
        private MessageQueue messageQueue;

        private void Awake() {
            GetComponent<Collider>().isTrigger = true;
        }

        // Use this for initialization
        void Start() {
            messageQueue = GameObject.Find("IngameHud").GetComponent<MessageQueue>();
        }

        // Update is called once per frame
        void Update() {

        }

        private void OnTriggerEnter(Collider other) {
            if (!messageQueue.CurrentMessage.text.Equals(message)) {
                messageQueue.AddMessage(message, timeToDisplay, overrideOtherMessages);
            }
        }
    }
}
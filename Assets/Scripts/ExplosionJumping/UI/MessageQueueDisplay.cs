using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ExplosionJumping.UI {
    [RequireComponent(typeof(MessageQueue))]
    public class MessageQueueDisplay : MonoBehaviour {

        public Text messageText;
        private MessageQueue messageQueue;

        private void Awake() {
            messageQueue = GetComponent<MessageQueue>();
        }

        // Update is called once per frame
        void Update() {
            messageText.text = messageQueue.CurrentMessage.text;
        }
    }
}
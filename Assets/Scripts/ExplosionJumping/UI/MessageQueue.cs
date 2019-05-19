using UnityEngine;
using System.Collections.Generic;

namespace ExplosionJumping.UI {
    public class MessageQueue : MonoBehaviour {

        public class Message {
            public static Message empty = new Message("", 0f);
            public readonly string text;
            public readonly float timeToDisplay;
            public Message(string text, float timeToDisplay) {
                this.text = text;
                this.timeToDisplay = timeToDisplay;
            }
        }

        public const float DEFAULT_DISPLAY_TIME = 4f;
        private Queue<Message> messages = new Queue<Message>();
        private Message currentMessage;
        public Message CurrentMessage {
            get {
                if (currentMessage != null) {
                    return currentMessage;
                }
                return Message.empty;
            }
        }
        private float timeWhenDisplayed;

        void Update() {
            if(Time.time - timeWhenDisplayed >= CurrentMessage.timeToDisplay) {
                if (messages.Count == 0) {
                    currentMessage = null;
                }
                else {
                    timeWhenDisplayed = Time.time;
                    currentMessage = messages.Dequeue();
                }
            }
        }

        public void AddMessage(Message message, bool overrideCurrentMessage) {
            if(overrideCurrentMessage) {
                timeWhenDisplayed = Time.time;
                currentMessage = message;
            } else {
                messages.Enqueue(message);
            }
        }

        public void AddMessage(string message, float timeToDisplay, bool overrideCurrentMessage) {
            AddMessage(new Message(message, timeToDisplay), overrideCurrentMessage);
        }

        public void AddMessage(string message) {
            AddMessage(new Message(message, DEFAULT_DISPLAY_TIME), false);
        }
    }
}
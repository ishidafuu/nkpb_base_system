using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UniExtensions
{
    public class Console : MonoBehaviour
    {
        public static System.Action<ConsoleMessage> OnNewMessage;

        int capacity = 128;
        List<ConsoleMessage> messages;
        static Console instance;

        static Console Instance {
            get {
                if (instance == null) {
                    instance = new GameObject ("Console", typeof(Console)).GetComponent<Console> ();
                    instance.gameObject.hideFlags = HideFlags.HideAndDontSave;
                }
                return instance;
            }
        }

        void Awake ()
        {
            if (instance == null) {
                instance = this;
                instance.messages = new List<ConsoleMessage> (capacity);
            } else {
                Destroy (instance);
            }
        }

        public static List<ConsoleMessage> GetMessageList() {
            return instance.messages;
        }

        public static void Log (string msg)
        {
            var cm = Add (MessageType.Info, msg);
            if(OnNewMessage != null) OnNewMessage(cm);
            if (Application.isEditor)
                Debug.Log (msg);
        }

        public static void Warn (string msg)
        {
            Add (MessageType.Warn, msg);
            if (Application.isEditor)
                Debug.LogWarning (msg);
        }

        public static void Error (string msg)
        {
            Add (MessageType.Error, msg);
            if (Application.isEditor)
                Debug.LogError (msg);
        }

        public static void Exception (System.Exception exception)
        {
            Add (MessageType.Exception, exception.ToString());
            if (Application.isEditor)
                Debug.LogException (exception);
        }

        static ConsoleMessage Add (MessageType type, string message)
        {
            var cm = new ConsoleMessage (type, message);
            instance.messages.Add (cm);
            while (instance.messages.Count > instance.capacity) {
                instance.messages.RemoveAt (0);
            }
            return cm;
        }

        public enum MessageType
        {
            Info,
            Warn,
            Error,
            Exception
        }
        
        public class ConsoleMessage
        {
            public System.DateTime timestamp;
            public MessageType type;
            public string message;
            
            public ConsoleMessage (MessageType type, string message)
            {
                this.type = type;
                this.message = message;
                this.timestamp = System.DateTime.UtcNow;
            }
        }

    
    }
}

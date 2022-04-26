using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class UpdateCaller : MonoBehaviour {
        private static UpdateCaller instance;
        public static void AddUpdateCallback(Action updateMethod) {
            if (instance == null) {
                instance = new GameObject("[Update Caller]").AddComponent<UpdateCaller>();
                instance.transform.parent = FindObjectOfType<CombatManager>().gameObject.transform;

            }
            instance.updateCallback += updateMethod;
        }
        public static void RemoveUpdateCallback(Action updateMethod) {
            if (instance == null) {
                instance = new GameObject("[Update Caller]").AddComponent<UpdateCaller>();
                instance.transform.parent = FindObjectOfType<CombatManager>().gameObject.transform;
            }
            instance.updateCallback -= updateMethod;
        }
 
        private Action updateCallback;
 
        private void Update() {
            updateCallback?.Invoke();
        }
    }
}
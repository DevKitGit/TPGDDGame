using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class NodeEvent : MonoBehaviour
{
    [SerializeField] private GameObject EventPrefab;
    [SerializeField] private GameObject EventLockedPrefab;
    [SerializeField] private bool LockEvent = false;
    
    private Node node;

    private SplineFollower party;
    private bool eventExpended = false;
    // Start is called before the first frame update
    void Start()
    {
        node = GetComponent<Node>();
        party = FindObjectOfType<SplineFollower>();
        party.onNode += HasReachedNode;
    }

    private void HasReachedNode(List<SplineTracer.NodeConnection> passed)
    {
        if (passed[0].node != node) return;
        if (party.GetPercent() == 1.0 || party.GetPercent() == 0.0)
        {
            if (LockEvent)
            {
                DisplayLockEvent();
                //do stuff when event is locked
                return;
            }
            if (eventExpended) return;
            DisplayEvent();
            eventExpended = true;
            //do stuff when event is unlocked and not expended
        }
        
    }

    private void DisplayLockEvent()
    {
        Debug.Log("lock event");
    }

    private void DisplayEvent()
    {
        Debug.Log("event");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

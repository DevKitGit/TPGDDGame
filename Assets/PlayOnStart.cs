using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnStart : MonoBehaviour
{
    [SerializeField] private List<Audio> _audioClips;
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Play(_audioClips[0], true,targetParent:gameObject);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnMapChange : MonoBehaviour
{
    [SerializeField] private List<Audio> _audios;
    private AudioSource currentSource;

    private int sourceIndex;
    // Start is called before the first frame update
    void Awake()
    {
        currentSource = AudioManager.Play(_audios[sourceIndex], true, targetParent: gameObject);
    }

    private void OnDisable()
    {
        currentSource.Pause();
    }

    private void OnEnable()
    {
        currentSource.UnPause();
    }

    public void PlayNextMapClip()
    {
        if (sourceIndex < _audios.Count-1)
        {
            sourceIndex++;
            currentSource.Stop();
            Destroy(currentSource.gameObject);
            currentSource = AudioManager.Play(_audios[sourceIndex], true, targetParent: gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class GameAssets : ScriptableObject
{
	[field:SerializeField]
	public List<PlayerAsset> PlayerAssets { get; private set; }
	
	[field:SerializeField]
	public List<Audio> Audios { get; private set; }
	
	[field:SerializeField]
	public AudioMixer Mixer { get; private set; }
}
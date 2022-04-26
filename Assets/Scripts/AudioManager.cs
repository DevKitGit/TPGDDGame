
using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class AudioManager : MonoBehaviour
{
	private static AudioManager _instance;

	public static AudioManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new GameObject().AddComponent<AudioManager>();
			}
			return _instance;
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	public static void StartGameMusic()
	{
		/*_instance = null;
		
		var startMusic = Play("ben-sexy", looping:true, "MenuSoundtrack");
		var gameMusic = Play("erotic-sexy", looping:true, "IngameSoundtrack");
		var uiSnap = GameManager.Assets.Mixer.FindSnapshot("MainMenu");

		IEnumerator Routine()
		{
			yield return new WaitForEndOfFrame();
			uiSnap.TransitionTo(.3f);
		}

		Instance.StartCoroutine(Routine());*/
		
		SceneManager.sceneLoaded +=SceneManagerOnsceneLoaded; 
	}

	private static void SceneManagerOnsceneLoaded(Scene arg0, LoadSceneMode arg1)
	{
		string snapshot = arg0.buildIndex switch
		{
			0 => "MainMenu",
			1 => "InGame",
			2 => "PostGame",
			_ => "MainMenu",
		};

		var snap = GameManager.Assets.Mixer.FindSnapshot(snapshot);
		if (snap != null)
		{
			snap.TransitionTo(0.5f);
		}
	}

	public static AudioSource Play(Audio audio, bool looping = false, string audioMixerGroup = "SFX", GameObject targetParent = null)
	{
		if (audio == null)
		{
			return null;
		}
		
		GameObject gameObject = new GameObject("One shot audio");
		if (targetParent !=  null)
		{
			gameObject.transform.parent = targetParent.transform;
		}
		AudioSource audioSource = (AudioSource) gameObject.AddComponent(typeof (AudioSource));
		audioSource.clip = audio.Clip;
		audioSource.spatialBlend = 0;
		audioSource.volume = 1;
		audioSource.Play();
		audioSource.loop = looping;
		audioSource.outputAudioMixerGroup = GameManager.Assets.Mixer.FindMatchingGroups(audioMixerGroup).FirstOrDefault();
		if (targetParent == null)
		{
			DontDestroyOnLoad(audioSource.gameObject);
		}
		
		if(!looping)
		{
			Object.Destroy(gameObject, audio.Clip.length * (Time.timeScale < 0.00999999977648258 ? 0.01f : Time.timeScale));
		}

		return audioSource;
	}
	
	public static AudioSource Play(string name, bool looping = false, string audioMixerGroup = "SFX")
	{
		var audio = GameManager.Assets.Audios.FirstOrDefault(a => a.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
		if (audio == null)
		{
			Debug.LogError($"Can't find audio clip \"{name}\"");
			return null;
		}

		return Play(audio, looping, audioMixerGroup);
	}
}

using System;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Rendering;

public class SplashScreenExample : MonoBehaviour
{
	private bool isStopping;
	private IDisposable listener;

	IEnumerator Start()
	{
		InputManager.Instance.InputSystem.enabled = false;
		listener = InputSystem.onAnyButtonPress.Call(_ => Stop());
		
		SplashScreen.Begin();
		while (!SplashScreen.isFinished)
		{
			SplashScreen.Draw();
			yield return null;
		}
		Destroy(gameObject);
	}

	private void OnDisable()
	{
		Stop();
	}

	private void Stop()
	{
		if (isStopping)
		{
			return;
		}
		isStopping = true;
		if (!SplashScreen.isFinished)
		{
			SplashScreen.Stop(SplashScreen.StopBehavior.FadeOut);
		}
		listener?.Dispose();
		if (InputManager.Instance.InputSystem != null)
		{
			InputManager.Instance.InputSystem.enabled = true;
		}
	}
}
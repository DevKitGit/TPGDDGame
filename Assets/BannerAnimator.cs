using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class BannerAnimator : MonoBehaviour
{
    private RectMask2D _rectMask2D;
    private Animator _animator;
    private TextMeshProUGUI _textMeshProUGUI;
    [SerializeField, Range(0, 8)] private float EndPoint;
    [SerializeField, Range(0, 8)] private float StartPoint;
    private AnimationClip introClip, outroClip;
    private float durationA = 410f;
    private float durationb = 410f;
    private float durationToLerpBetween = 410f;
    [Range(0.1f,2f)]public float modifier = 1f;
    private bool IntroActive,OutroActive;
    private Stopwatch _stopwatch;

    private TaskCompletionSource<bool> _taskCompletionSource;
    private static readonly int Start1 = Animator.StringToHash("Start");
    private static readonly int Stop = Animator.StringToHash("Stop");

    void Start()
    {
        _textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
        durationToLerpBetween = durationToLerpBetween * modifier;
        _rectMask2D = GetComponent<RectMask2D>();
        _animator = GetComponent<Animator>();
        introClip = _animator.runtimeAnimatorController.animationClips.First(aClip => aClip.name == "BannerIntro");
        outroClip = _animator.runtimeAnimatorController.animationClips.First(aClip => aClip.name == "BannerOutro");
        durationA = (introClip.events[1].time - introClip.events[0].time) * introClip.length * 1000;
        durationb = (outroClip.events[1].time - outroClip.events[0].time) * outroClip.length * 1000;

        introClip.events[0].functionName = nameof(IntroBegin);
        introClip.events[1].functionName = nameof(IntroEnd);
        outroClip.events[0].functionName = nameof(OutroBegin);
        outroClip.events[1].functionName = nameof(OutroEnd);
    }

    public Task<bool> StartBanner(string textToDisplay,float displayTime)
    {
        _textMeshProUGUI.text = textToDisplay;
        _textMeshProUGUI.ForceMeshUpdate();
        _animator.SetTrigger(Start1);
        Invoke(nameof(StopBanner),displayTime);
        _taskCompletionSource = new TaskCompletionSource<bool>();
        return _taskCompletionSource.Task;
    }
    
    private void StopBanner()
    {
        _animator.SetTrigger(Stop);
    }
    
    private void IntroBegin()
    {
        IntroActive = true;
        _stopwatch = Stopwatch.StartNew();
    }
    private void OutroBegin()
    {
        OutroActive = true;
        _stopwatch = Stopwatch.StartNew();

    }
    private void IntroEnd()
    {
        //print($"intro {_stopwatch.ElapsedMilliseconds}");
    }
    private void OutroEnd()
    {
        //print($"outro {_stopwatch.ElapsedMilliseconds}");

    }
    
    void Update()
    {
        if (IntroActive)
        {

            if (_stopwatch.ElapsedMilliseconds < durationA)
            {
                var value = Mathf.Lerp(StartPoint, EndPoint, _stopwatch.ElapsedMilliseconds / durationA);
                _rectMask2D.padding = new Vector4(value, 0, value, 0);
            }
            else
            {
                IntroActive = false;
                _rectMask2D.padding = new Vector4(EndPoint, 0, EndPoint, 0);
                _stopwatch.Stop();
            }
        }
        else if (OutroActive)
        {
            if (_stopwatch.ElapsedMilliseconds < durationb)
            {
                var value = Mathf.Lerp(EndPoint, StartPoint, _stopwatch.ElapsedMilliseconds / durationb);
                _rectMask2D.padding = new Vector4(value, 0, value, 0);
            }
            else
            {
                OutroActive = false;
                _rectMask2D.padding = new Vector4(StartPoint, 0, StartPoint, 0);
                _stopwatch.Stop();
                _taskCompletionSource?.TrySetResult(true);
                _taskCompletionSource = null;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.UI;

public class ConstrainedCameraFollower : MonoBehaviour
{
    private Transform _partyTransform;
    private Camera _camera;
    [SerializeField] private SpriteRenderer MapSpriteRenderer;

    private float m_xMin, m_xMax, m_yMin, m_yMax;

    private float c_xMin, c_xMax, c_yMin, c_yMax,c_xSize, c_ySize;
    private Vector3 clampedPosition = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        SetupCamera();
    }

    public void SetupCamera()
    {
        _partyTransform = FindObjectOfType<SplineFollower>().gameObject.transform;
        SetMapConstraints();
        var bounds = _camera.ScreenToWorldPoint(new Vector3(Screen.width, 0, _camera.transform.position.z));
        c_xMax = bounds.x;
        c_yMin = bounds.y;
        bounds = _camera.ScreenToWorldPoint(new Vector3(0, Screen.height, _camera.transform.position.z));
        c_xMin = bounds.x;
        c_yMax = bounds.y;
        c_ySize = (c_yMax - c_yMin) / 2;
        c_xSize = (c_xMax - c_xMin) / 2;
    }
    public void SetMapConstraints()
    {
        var bounds = MapSpriteRenderer.bounds;
        m_xMin = bounds.min.x;
        m_yMin = bounds.min.y;
        m_xMax = bounds.max.x;
        m_yMax = bounds.max.y;
    }
    public void TransitionToMap(SpriteRenderer spriteRenderer)
    {
        MapSpriteRenderer = spriteRenderer;
        SetMapConstraints();
        GetComponentInChildren<PlayOnMapChange>().PlayNextMapClip();
    }
    // Update is called once per frame
    void Update()
    {
        _camera.transform.position = new Vector3(
            Mathf.Clamp(_partyTransform.transform.position.x, m_xMin+c_xSize, m_xMax-c_xSize),
            Mathf.Clamp(_partyTransform.transform.position.y, m_yMin+c_ySize, m_yMax-c_ySize),
            _camera.transform.position.z);;
    }
}

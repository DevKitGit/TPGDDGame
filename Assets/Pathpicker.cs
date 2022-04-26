
using System.Collections.Generic;
using System.Linq;
using Dreamteck.Splines;

using UnityEngine;
using UnityEngine.InputSystem;


public class Pathpicker : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private SplineFollower splineFollower;
    [SerializeField] private bool ReadyToMove = false;

    List<GameObject> arrows;
    Node.Connection[] _connections;
    private int currentDirectionIndex = 0;
    private PlayerController _playerController;
    private float followSpeedHolder;
    [SerializeField] private Audio travellingSound;
    private AudioSource _audioSource;
    private float timesinceAudioStart;
    void Awake()
    {
        arrows = new List<GameObject>();
        _connections = new Node.Connection[] { };
        splineFollower.onNode += NodeReached;
        PlayerJoined(PlayerInput.all.ToList().Select(PI => PI.GetComponent<PlayerController>()).ToList());
        ReadyToMove = true;
        followSpeedHolder = splineFollower.followSpeed;
        splineFollower.SetPercent(0.01);
        splineFollower.direction = Spline.Direction.Backward;

    }
    
    private void PlayerJoined(List<PlayerController> playerControllers)
    {
        foreach (var playerController in playerControllers)
        {
            _playerController = playerController;
            playerController.PlayerInput.SwitchCurrentActionMap("World");
            _playerController.WorldNavigate += ReceivePathDirection;
            _playerController.WorldInteract += ReceivePathSelect;
            //_playerController.AssignAction(, ReceivePathDirection);
            //_playerController.AssignAction(_playerController.WorldInteract, ReceivePathSelect);
        }
    }

    public void ReceivePathDirection(InputAction.CallbackContext context)
    {
        if (context.performed && ReadyToMove)
        {
            var thisdir = context.ReadValue<Vector2>();
            if (thisdir.magnitude > new Vector2(0.3f,0.3f).magnitude)
            {
                
                arrows.ForEach(a => a.GetComponent<ArrowDirection>().UpdateArrowState(false));
                var arrow = FindNearestAngle(thisdir);
                
                arrow?.GetComponent<ArrowDirection>().UpdateArrowState(true);
            }
        }
    }

    public void ReceivePathSelect(InputAction.CallbackContext context)
    {
        if (context.performed && ReadyToMove && currentDirectionIndex != -1)
        {
            if (_connections.Length >= 1)
            {
                var connection = _connections[currentDirectionIndex];
                var isEnd = connection.pointIndex >= connection.spline.pointCount / 2f;
                splineFollower.spline = connection.spline;
                
                splineFollower.RebuildImmediate();
                if (!isEnd)
                {
                    splineFollower.direction = Spline.Direction.Forward;
                }
                else
                {
                    splineFollower.direction = Spline.Direction.Backward;
                }
                splineFollower.SetPercent(splineFollower.ClipPercent(connection.spline.GetPointPercent(connection.pointIndex)));
                
                splineFollower.followSpeed = followSpeedHolder;
                ReadyToMove = false;
                if (_audioSource != null)
                {
                    Destroy(_audioSource.gameObject);
                }
                _audioSource = AudioManager.Play(travellingSound, true, targetParent: gameObject);
                timesinceAudioStart = Time.time;
                foreach (var arrowDirection in FindObjectsOfType<ArrowDirection>())
                {
                    Destroy(arrowDirection.gameObject);
                }
            }
            
        }
    }

    
    private GameObject FindNearestAngle(Vector2 direction)
    {
        var smallestVal = float.PositiveInfinity;
        var smallestIndex = 0;
        GameObject smallestGO = null;
        for (var index = 0; index < arrows.Count; index++)
        {
            var arrow = arrows[index];
            var currArrowDiff = new Vector2(arrow.transform.position.x, arrow.transform.position.y) -
                                new Vector2(transform.position.x, transform.position.y);
            if (Vector2.Angle(direction, currArrowDiff.normalized) <= smallestVal)
            {
                smallestVal = Vector2.Angle(direction, currArrowDiff);
                smallestGO = arrow;
                smallestIndex = index;
            }
        }
        currentDirectionIndex = smallestIndex;
        return smallestGO;
    }
    
    private void NodeReached(List<SplineTracer.NodeConnection> passed)
    {
        currentDirectionIndex = -1;
        if (_audioSource != null && Time.time-timesinceAudioStart > 0.1f)
        {
            _audioSource.Stop();
            Destroy(_audioSource.gameObject);
        }
        
        ReadyToMove = splineFollower.GetPercent() == 1.0f || splineFollower.GetPercent() == 0.0;
        if (!ReadyToMove)
        {
            return;
        }
        arrows.ForEach(go => Destroy(go));
        arrows.Clear();
        _connections = passed[0].node.GetConnections();
        foreach (var connection in _connections)
        {
            var points = connection.spline.GetPoints();
            var smallestDist = float.PositiveInfinity;
            SplinePoint smallestPoint = points[0];
            foreach (var point in points)
            {
                var dist = Vector3.Distance(point.position, passed[0].node.transform.position);
                if (dist < smallestDist && dist > 0.001f)
                {
                    smallestDist = Vector3.Distance(point.position, passed[0].node.transform.position);
                    smallestPoint = point;
                }
            }
            var arrow = Instantiate(
                arrowPrefab, 
                passed[0].node.transform.position, 
                Quaternion.identity, 
                GameObject.FindWithTag("WorldScene").transform)
                .GetComponent<ArrowDirection>();
            arrow.UpdateArrowDirection(passed[0].node.transform, smallestPoint.position);
            arrows.Add(arrow.gameObject);
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class ArrowDirection : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float moveInDirectionAmount;
    [SerializeField] public int pointIndex;
    
    public bool IsHighlighted { get; private set; }

    
    public void UpdateArrowState(bool IsHighlighted)
    {
        this.IsHighlighted = IsHighlighted;
        _spriteRenderer.color = this.IsHighlighted ? Color.green : Color.red;
    }

    public void UpdateArrowDirection(Transform originPos, Vector3 destinationPos)
    {
        transform.LookAt(destinationPos,Vector3.forward);
        transform.Rotate(Vector3.right,90);
        transform.Rotate(Vector3.forward,90);
        transform.position = Vector3.MoveTowards(transform.position, destinationPos, moveInDirectionAmount);
    }
    
    public Vector3 Abs(Vector3 input)
    {
        return new(math.abs(input.x), math.abs(input.y), math.abs(input.z));
    }
}

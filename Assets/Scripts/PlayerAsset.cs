using UnityEngine;

[CreateAssetMenu]
public class PlayerAsset : ScriptableObject
{
	public string Name => name;
	
	[field:SerializeField]
	public GameObject Prefab { get; private set; }
	
	[field:SerializeField]
	public Color Color { get; set; } 
	
	[field:SerializeField]
	public Texture2D Texture { get; private set; } 
}
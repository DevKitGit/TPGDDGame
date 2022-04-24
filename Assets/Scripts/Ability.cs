using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefaultNamespace;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Ability
{
    public string Name;
    public Sprite sprite;
    public int ActionCost;
    public int ChargesCurrent;
    public int ChargesMax;
    public bool Ranged;
    public int Radius;
    public List<Effect> Effects;
    public GameObject ProjectilePrefab;
    public GameObject TargetVisualEffect;
    public float ProjectileSpeed;
    [Range(0f, 1f)] public float AttackTimeNormalized;

    private TaskCompletionSource<bool> _rangedProjectileTask;
    private GameObject _spawnedProjectile;
    private Vector3 _targetPos;
    public Ability(AbilitySo abilitySo)
    {
        sprite = abilitySo.sprite;
        ActionCost = abilitySo.ActionCost;
        ChargesCurrent = abilitySo.ChargesCurrent;
        ChargesMax = abilitySo.ChargesMax;
        Ranged = abilitySo.Ranged;
        Radius = abilitySo.Radius;
        Effects = abilitySo.Effects.Select(e => new Effect(e)).ToList();
        ProjectilePrefab = abilitySo.ProjectilePrefab;
        TargetVisualEffect = abilitySo.TargetVisualEffect;
        ProjectileSpeed = abilitySo.ProjectileSpeed;
        AttackTimeNormalized = abilitySo.AttackTimeNormalized;
    }

    public Task<bool> SpawnProjectile(Tile origin, Tile target)
    {
        _rangedProjectileTask = new TaskCompletionSource<bool>();
        _spawnedProjectile = GameObject.Instantiate(ProjectilePrefab,ProjectilePrefab.transform.position+origin.Position_world, ProjectilePrefab.transform.rotation);
        _spawnedProjectile.transform.LookAt(target.Position_world,Vector3.forward);
        _spawnedProjectile.transform.Rotate(Vector3.right,90);
        _spawnedProjectile.transform.Rotate(Vector3.forward,90);
        _targetPos = target.Position_world;
        UpdateCaller.AddUpdateCallback(MoveProjectile);
        return _rangedProjectileTask.Task;
    }

    public void MoveProjectile()
    {
        if (_spawnedProjectile != null)
        {
            if (Vector3.Distance(_spawnedProjectile.transform.position,_targetPos) < 0.01f)
            {
                Debug.Log("hit");
                UpdateCaller.RemoveUpdateCallback(MoveProjectile);
                _rangedProjectileTask.TrySetResult(true);
                GameObject.Destroy(_spawnedProjectile);
                _spawnedProjectile = null;
                return;
            }
            _spawnedProjectile.transform.position = Vector3.MoveTowards(_spawnedProjectile.transform.position, _targetPos,
                ProjectileSpeed);
        }
    }
}
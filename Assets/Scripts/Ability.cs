using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DefaultNamespace;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Ability
{
    public string Name;
    public string Description;
    public Sprite sprite;
    public int ChargeCost;
    public int ChargesCurrent;
    public int ChargesMax;
    public bool Ranged;
    public int Radius;
    public List<Effect> Effects;
    public GameObject ProjectilePrefab;
    public Audio ProjectileAudio,TargetAudio;
    public GameObject TargetVisualEffect;
    public float ProjectileSpeed;
    [Range(0f, 1f)] public float AttackTimeNormalized;
    public TargetType targetType;
    public enum TargetType
    {
        self,
        ally,
        ground,
        enemy,
        pet
    }
    private AudioSource projectileAudioSource;
    
    private bool TargetVisualEffectActive;
    private TaskCompletionSource<bool> _rangedProjectileTask;
    private GameObject _spawnedProjectile, _spawnedVisualEffect;
    private Vector3 _targetPos;
    private Stopwatch TargetVisualEffectTimer;

    public Ability(AbilitySo abilitySo)
    {
        Name = abilitySo.name;
        Description = abilitySo.Description;
        sprite = abilitySo.sprite;
        ChargeCost = abilitySo.ActionCost;
        ChargesCurrent = abilitySo.ChargesCurrent;
        ChargesMax = abilitySo.ChargesMax;
        Ranged = abilitySo.Ranged;
        Radius = abilitySo.Radius;
        Effects = abilitySo.Effects.Select(e => new Effect(e)).ToList();
        ProjectilePrefab = abilitySo.ProjectilePrefab;
        TargetVisualEffect = abilitySo.TargetVisualEffect;
        ProjectileSpeed = abilitySo.ProjectileSpeed;
        AttackTimeNormalized = abilitySo.AttackTimeNormalized;
        targetType = abilitySo.targetType;
        ProjectileAudio = abilitySo.ProjectileAudio;
        TargetAudio = abilitySo.TargetAudio;
    }

    public Task<bool> SpawnProjectile(Tile origin, Tile target)
    {
        _rangedProjectileTask = new TaskCompletionSource<bool>();
        
        _spawnedProjectile = GameObject.Instantiate(ProjectilePrefab,ProjectilePrefab.transform.position+origin.Position_world, ProjectilePrefab.transform.rotation);
        _spawnedProjectile.transform.LookAt(target.Position_world,Vector3.forward);
        _spawnedProjectile.transform.Rotate(Vector3.right,90);
        _spawnedProjectile.transform.Rotate(Vector3.forward,90);
        _targetPos = target.Position_world;
        if (ProjectileAudio != null)
        {
            projectileAudioSource = AudioManager.Play(ProjectileAudio, targetParent:_spawnedProjectile);
        }
        UpdateCaller.AddUpdateCallback(MoveProjectile);
        TargetVisualEffectTimer = new Stopwatch();
        return _rangedProjectileTask.Task;
    }

    public void MoveProjectile()
    {
        
        if (TargetVisualEffectActive && TargetVisualEffectTimer.ElapsedMilliseconds > 600f)
        {
            _rangedProjectileTask.TrySetResult(true);
            TargetVisualEffectActive = false;
            TargetVisualEffectTimer.Stop();
            UpdateCaller.RemoveUpdateCallback(MoveProjectile);
        }
        if (_spawnedProjectile != null)
        {
            if (Vector3.Distance(_spawnedProjectile.transform.position,_targetPos) < 0.01f)
            {
                GameObject.Destroy(_spawnedProjectile);
                _spawnedProjectile = null;
                if (TargetVisualEffect != null)
                {
                    _spawnedVisualEffect = GameObject.Instantiate(TargetVisualEffect, _targetPos + TargetVisualEffect.transform.position,
                        TargetVisualEffect.transform.rotation);
                    if (TargetAudio != null)
                    {
                        AudioManager.Play(TargetAudio, false, targetParent:_spawnedVisualEffect);
                    }
                    TargetVisualEffectActive = true;
                    TargetVisualEffectTimer = Stopwatch.StartNew();
                }
                else
                {
                    _rangedProjectileTask.TrySetResult(true);
                    UpdateCaller.RemoveUpdateCallback(MoveProjectile);
                }
                return;
            }
            _spawnedProjectile.transform.position = Vector3.MoveTowards(_spawnedProjectile.transform.position, _targetPos,
                Time.deltaTime*10);
        }
    }

    public bool Castable()
    {
        return ChargeCost == 0 || ChargesCurrent > 0;
    }

    public void CastAbility()
    {
        ChargesCurrent = Math.Clamp(ChargesCurrent - ChargeCost, 0, ChargesMax);
    }
}
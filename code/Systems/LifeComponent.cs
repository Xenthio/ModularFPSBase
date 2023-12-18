﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPSKit;

public class LifeComponent : Component
{
	/// <summary>
	/// How much Health Points this object has
	/// </summary>
	[Property] public float Health { get; set; } = 100.0f;
	
	/// <summary>
	/// How much Armour Points this object has, just for demo purposes remove if you dont need this
	/// </summary>
	[Property] public float Armour { get; set; } = 35.0f;
	
	/// <summary>
	/// State of life this object has
	/// </summary>
	[Property] public LifeState LifeState { get; set; } = LifeState.Alive;


	[Property] public bool PhysicsImpact { get; set; } = true;

	public Action<DamageInfo> OnTakeDamage;
	public Action OnKilled;

	public void TakeDamage( DamageInfo info )
	{
		Health -= info.Damage;
		if (LifeState == LifeState.Alive && Health <= 0) Kill();
		if (GameObject.Components.TryGet<Rigidbody>(out var rigidbody))
		{
			rigidbody.ApplyImpulseAt( info.Position, info.Force );
		}
		OnTakeDamage(info);
	}

	public void Kill()
	{
		LifeState = LifeState.Dead;
		OnKilled();
	}
}

public enum LifeState
{
	Alive,
	Dead,
	Respawning,
	Limbo
}

public struct DamageInfo
{
	public float Damage { get; set; }
	public ITagSet Tags { get; set; }
	public int Bone { get; set; }
	public Vector3 Position { get; set; }
	public Vector3 Force { get; set; }
	public GameObject Weapon { get; set; }
	public GameObject Attacker { get; set; }
	public static DamageInfo FromBullet(Vector3 position, Vector3 force, float damage)
	{
		var info = new DamageInfo();
		info.Damage = damage;
		info.Tags.Add( "bullet" );
		info.Position = position;
		info.Force = force;
		return info;
	}
	public DamageInfo WithWeapon( GameObject weapon )
	{
		this.Weapon = weapon;
		return this;
	}
	public DamageInfo WithAttacker( GameObject attacker )
	{
		this.Attacker = attacker;
		return this;
	}
	public DamageInfo WithBone( int bone )
	{
		this.Bone = bone;
		return this;
	}
	public DamageInfo WithTag(string tag)
	{
		this.Tags.Set( tag, true );
		return this;
	}
}

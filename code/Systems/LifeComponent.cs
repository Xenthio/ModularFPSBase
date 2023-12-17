using System;
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

	public Action<float,string[]> OnTakeDamage;
	public Action OnKilled;

	public void TakeDamage( float damage, string[] tags)
	{
		Health -= damage;
		if (LifeState == LifeState.Alive && Health <= 0) Kill();
		OnTakeDamage(damage, tags);
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

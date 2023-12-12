using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPSKit;

public class LifeComponent : Component
{
	[Property] public float Health { get; set; } = 100.0f;

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

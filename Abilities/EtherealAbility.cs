using DiskCardGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class EtherealAbility : AbilityBehaviour
{

    public static Ability ability;

    public override Ability Ability
	{
		get
		{
			return ability;
		}
	}

	// No triggers, handled by TakeDamage

}
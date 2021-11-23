using DiskCardGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CannonAbility : AbilityBehaviour
{

	public static Ability ability;

	public override Ability Ability
	{
		get
		{
			return ability;
		}
	}

    public override IEnumerator OnUpkeep(bool playerUpkeep)
    {
		if (playerUpkeep)
		{
			yield return ConsumeBall();
		}
		yield break;
    }

    public override bool RespondsToUpkeep(bool playerUpkeep)
    {
		if (playerUpkeep) return true;
		else return false;
    }

    public override IEnumerator OnPlayFromHand()
    {
		yield return ConsumeBall();
	}

	public override bool RespondsToPlayFromHand()
	{
		return true;
	}

	private IEnumerator ConsumeBall()
    {
		foreach (PlayableCard handCard in Singleton<PlayerHand>.Instance.CardsInHand)
		{
			if (handCard.Info.name == "PBM_Cannonball")
			{
				yield return new WaitForSeconds(0.5f);
				Singleton<ViewManager>.Instance.SwitchToView(View.Hand);
				yield return new WaitForSeconds(0.5f);
				handCard.Anim.PlayDeathAnimation(false);
				Destroy(handCard.gameObject, 1f);
				Singleton<PlayerHand>.Instance.RemoveCardFromHand(handCard);
				yield return new WaitForSeconds(1f);
				Singleton<ViewManager>.Instance.SwitchToView(View.Board);
				yield return new WaitForSeconds(0.5f);
				Card.Anim.PlayTransformAnimation();
				yield return new WaitForSeconds(0.15f);
				Card.AddTemporaryMod(new CardModificationInfo()
				{
					attackAdjustment = 1,
					RemoveOnUpkeep = true
				});
				yield return new WaitForSeconds(1f);
				yield break;
			}
		}
	}
}
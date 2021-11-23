using BepInEx;
using BepInEx.Logging;
using DiskCardGame;
using HarmonyLib;
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace PirateBoss.Boss
{

	public class PirateBossSequencer : Part1BossBattleSequencer
    {

        public static PlayableCard blocker = null;
        public static Vector3 blockerPos;

        public override Opponent.Type BossType => (Opponent.Type)100;

        public override StoryEvent DefeatedStoryEvent => (StoryEvent)1000;

        public override IEnumerator PlayerCombatStart()
        {
            if (Singleton<TurnManager>.Instance.Opponent.NumLives > 1)
            {
                yield return new WaitForSeconds(0.5f);
                List<PlayableCard> best = new List<PlayableCard>();
                int bestAtk = 1;
                Singleton<BoardManager>.Instance.PlayerSlotsCopy.ForEach(pSlot =>
                {
                    if (pSlot.Card != null)
                    {
                        if (pSlot.Card.Attack > bestAtk)
                        {
                            best.Clear();
                            bestAtk = pSlot.Card.Attack;
                            best.Add(pSlot.Card);
                        }
                        else if (pSlot.Card.Attack == bestAtk)
                        {
                            best.Add(pSlot.Card);
                        }
                    }
                });
                if (best.Count > 0)
                {
                    int currentRandomSeed = SaveManager.SaveFile.GetCurrentRandomSeed();
                    int index = SeededRandom.Range(0, best.Count, currentRandomSeed + TurnNumber);

                    CardInfo card = (CardInfo)CardLoader.GetCardByName("Banshee").Clone();
                    card.Mods.Add(new CardModificationInfo()
                    {
                        abilities = new List<Ability>() { Plugin.negateAbility },
                        fromCardMerge = true
                    });

                    blocker = CardSpawner.SpawnPlayableCard(card);

                    blocker.Anim.SetHovering(true);

                    blocker.gameObject.transform.position = best[index].transform.position;
                    blocker.gameObject.transform.Translate(new Vector3(0, 1f, -11f));
                    blocker.gameObject.transform.Rotate(new Vector3(-90f, 0, 0));

                    blockerPos = blocker.gameObject.transform.position;

                    Tween.Position(blocker.gameObject.transform, new Vector3(blockerPos.x, blockerPos.y - 10, blockerPos.z), 0.5f, 0, Tween.EaseOut);
                    yield return new WaitForSeconds(0.5f);
                    best[index].AddTemporaryMod(new CardModificationInfo()
                    {
                        attackAdjustment = -bestAtk,
                        RemoveOnUpkeep = true
                    });
                    best[index].Anim.StrongNegationEffect();
                    if (!ProgressionData.LearnedMechanic((MechanicsConcept)100))
                    {
                        yield return Singleton<TextDisplayer>.Instance.ShowUntilInput("Yer " + best[index].Info.DisplayedNameEnglish + " can't do nothin' ta me now.");
                        ProgressionData.SetMechanicLearned((MechanicsConcept)100);
                    }
                }
                yield return new WaitForSeconds(0.5f);
            } else
            {
                
                yield return new WaitForSeconds(0.5f);
            }
        }

        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            if (blocker != null)
            {
                Tween.Position(blocker.gameObject.transform, new Vector3(blockerPos.x, blockerPos.y, blockerPos.z), 0.5f, 0, Tween.EaseIn);
                Destroy(blocker);
                yield return new WaitForSeconds(0.5f);
            }
        }

        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            return true;
        }

        public override EncounterData BuildCustomEncounter(CardBattleNodeData nodeData)
        {
            EncounterData encounterData = base.BuildCustomEncounter(nodeData);
            EncounterBlueprintData blueprint = ScriptableObject.CreateInstance<PirateBossP1Blueprint>();
            encounterData.Blueprint = blueprint;
            encounterData.opponentTurnPlan = EncounterBuilder.BuildOpponentTurnPlan(blueprint, 15 + RunState.Run.difficultyModifier, removeLockedCards: false);
            EncounterData.StartCondition startCondition = new EncounterData.StartCondition();

            startCondition.cardsInOpponentSlots[1] = CardLoader.GetCardByName("Skeleton");
            encounterData.startConditions.Add(startCondition);
            return encounterData;
        }

        public static IEnumerator CreateBlocker()
        {
            yield return new WaitForSeconds(0.5f);
            CardInfo card = (CardInfo)CardLoader.GetCardByName("Banshee").Clone();
            blocker = CardSpawner.SpawnPlayableCard(card);

            blocker.Anim.SetHovering(true);

            blocker.gameObject.transform.position = Singleton<BoardManager>.Instance.PlayerSlotsCopy[0].transform.position;
            blocker.gameObject.transform.Translate(new Vector3(0, 1f, -11f));
            blocker.gameObject.transform.Rotate(new Vector3(-90f, 0, 0));

            blockerPos = blocker.gameObject.transform.position;

            Tween.Position(blocker.gameObject.transform, new Vector3(blockerPos.x, blockerPos.y - 10, blockerPos.z), 0.5f, 0, Tween.EaseOut);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
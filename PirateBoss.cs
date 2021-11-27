using BepInEx;
using BepInEx.Logging;
using DiskCardGame;
using HarmonyLib;
using System;
using PirateBoss.Boss;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using APIPlugin;

namespace PirateBoss
{

    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "IngoH.inscryption.PirateBoss";
        private const string PluginName = "PirateBoss";
        private const string PluginVersion = "1.0.0";

        internal static ManualLogSource Log;

        private void Awake()
        {
            Logger.LogInfo($"Loaded {PluginName}!");
            Plugin.Log = base.Logger;

            Harmony harmony = new Harmony(PluginGuid);
            harmony.PatchAll();

            CreateCards();
        }

        public static AbilityIdentifier cannonId;
        public static AbilityIdentifier negateId;
        public static AbilityIdentifier etherealId;

        public static Ability cannonAbility;
        public static Ability negateAbility;
        public static Ability etherealAbility;

        private void CreateCards()
        {

            // TODO: Cleanup

            byte[] imgBytes = File.ReadAllBytes(Path.Combine(Info.Location.Replace("PirateBoss.dll", ""), "Artwork/cannonball.png"));
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imgBytes);

            NewCard.Add("PBM_Cannonball", "Cannonball", 0, 1, new List<CardMetaCategory>(), CardComplexity.Simple, CardTemple.Nature, defaultTex: tex, appearanceBehaviour: new List<CardAppearanceBehaviour.Appearance>() { CardAppearanceBehaviour.Appearance.TerrainBackground, CardAppearanceBehaviour.Appearance.TerrainLayout }, traits: new List<Trait>() { Trait.Terrain });

            byte[] imgBytes2 = System.IO.File.ReadAllBytes(Path.Combine(this.Info.Location.Replace("PirateBoss.dll", ""), "Artwork/cannonability.png"));
            Texture2D tex2 = new Texture2D(2, 2);
            tex2.LoadImage(imgBytes2);

            AbilityInfo info = ScriptableObject.CreateInstance<AbilityInfo>();
            info.powerLevel = 1;
            info.rulebookName = "Cannon";
            info.rulebookDescription = "Each turn, consumes a cannonball from your hand to gain 1 power for that turn.";
            info.metaCategories = new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook };

            NewAbility cannon = new NewAbility(info, typeof(CannonAbility), tex2, AbilityIdentifier.GetAbilityIdentifier(PluginGuid, "CannonAbility"));
            cannonId = cannon.id;
            cannonAbility = cannon.ability;

            byte[] imgBytes3 = File.ReadAllBytes(Path.Combine(Info.Location.Replace("PirateBoss.dll", ""), "Artwork/cannon.png"));
            Texture2D tex3 = new Texture2D(2, 2);
            tex3.LoadImage(imgBytes3);

            NewCard.Add("PBM_Cannon", "Cannon", 0, 5, new List<CardMetaCategory>(), CardComplexity.Advanced, CardTemple.Nature, defaultTex: tex3, appearanceBehaviour: new List<CardAppearanceBehaviour.Appearance>() { CardAppearanceBehaviour.Appearance.TerrainBackground, CardAppearanceBehaviour.Appearance.TerrainLayout }, traits: new List<Trait>() { Trait.Terrain }, abilities: new List<Ability>() { Ability.Sniper }, abilityIdsParam: new List<AbilityIdentifier>() { cannonId });

            byte[] imgBytes4 = File.ReadAllBytes(Path.Combine(Info.Location.Replace("PirateBoss.dll", ""), "Artwork/etherealability.png"));
            Texture2D tex4 = new Texture2D(2, 2);
            tex4.LoadImage(imgBytes4);

            AbilityInfo info2 = ScriptableObject.CreateInstance<AbilityInfo>();
            info2.powerLevel = 2;
            info2.rulebookName = "Ethereal";
            info2.rulebookDescription = "When [creature] is attacked, the damage to this creature is reduced to 1.";
            info2.metaCategories = new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook };

            NewAbility ethereal = new NewAbility(info2, typeof(EtherealAbility), tex4, AbilityIdentifier.GetAbilityIdentifier(PluginGuid, "EtherealAbility"));
            etherealId = ethereal.id;
            etherealAbility = ethereal.ability;

            byte[] imgBytes5 = File.ReadAllBytes(Path.Combine(Info.Location.Replace("PirateBoss.dll", ""), "Artwork/spectral_skeleton.png"));
            Texture2D tex5 = new Texture2D(2, 2);
            tex5.LoadImage(imgBytes5);

            NewCard.Add("PBM_Spectral_Skeleton", "Spectral Skeleton", 1, 2, new List<CardMetaCategory>(), CardComplexity.Intermediate, CardTemple.Nature, defaultTex: tex5, abilityIdsParam: new List<AbilityIdentifier>() { etherealId });
            
            byte[] imgBytes6 = File.ReadAllBytes(Path.Combine(Info.Location.Replace("PirateBoss.dll", ""), "Artwork/ghost_shark.png"));
            Texture2D tex6 = new Texture2D(2, 2);
            tex6.LoadImage(imgBytes6);

            NewCard.Add("PBM_Ghost_Shark", "Ghost Shark", 3, 3, new List<CardMetaCategory>(), CardComplexity.Intermediate, CardTemple.Nature, defaultTex: tex6, abilityIdsParam: new List<AbilityIdentifier>() { etherealId });

            byte[] imgBytes7 = File.ReadAllBytes(Path.Combine(Info.Location.Replace("PirateBoss.dll", ""), "Artwork/destroyed_skeleton.png"));
            Texture2D tex7 = new Texture2D(2, 2);
            tex7.LoadImage(imgBytes7);

            NewCard.Add("PBM_Destroyed_Skeleton", "Recurring Skeleton", 0, 1, new List<CardMetaCategory>(), CardComplexity.Intermediate, CardTemple.Nature, defaultTex: tex7, abilities: new List<Ability>() { Ability.Evolve },  evolveId: new EvolveIdentifier("PBM_Reassembling_Skeleton", 1));

            byte[] imgBytes8 = File.ReadAllBytes(Path.Combine(Info.Location.Replace("PirateBoss.dll", ""), "Artwork/recurring_skeleton.png"));
            Texture2D tex8 = new Texture2D(2, 2);
            tex8.LoadImage(imgBytes8);

            NewCard.Add("PBM_Recurring_Skeleton", "Recurring Skeleton", 1, 1, new List<CardMetaCategory>(), CardComplexity.Intermediate, CardTemple.Nature, defaultTex: tex8, abilities: new List<Ability>() { Ability.IceCube, Ability.Strafe }, iceCubeId: new IceCubeIdentifier("PBM_Destroyed_Skeleton"));

            byte[] imgBytes9 = File.ReadAllBytes(Path.Combine(Info.Location.Replace("PirateBoss.dll", ""), "Artwork/negateattackability.png"));
            Texture2D tex9 = new Texture2D(2, 2);
            tex9.LoadImage(imgBytes9);

            AbilityInfo info3 = ScriptableObject.CreateInstance<AbilityInfo>();
            info3.powerLevel = 2;
            info3.rulebookName = "Attack Negation";
            info3.rulebookDescription = "Your strongest creature on the board does not attack.";
            info3.metaCategories = new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook };

            NewAbility negate = new NewAbility(info3, typeof(NullAbility), tex9, AbilityIdentifier.GetAbilityIdentifier(PluginGuid, "NegateAttackAbility"));
            negateId = negate.id;
            negateAbility = negate.ability;

            byte[] imgBytes10 = File.ReadAllBytes(Path.Combine(Info.Location.Replace("PirateBoss.dll", ""), "Artwork/reassembling_skeleton.png"));
            Texture2D tex10 = new Texture2D(2, 2);
            tex10.LoadImage(imgBytes10);

            NewCard.Add("PBM_Reassembling_Skeleton", "Recurring Skeleton", 1, 1, new List<CardMetaCategory>(), CardComplexity.Intermediate, CardTemple.Nature, defaultTex: tex10, abilities: new List<Ability>() { Ability.IceCube, Ability.Brittle }, iceCubeId: new IceCubeIdentifier("PBM_Destroyed_Skeleton"));
        }

        [HarmonyPatch(typeof(TurnManager), "StartGame", new Type[] {typeof(CardBattleNodeData)})]
        public class BossPatch : TurnManager
        {
            public static bool Prefix(TurnManager __instance, CardBattleNodeData nodeData)
            {
                // Overrides trapper/trader
                if (nodeData.specialBattleId == "TrapperTraderBattleSequencer")
                {
                    __instance.StartGame(null, "PirateBossSequencer");
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(Opponent), "SpawnOpponent", new Type[] { typeof(EncounterData) })]
        public class SpawnOpponentPatch
        {
            public static bool Prefix(TurnManager __instance, ref Opponent __result, EncounterData encounterData)
            {
                if (encounterData.opponentType == (Opponent.Type)100)
                {
                    GameObject gameObject = new GameObject();
                    gameObject.name = "Opponent";
                    Opponent.Type type = encounterData.opponentType;
                    Part1BossOpponent opponent = null;
                    opponent = gameObject.AddComponent<PirateBossOpponnent>();
                    string text = encounterData.aiId;
                    if (string.IsNullOrEmpty(text))
                    {
                        text = "AI";
                    }
                    opponent.AI = Activator.CreateInstance(CustomType.GetType("DiskCardGame", text)) as AI;
                    opponent.NumLives = opponent.StartingLives;
                    opponent.OpponentType = type;
                    opponent.TurnPlan = opponent.ModifyTurnPlan(encounterData.opponentTurnPlan);
                    opponent.Blueprint = encounterData.Blueprint;
                    opponent.Difficulty = encounterData.Difficulty;
                    opponent.ExtraTurnsToSurrender = SeededRandom.Range(0, 3, SaveManager.SaveFile.GetCurrentRandomSeed());
                    __result = opponent;
                    return false;
                } else
                {
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(TurnManager), "UpdateSpecialSequencer", new Type[] { typeof(string) })]
        public class SequencerPatch : TurnManager
        {
            public static bool Prefix(TurnManager __instance, string specialBattleId)
            {
                if (specialBattleId == "PirateBossSequencer")
                {
                    Destroy(__instance.SpecialSequencer);
                    __instance.GetType().GetProperty("SpecialSequencer").SetValue(__instance, __instance.gameObject.AddComponent(typeof(PirateBossSequencer)));
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(LeshyAnimationController), "SpawnMask", new Type[] { typeof(Mask), typeof(bool) })]
        public class MaskPatch : LeshyAnimationController
        {
            public static bool Prefix(LeshyAnimationController __instance, Mask mask, ref GameObject ___currentHeldMask, ref Transform ___maskParent, ref Transform ___heldMaskParent)
            {
                if (mask == (Mask)10)
                {
                    GameObject currentMask = __instance.CurrentMask;
                    if (currentMask != null)
                    {
                        Destroy(currentMask);
                        Destroy(___currentHeldMask);
                    }
                    GameObject normal = ResourceBank.Get<GameObject>("Prefabs/Opponents/Leshy/Masks/MaskProspector");
                    GameObject original = ResourceBank.Get<GameObject>("Prefabs/Opponents/Grimora/RoyalBossSkull");
                    GameObject faceMask = Instantiate(original, ___maskParent);
                    faceMask.transform.localPosition = normal.transform.localPosition;
                    faceMask.transform.Translate(new Vector3(-0.5f, 1.7f, 0));
                    faceMask.transform.Rotate(new Vector3(180f, 0f, -35f));
                    GameObject handMask = Instantiate(original, ___heldMaskParent);
                    handMask.transform.localPosition = normal.transform.localPosition;
                    handMask.transform.Translate(new Vector3(-0.5f, 1.75f, 0));
                    handMask.transform.Rotate(new Vector3(180f, 0f, -35f));
                    __instance.GetType().GetProperty("CurrentMask").SetValue(__instance, faceMask);
                    ___currentHeldMask = handMask;
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(CombatPhaseManager), "SlotAttackSequence", new Type[] { typeof(CardSlot) })]
        public class AttackPatch : CombatPhaseManager
        {
            public static bool Prefix(CombatPhaseManager __instance, CardSlot slot)
            {
                if (slot.Card.Attack == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(PlayableCard), "Die", new Type[] { typeof(bool), typeof(PlayableCard), typeof(bool) })]
        public class DiePatch : PlayableCard
        {
            public static bool Prefix(ref PlayableCard __instance)
            {
                if (Singleton<TurnManager>.Instance.Opponent != null && Singleton<TurnManager>.Instance.Opponent.Blueprint != null && Singleton<TurnManager>.Instance.Opponent.Blueprint.name != null && Singleton<TurnManager>.Instance.Opponent.Blueprint.name == "DeathPreventionBlueprint") return false;
                else return true;
            }
        }

        [HarmonyPatch(typeof(PlayableCard), "TakeDamage", new Type[] { typeof(int), typeof(PlayableCard) })]
        public class EtherealPatch : PlayableCard
        {
            public static bool Prefix(ref PlayableCard __instance, ref int damage)
            {
                if (__instance.HasAbility(etherealAbility)) damage = 1;
                return true;
            }
        }

        [HarmonyPatch(typeof(CombatPhaseManager), "DealOverkillDamage", new Type[] { typeof(int), typeof(CardSlot), typeof(CardSlot) })]
        public class EtherealOverkillPatch : CombatPhaseManager
        {
            public static bool Prefix(ref CombatPhaseManager __instance, CardSlot opposingSlot)
            {
                if (opposingSlot.Card != null && opposingSlot.Card.HasAbility(etherealAbility)) return false;
                return true;
            }
        }
    }
}

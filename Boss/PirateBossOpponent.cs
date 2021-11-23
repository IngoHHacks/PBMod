using BepInEx;
using BepInEx.Logging;
using DiskCardGame;
using HarmonyLib;
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace PirateBoss.Boss
{

    public class PirateBossOpponnent : Part1BossOpponent
    {
        public override string BlueprintSubfolderName => "";

        public override string DefeatedPlayerDialogue => "Arr! Ye'll be walkin' the plank for yer failure!";

        public override IEnumerator IntroSequence(EncounterData encounter)
        {
            AudioController.Instance.FadeOutLoop(0.75f);
            yield return base.IntroSequence(encounter);
            yield return new WaitForSeconds(0.75f);
            Singleton<ViewManager>.Instance.SwitchToView(View.Default, immediate: false, lockAfter: true);
            yield return Singleton<TextDisplayer>.Instance.ShowUntilInput("I know this is Grimora's theme, but...");
            LeshyAnimationController.Instance.PutOnMask((LeshyAnimationController.Mask)10);
            AudioController.Instance.SetLoopAndPlay("boss_trappertrader_base");
            //AudioController.Instance.SetLoopAndPlay("boss_trappertrader_ambient", 1);
            AudioController.Instance.SetLoopVolume(0.25f, 4f, 1);
            AudioController.Instance.BaseLoopSource.pitch *= 2f;
            yield return new WaitForSeconds(1.55f);
            SetSceneEffectsShown(true);
            Singleton<ViewManager>.Instance.SwitchToView(View.BossCloseup, immediate: false, lockAfter: true);
            StartCoroutine(Singleton<TextDisplayer>.Instance.ShowThenClear("[c:bR]Y'AARRRRR![c:]", 1.5f));
            yield return new WaitForSeconds(1.5f);
            Singleton<ViewManager>.Instance.SwitchToView(View.Default);
            yield return new WaitForSeconds(0.75f);
            sceneryObject = Instantiate(ResourceBank.Get<GameObject>("Prefabs/Environment/TableEffects/CannonTableEffects"));
        }

        public static string GetGameObjectPath(GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }

        public override void SetSceneEffectsShown(bool showEffects)
        {
            if (showEffects)
            {
                Color brightBlue = GameColors.Instance.brightBlue;
                brightBlue.a = 0.5f;
                Singleton<TableVisualEffectsManager>.Instance.ChangeTableColors(GameColors.Instance.blue, GameColors.Instance.marigold, GameColors.Instance.brightBlue, brightBlue, GameColors.Instance.brightBlue, GameColors.Instance.brightBlue, GameColors.Instance.gray, GameColors.Instance.gray, GameColors.Instance.lightGray);
            }
            else
            {
                Singleton<TableVisualEffectsManager>.Instance.ResetTableColors();
            }
        }

        public void SetP2SceneEffectsShown(bool showEffects)
        {
            Color lightPurple = GameColors.Instance.lightPurple;
            lightPurple.a = 0.5f;
            Singleton<TableVisualEffectsManager>.Instance.ChangeTableColors(GameColors.Instance.purple, GameColors.Instance.yellow, GameColors.Instance.lightPurple, lightPurple, GameColors.Instance.lightPurple, GameColors.Instance.lightPurple, GameColors.Instance.gray, GameColors.Instance.gray, GameColors.Instance.lightGray);
        }

        public override IEnumerator StartNewPhaseSequence()
        {
            base.TurnPlan.Clear();
            SetP2SceneEffectsShown(true);
            yield return new WaitForSeconds(0.2f);
            yield return ClearBoard();
            yield return ClearQueue();
            yield return new WaitForSeconds(0.4f);
            if (!ProgressionData.LearnedMechanic((MechanicsConcept)110))
            {
                yield return Singleton<TextDisplayer>.Instance.ShowUntilInput("Ye thought me cannons were just fer decoration?");
                ProgressionData.SetMechanicLearned((MechanicsConcept)110);
            } else
            {
                yield return Singleton<TextDisplayer>.Instance.ShowUntilInput("Yer cards are about ta be blown ta smithereens!");
            }
            yield return new WaitForSeconds(0.25f);
            Blueprint = ScriptableObject.CreateInstance<DeathPreventionBlueprint>(); // Sets a flag to cancel dying
            yield return FireCannonsSequence();
            List<CardSlot> list = Singleton<BoardManager>.Instance.OpponentSlotsCopy.FindAll((CardSlot x) => x.opposingSlot.Card != null);
            CardInfo blockCard = (CardInfo) CardLoader.GetCardByName("PBM_Spectral_Skeleton").Clone();
            blockCard.Mods.Add(new CardModificationInfo() { abilities = new List<Ability>() { Ability.Reach }, fromCardMerge = true });
            foreach (CardSlot item in list)
            {
                yield return Singleton<BoardManager>.Instance.CreateCardInSlot(blockCard, item, 0.2f);
                yield return new WaitForSeconds(0.25f);
            }
            Blueprint = ScriptableObject.CreateInstance<PirateBossP2Blueprint>();
            List<List<CardInfo>> plan = EncounterBuilder.BuildOpponentTurnPlan(Blueprint, 0, removeLockedCards: false);
            ReplaceAndAppendTurnPlan(plan);
            yield return QueueNewCards();
        }

        private IEnumerator FireCannonsSequence()
        {
            Singleton<ViewManager>.Instance.SwitchToView(View.Default, lockAfter: true);

            Vector3 leshyLoc = LeshyAnimationController.Instance.gameObject.transform.position;

            yield return new WaitForSeconds(0.25f);
            if (PirateBossSequencer.blocker == null)
            {
                yield return PirateBossSequencer.CreateBlocker();
            }
            CannonTableEffects eff = sceneryObject.GetComponent<CannonTableEffects>();
            List<Transform> cannonParents = (List<Transform>)eff.GetType().GetField("cannonParents", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(eff);
            foreach (Transform cannon in cannonParents)
            {
                Tween.Rotate(cannon.transform, new Vector3(45f, 0f, 0f), Space.Self, 0.5f, 0f, Tween.EaseInOut);
            }
            yield return new WaitForSeconds(0.75f);
            int totalHealth = 0;
            List<CardSlot> slots = Singleton<BoardManager>.Instance.PlayerSlotsCopy;
            List<PlayableCard> boardCards = new List<PlayableCard>();
            Singleton<ViewManager>.Instance.SwitchToView(View.Board, lockAfter: true);
            List<PlayableCard> deadCardsBoard = new List<PlayableCard>();
            foreach (CardSlot slot in slots)
            {
                if (slot.Card != null)
                {
                    totalHealth += slot.Card.Health;
                    boardCards.Add(slot.Card);
                }
            }
            List<ParticleSystem> particleSystems = new List<ParticleSystem>();
            Texture2D circle = (Texture2D) Resources.Load("art/basic/circle_image");
            yield return new WaitForSeconds(1f);
            int count = (int) Math.Round(totalHealth * 0.67) + 1;
            for (int i = 0; i < count; i++)
            {
                GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                ball.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.2f, 0.2f, 0.2f));
                int currentRandomSeed = SaveManager.SaveFile.GetCurrentRandomSeed();
                ParticleSystem particleSystem = null;
                int target = SeededRandom.Range(0, boardCards.Count, currentRandomSeed + Singleton<TurnManager>.Instance.TurnNumber + i);
                if (i != count - 1)
                {
                    ball.transform.position = boardCards[target].transform.position;
                    ball.transform.Translate(new Vector3(0f, 20f, 0f));
                    Tween.Position(ball.transform, boardCards[target].transform.position, 0.25f, 0, Tween.EaseIn);
                    yield return new WaitForSeconds(0.25f);
                    Destroy(ball);
                    particleSystem = boardCards[target].gameObject.GetComponent<ParticleSystem>();
                    if (particleSystem == null) particleSystem = boardCards[target].gameObject.AddComponent<ParticleSystem>();
                } else
                {
                    ball.transform.position = PirateBossSequencer.blocker.transform.position;
                    ball.transform.Translate(new Vector3(0f, 20f, 0f));
                    Tween.Position(ball.transform, PirateBossSequencer.blocker.transform.position, 0.25f, 0, Tween.EaseIn);
                    yield return new WaitForSeconds(0.25f);
                    Destroy(ball);
                    particleSystem = PirateBossSequencer.blocker.gameObject.GetComponent<ParticleSystem>();
                    if (particleSystem == null) particleSystem = PirateBossSequencer.blocker.gameObject.AddComponent<ParticleSystem>();
                }
                particleSystems.Add(particleSystem);
                ParticleSystem.MainModule main = particleSystem.main;
                ParticleSystem.EmissionModule emission = particleSystem.emission;
                ParticleSystem.ShapeModule shape = particleSystem.shape;
                ParticleSystem.SizeOverLifetimeModule sizeOverLifetimeModule = particleSystem.sizeOverLifetime;
                ParticleSystemRenderer renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
                main.duration = 1.0f;
                main.startSpeed = 0.25f;
                main.loop = false;
                main.startLifetime = new ParticleSystem.MinMaxCurve(0.5f, 1.0f);
                main.startSize = new ParticleSystem.MinMaxCurve(1f, 1.5f);
                emission.rateOverTime = 0;
                emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 30) });
                shape.shapeType = ParticleSystemShapeType.Sphere;
                sizeOverLifetimeModule.enabled = true;
                sizeOverLifetimeModule.size = new ParticleSystem.MinMaxCurve(1.0f, new AnimationCurve(new Keyframe[]
                {
                    new Keyframe(0, 0.5f),
                    new Keyframe(0.25f, 1f),
                    new Keyframe(1f, 0),
                }));

                renderer.receiveShadows = false;
                renderer.material.mainTexture = circle;
                renderer.material.color = new Color(0.2f, 0.2f, 0);
                renderer.material.EnableKeyword("_ALPHATEST_ON");
                renderer.material.renderQueue = 2450;

                particleSystem.Play();

                if (i != count - 1)
                {
                    yield return boardCards[target].TakeDamage(1, null);
                    if (boardCards[target].Health == 0 && !deadCardsBoard.Contains(boardCards[target])) deadCardsBoard.Add(boardCards[target]);
                    boardCards[target].Anim.PlayHitAnimation();
                }
                else
                {
                    yield return PirateBossSequencer.blocker.TakeDamage(1, null);
                }
            }
            Blueprint = ScriptableObject.CreateInstance<PirateBossP2Blueprint>();
            PirateBossSequencer.blocker.Slot = Singleton<BoardManager>.Instance.OpponentSlotsCopy[0];
            yield return PirateBossSequencer.blocker.Die(false);
            yield return new WaitForSeconds(0.75f);
            Singleton<ViewManager>.Instance.SwitchToView(View.BossCloseup, immediate: false, lockAfter: true);
            yield return Singleton<TextDisplayer>.Instance.ShowUntilInput("[c:bR]Arrrgh![c:] Me banshee! Yer gonna pay fer that!");
            Singleton<ViewManager>.Instance.SwitchToView(View.Default);
            yield return new WaitForSeconds(0.5f);
            foreach (PlayableCard card in deadCardsBoard)
            {
                yield return card.Die(false);
            }
            Blueprint = ScriptableObject.CreateInstance<DeathPreventionBlueprint>();
            ProgressionData.SetMechanicLearned((MechanicsConcept)111);
            int totalHealth2 = 0;
            List<PlayableCard> cards = Singleton<PlayerHand>.Instance.CardsInHand;
            Singleton<ViewManager>.Instance.SwitchToView(View.Hand, lockAfter: true);
            List<PlayableCard> deadCardsHand = new List<PlayableCard>();
            foreach (PlayableCard card in cards)
            {
                    totalHealth2 += card.Health;
            }
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < Math.Round(totalHealth2 * 0.67); i++)
            {
                GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                ball.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.2f, 0.2f, 0.2f));
                int currentRandomSeed = SaveManager.SaveFile.GetCurrentRandomSeed();
                int target = SeededRandom.Range(0, cards.Count, currentRandomSeed + Singleton<TurnManager>.Instance.TurnNumber + totalHealth + i);
                ball.transform.position = cards[target].transform.position;
                ball.transform.Translate(new Vector3(0f, 20f, 0f));
                Tween.Position(ball.transform, cards[target].transform.position, 0.25f, 0, Tween.EaseIn);
                yield return new WaitForSeconds(0.25f);
                Destroy(ball);
                ParticleSystem particleSystem = cards[target].gameObject.GetComponent<ParticleSystem>();
                if (particleSystem == null) particleSystem = cards[target].gameObject.AddComponent<ParticleSystem>();
                particleSystems.Add(particleSystem);
                ParticleSystem.MainModule main = particleSystem.main;
                ParticleSystem.EmissionModule emission = particleSystem.emission;
                ParticleSystem.ShapeModule shape = particleSystem.shape;
                ParticleSystem.SizeOverLifetimeModule sizeOverLifetimeModule = particleSystem.sizeOverLifetime;
                ParticleSystemRenderer renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
                main.duration = 1.0f;
                main.startSpeed = 0.25f;
                main.loop = false;
                main.startLifetime = new ParticleSystem.MinMaxCurve(0.5f, 1.0f);
                main.startSize = new ParticleSystem.MinMaxCurve(1f, 1.5f);
                emission.rateOverTime = 0;
                emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 30) });
                shape.shapeType = ParticleSystemShapeType.Sphere;
                sizeOverLifetimeModule.enabled = true;
                sizeOverLifetimeModule.size = new ParticleSystem.MinMaxCurve(1.0f, new AnimationCurve(new Keyframe[]
                {
                    new Keyframe(0, 0.5f),
                    new Keyframe(0.25f, 1f),
                    new Keyframe(1f, 0),

                }));

                renderer.receiveShadows = false;
                renderer.material.mainTexture = circle;
                renderer.material.color = new Color(0.2f, 0.2f, 0);
                renderer.material.EnableKeyword("_ALPHATEST_ON");
                renderer.material.renderQueue = 2450;

                particleSystem.Play();

                yield return cards[target].TakeDamage(1, null);
                if (cards[target].Health == 0 && !deadCardsHand.Contains(cards[target])) deadCardsHand.Add(cards[target]);
                cards[target].Anim.PlayHitAnimation();
            }
            yield return new WaitForSeconds(1.0f);
            Singleton<ViewManager>.Instance.SwitchToView(View.Hand, lockAfter: true);
            Blueprint = ScriptableObject.CreateInstance<PirateBossP2Blueprint>();
            foreach (PlayableCard card in deadCardsHand)
            {
                card.Slot = Singleton<BoardManager>.Instance.OpponentSlotsCopy[0];
                if (card.HasAbility(Ability.QuadrupleBones)) yield return Singleton<ResourcesManager>.Instance.AddBones(4);
                else yield return Singleton<ResourcesManager>.Instance.AddBones(1);
                yield return card.Die(false);
                Singleton<PlayerHand>.Instance.RemoveCardFromHand(card);
            }
            yield return new WaitForSeconds(0.5f);

            if (deadCardsBoard.Count + deadCardsHand.Count > 0)
            {
                CardInfo cannonball = CardLoader.GetCardByName("PBM_Cannonball");
                for (int i = 0; i < deadCardsBoard.Count + deadCardsHand.Count; i++)
                {
                    yield return Singleton<CardSpawner>.Instance.SpawnCardToHand((CardInfo)cannonball.Clone());
                    yield return new WaitForSeconds(0.05f);
                }
                yield return new WaitForSeconds(0.5f);
                CardInfo cannon = CardLoader.GetCardByName("PBM_Cannon");
                yield return Singleton<CardSpawner>.Instance.SpawnCardToHand((CardInfo)cannon.Clone());
                yield return new WaitForSeconds(1f);
            }

            Singleton<ViewManager>.Instance.SwitchToView(View.Default);

            yield return new WaitForSeconds(1.0f);
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                Destroy(particleSystem);
            }
        }
    }
}
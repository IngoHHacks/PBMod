using DiskCardGame;
using System.Collections.Generic;

namespace PirateBoss.Boss
{
    public class PirateBossP2Blueprint : EncounterBlueprintData
    {
        public void OnEnable()
        {
            name = "PirateBossP2Blueprint";
            turns = new List<List<CardBlueprint>>();
            // TURN 1
            turns.Add(new List<CardBlueprint>()
            {
                new CardBlueprint()
                {
                        card = CardLoader.GetCardByName("Revenant")
                },
                new CardBlueprint()
                {
                        card = CardLoader.GetCardByName("Revenant")
                },
                new CardBlueprint()
                {
                        card = CardLoader.GetCardByName("PBM_Spectral_Skeleton")
                },
            });
            // TURN 2
            turns.Add(new List<CardBlueprint>()
            {
                new CardBlueprint()
                {
                        card = CardLoader.GetCardByName("PBM_Recurring_Skeleton")
                },
                new CardBlueprint()
                {
                        card = CardLoader.GetCardByName("PBM_Ghost_Shark")
                }
            });
            // TURN 3
            turns.Add(new List<CardBlueprint>()
            {
                new CardBlueprint()
                {
                        card = CardLoader.GetCardByName("Bonehound")
                },
                new CardBlueprint()
                {
                        card = CardLoader.GetCardByName("PBM_Spectral_Skeleton")
                }
            });
            // TURN 4
            turns.Add(new List<CardBlueprint>()
            {
                new CardBlueprint()
                {
                        card = CardLoader.GetCardByName("PBM_Ghost_Shark")
                }
            });
            // TURN 5
            turns.Add(new List<CardBlueprint>()
            {
                new CardBlueprint()
                {
                        card = CardLoader.GetCardByName("PBM_Recurring_Skeleton")
                }
            });
            // TURN 6
            turns.Add(new List<CardBlueprint>()
            {
                new CardBlueprint()
                {
                        card = CardLoader.GetCardByName("Bonehound")
                },
                new CardBlueprint()
                {
                        card = CardLoader.GetCardByName("PBM_Spectral_Skeleton")
                }
            });
            // TURN 7
            turns.Add(new List<CardBlueprint>()
            {
                new CardBlueprint()
                {
                        card = CardLoader.GetCardByName("PBM_Ghost_Shark")
                }
            });
            // TURN 8-99
            for (int i = 8; i < 100; i++)
            {
                turns.Add(new List<CardBlueprint>()
                {
                    new CardBlueprint()
                    {
                            card = CardLoader.GetCardByName("Skeleton")
                    },
                });
            }
        }
    }
}
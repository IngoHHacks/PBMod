using DiskCardGame;
using System.Collections.Generic;

namespace PirateBoss.Boss
{
    public class PirateBossP1Blueprint : EncounterBlueprintData
    {
        public PirateBossP1Blueprint()
        {
            name = "PirateBossP1Blueprint1";
            turns = new List<List<CardBlueprint>>();
            // TURN 1
            turns.Add(new List<CardBlueprint>()
            {
                new CardBlueprint()
                {
                        card = CardLoader.GetCardByName("Skeleton")
                },
                new CardBlueprint()
                {
                        card = CardLoader.GetCardByName("Skeleton")
                }
            });
            // TURN 2
            turns.Add(new List<CardBlueprint>()
            {
                new CardBlueprint()
                {
                        card = CardLoader.GetCardByName("Revenant")
                },
            });
            // TURN 3
            turns.Add(new List<CardBlueprint>()
            {
                new CardBlueprint()
                {
                        card = CardLoader.GetCardByName("PBM_Spectral_Skeleton")
                },
                new CardBlueprint()
                {
                        card = CardLoader.GetCardByName("Skeleton")
                }
            });
            // TURN 4
            turns.Add(new List<CardBlueprint>()
            {
                new CardBlueprint()
                {
                        card = CardLoader.GetCardByName("Bonehound")
                }
            });
            // TURN 5
            turns.Add(new List<CardBlueprint>()
            {
                new CardBlueprint()
                {
                        card = CardLoader.GetCardByName("PBM_Recurring_Skeleton")
                },
                new CardBlueprint()
                {
                        card = CardLoader.GetCardByName("PBM_Spectral_Skeleton")
                }
            });
            // TURN 6
            turns.Add(new List<CardBlueprint>()
            {
                new CardBlueprint()
                {
                        card = CardLoader.GetCardByName("PBM_Ghost_Shark")
                }
            });
            // TURN 7
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
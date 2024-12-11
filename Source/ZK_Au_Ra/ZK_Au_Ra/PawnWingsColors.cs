using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace ZK_Au_Ra
{
    public class PawnWingsColors
    {
        public static readonly Color Fallen = new Color(0.2f, 0.2f, 0.2f);
        public static readonly Color Risen = new Color(0.96f, 0.98f, 0.94f);
        private static List<AddonGeneDef> wingsColorGenes;
        public static List<AddonGeneDef> tmpWingsColorGenes = new List<AddonGeneDef>();

        public static List<AddonGeneDef> WingsColorGenesInOrder
        {
            get
            {
                if (PawnWingsColors.wingsColorGenes == null)
                {
                    PawnWingsColors.wingsColorGenes = new List<AddonGeneDef>();
                    foreach (AddonGeneDef allDef in DefDatabase<AddonGeneDef>.AllDefs)
                    {
                        if ((allDef.endogeneCategory == EndogeneCategory.Melanin || (double) allDef.minMelanin < 0.0) && allDef.wingsColorBase.HasValue)
                            PawnWingsColors.wingsColorGenes.Add(allDef);
                    }

                    PawnWingsColors.wingsColorGenes.SortBy<AddonGeneDef, float>(
                        (Func<AddonGeneDef, float>)(x => x.minMelanin));
                }

                return PawnWingsColors.wingsColorGenes;
            }
        }

        public static void ResetStaticData() => PawnWingsColors.wingsColorGenes = (List<AddonGeneDef>) null;

        public static bool IsDarkWings(Color color)
        {
            Color scalesColor = PawnWingsColors.GetWingsColor(0.5f);
            return (double)color.r + (double)color.g + (double)color.b <= (double)scalesColor.r +
                (double)scalesColor.g + (double)scalesColor.b + 0.009999999776482583;
        }

        public static Color GetWingsColor(float melanin)
        {
            return PawnWingsColors.GetWingsColorGene(melanin).wingsColorBase.Value;
        }

        public static AddonGeneDef GetWingsColorGene(float melanin)
        {
            List<AddonGeneDef> colorGenesInOrder = PawnWingsColors.WingsColorGenesInOrder;
            for (int index = colorGenesInOrder.Count - 1; index >= 0; --index)
            {
                if ((double) melanin >= (double) colorGenesInOrder[index].minMelanin)
                    return colorGenesInOrder[index];
            }

            return colorGenesInOrder.RandomElement<AddonGeneDef>();
        }

        public static AddonGeneDef RandomWingsColorGene(Pawn pawn)
        {
            return pawn.Faction != null
                ? PawnWingsColors.GetWingsColorGene(pawn.Faction.def.melaninRange.RandomInRange)
                : PawnWingsColors.WingsColorGenesInOrder.RandomElementByWeight<AddonGeneDef>(
                    (Func<AddonGeneDef, float>)(x => x.selectionWeight));
        }

        public static int WingsColorIndex(Pawn pawn)
        {
            if (pawn?.genes == null)
                return -1;
            GeneDef endogeneByCategory = pawn.genes.GetFirstEndogeneByCategory(EndogeneCategory.Melanin);
            return endogeneByCategory != null ? PawnWingsColors.WingsColorGenesInOrder.IndexOf(null) : -1;
        }

        public static List<AddonGeneDef> WingsColorFromParents(Pawn father, Pawn mother)
        {
            PawnWingsColors.tmpWingsColorGenes.Clear();
            if (father == null && mother == null)
                return PawnWingsColors.tmpWingsColorGenes;
            int a = PawnWingsColors.WingsColorIndex(father);
            int b = PawnWingsColors.WingsColorIndex(mother);
            if (a >= 0 && b >= 0)
            {
                int num1 = Mathf.Min(a, b);
                int num2 = Mathf.Max(a, b);
                for (int index = num1; index <= num2; ++index)
                    PawnWingsColors.tmpWingsColorGenes.Add(PawnWingsColors.WingsColorGenesInOrder[index]);
            }
            return PawnWingsColors.tmpWingsColorGenes;
        }

    }
}
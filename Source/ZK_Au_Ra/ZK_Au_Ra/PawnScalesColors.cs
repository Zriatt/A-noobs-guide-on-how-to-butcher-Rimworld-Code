using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace ZK_Au_Ra
{
    public class PawnScalesColors
    {
        public static readonly Color Xaela = new Color(0.2f, 0.2f, 0.2f);
        public static readonly Color Raen = new Color(0.96f, 0.98f, 0.94f);
        public static readonly Color RaenAlt = new Color(0.85f, 0.87f, 0.8f);
        public static readonly Color XaelaAlt = new Color(0.2f, 0.2f, 0.3f);
        public static readonly Color Graen = new Color(0.52f, 0.45f, 0.5f);
        public static readonly Color GraenAlt = new Color(0.64f, 0.5f, 0.5f);
        private static List<AddonGeneDef> cachedScalesColorGenes;

        private static List<AddonGeneDef> ScalesColorGenes
        {
            get
            {
                if (PawnScalesColors.cachedScalesColorGenes == null)
                    PawnScalesColors.cachedScalesColorGenes = DefDatabase<AddonGeneDef>.AllDefs
                        .Where<AddonGeneDef>((Func<AddonGeneDef, bool>)(x => x.scalesColorOverride.HasValue))
                        .ToList<AddonGeneDef>();
                return PawnScalesColors.cachedScalesColorGenes;
            }
        }

        public static void ResetStaticData()
        {
            PawnScalesColors.cachedScalesColorGenes = (List<AddonGeneDef>) null;
        }

        public static Color RandomScalesColor(Pawn pawn, Color wingsColor, int ageYears)
        {
            if ((double) Rand.Value < 0.0199999999)
                return new Color(Rand.Value, Rand.Value, Rand.Value);
            if (PawnWingsColors.IsDarkWings(wingsColor) || (double)Rand.Value < 0.5)
            {
                float num = Rand.Value;
                if ((double)num < 0.25)
                    return PawnScalesColors.Xaela;
                if ((double)num < 0.5)
                    return PawnScalesColors.XaelaAlt;
                return (double) num < 0.75 ? PawnScalesColors.Graen : PawnScalesColors.GraenAlt;
            }

            float num1 = Rand.Value;
            if ((double)num1 < 0.25)
                return PawnScalesColors.RaenAlt;
            if ((double)num1 < 0.5)
                return PawnScalesColors.RaenAlt;
            return (double) num1 < 0.75 ? PawnScalesColors.Raen : PawnScalesColors.RaenAlt;
        }

        public static AddonGeneDef RandomScalesColorGeneFor(Pawn pawn)
        {
            return PawnScalesColors.RandomScalesColorGene(pawn.story.SkinColorBase,
                ModsConfig.AnomalyActive && pawn.Faction == Faction.OfHoraxCult);
        }

        private static AddonGeneDef RandomScalesColorGene(Color wingsColor, bool cultist = false)
        {
            AddonGeneDef result;
            return PawnScalesColors.ScalesColorGenes.TryRandomElementByWeight<AddonGeneDef>((Func<AddonGeneDef, float>) (
                g =>
                {
                    float selectionWeight = g.selectionWeight;
                    if (PawnWingsColors.IsDarkWings(wingsColor))
                        selectionWeight *= g.selectionWeightFactorDarkSkin;
                    if (cultist)
                        selectionWeight *= g.selectionWeightCultist;
                    return selectionWeight;
                }), out result) ? result : (AddonGeneDef) null;
        }

        public static AddonGeneDef ClosestScalesColorGene(Color scalesColor, Color wingsColor)
        {
            foreach (AddonGeneDef scalesColorGene in PawnScalesColors.ScalesColorGenes)
            {
                if (scalesColor.IndistinguishableFrom(scalesColorGene.scalesColorOverride.Value))
                    return scalesColorGene;
            }

            return PawnScalesColors.RandomScalesColorGene(scalesColor);
        }
    }
}
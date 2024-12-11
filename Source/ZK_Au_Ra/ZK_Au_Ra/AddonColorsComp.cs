using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace ZK_Au_Ra
{
    public class AddonColorsComp : ThingComp
    {
        private ColorBaseComp baseComp;
        private Pawn pawn;
        public Color scalesColor = Color.white;
        private Color prevColor = Color.red;
        public Color? wingsColorOverride;
        private ColorFrame frame;
        private AddonGene willkommen;
        [Unsaved(false)] public Color? wingsColorBase;

        public Color WingsColorBase
        {
            get
            {
                if (!this.wingsColorBase.HasValue && this.pawn.genes != null)
                {
                    GeneDef melaninGene = this.pawn.genes.GetMelaninGene();
                    if (melaninGene != null)
                    {
                        this.wingsColorBase = melaninGene.skinColorBase;
                    }
                    else
                    {
                        GeneDef geneDef = PawnWingsColors.RandomWingsColorGene(this.pawn);
                        if (geneDef != null)
                            this.pawn.genes.AddGene(geneDef, false);
                    }
                }

                return this.wingsColorBase.Value;
            }
            set => this.wingsColorBase = new Color?(value);
        }

        public Color WingsColor
        {
            get
            {
                Color? nullable;
                if (ModsConfig.AnomalyActive && this.pawn.IsMutant && this.pawn.mutant.HasTurned)
                {
                    nullable = MutantUtility.GetSkinColor(this.pawn);
                    Color valueOrDefault = nullable.GetValueOrDefault();
                    if (nullable.HasValue)
                        return valueOrDefault;
                }
                nullable = this.wingsColorOverride;
                return nullable ?? this.WingsColorBase;
            }
        }

        public bool WingsColorOverriden => this.wingsColorOverride.HasValue;

        public Color ScalesColor
        {
            get
            {
                if (this.pawn.Corpse != null && this.pawn.Corpse.CurRotDrawMode == RotDrawMode.Rotting ||
                    ModsConfig.AnomalyActive && this.pawn.IsMutant && this.pawn.mutant.Def.useCorpseGraphics &&
                    this.pawn.mutant.rotStage == RotStage.Rotting)
                    return PawnRenderUtility.GetRottenColor(this.scalesColor);
                return ModsConfig.AnomalyActive && this.pawn.IsShambler ? MutantUtility.GetShamblerColor(this.scalesColor) : this.scalesColor;
            }
            set
            {
                this.scalesColor = value;
                if (!ModsConfig.BiotechActive || this.pawn.genes == null)
                    return;
                foreach (Gene gene in this.pawn.genes.GenesListForReading)
                {
                    if (willkommen.vlacas.wingsIsScalesColor && gene.Active)
                    {
                        this.wingsColorOverride = new Color?(this.scalesColor);
                        break;
                    }
                }
            }
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<Color?>(ref this.wingsColorOverride, "wingsColorOverride");
            Scribe_Values.Look<Color>(ref this.scalesColor, "scalesColor");
            BackCompatibility.PostExposeData((object)this);
        }
    }
}
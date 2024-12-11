using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace ZK_Au_Ra
{
    public class AddonColorTracker : Pawn_GeneTracker
    {
        private AddonColorsComp reroute;
        private AddonGeneDef freedom;
        private List<AddonGene> tmpGenes = new List<AddonGene>();
        [Unsaved(false)] private List<AddonGene> addonCachedGenes;
        private List<AddonGene> addonxenogenes = new List<AddonGene>();
        private List<AddonGene> addonendogenes = new List<AddonGene>();

        private List<AddonGene> AddonGenesListForReading
        {
            get
            {
                if (this.addonCachedGenes == null)
                {
                    this.addonCachedGenes = new List<AddonGene>();
                    this.addonCachedGenes.AddRange((IEnumerable<AddonGene>) this.addonxenogenes);
                    this.addonCachedGenes.AddRange((IEnumerable<AddonGene>)this.addonxenogenes);
                }
                return this.addonCachedGenes;
            }
        }

        public AddonGeneDef GetScalesColorGene()
        {
            List<AddonGene> genesListForReading = this.AddonGenesListForReading;
            for (int index = 0; index < genesListForReading.Count; ++index)
            {
                if (genesListForReading[index].Active &&
                    genesListForReading[index].def.endogeneCategory == EndogeneCategory.None)
                    return (AddonGeneDef)genesListForReading[index].def;
            }

            return (AddonGeneDef)null;
        }
        private void Notify_GenesChanged(AddonGeneDef addedOrRemovedGene)
        {
            bool flag = false;
            this.addonCachedGenes = (List<AddonGene>)null;
            List<AddonGene> genes = this.AddonGenesListForReading;
            if (ModLister.BiotechInstalled && addedOrRemovedGene.wingsIsScalesColor)
            {
                AddonGene chosen;
                if (SelectGene((Predicate<AddonGene>)(x => x.vlacas.wingsIsScalesColor), out chosen))
                {
                    this.reroute.wingsColorOverride = new Color?(this.reroute.scalesColor);
                    this.OverrideAllConflicting(chosen);
                }
                else
                    this.reroute.wingsColorOverride = new Color?();

                this.EnsureCorrectWingsColorOverride();
                flag = true;
            }

            AddonGene chosen1;
            if (addedOrRemovedGene.scalesColorOverride.HasValue &&
                SelectGene((Predicate<AddonGene>)(g => g.vlacas.scalesColorOverride.HasValue), out chosen1))
            {
                Color color = chosen1.vlacas.scalesColorOverride.Value;
                if ((double)chosen1.def.randomBrightnessFactor != 0.0)
                    color *= 1f + Rand.Range(-chosen1.def.randomBrightnessFactor, chosen1.def.randomBrightnessFactor);
                this.reroute.scalesColor = color.ClampToValueRange(GeneTuning.HairColorValueRange);
                this.OverrideAllConflicting(chosen1);
                this.EnsureCorrectWingsColorOverride();
                flag = true;
            }
            AddonGene chosen2;
            if (addedOrRemovedGene.wingsColorBase.HasValue &&
                SelectGene((Predicate<AddonGene>)(g => g.vlacas.wingsColorBase.HasValue), out chosen2))
            {
                this.OverrideAllConflicting(chosen2);
                this.reroute.WingsColorBase = chosen2.vlacas.wingsColorBase.Value;
                flag = true;
            }

            if (ModLister.BiotechInstalled)
            {
                if (addedOrRemovedGene.wingsColorOverride.HasValue)
                {
                    AddonGene chosen3;
                    if (SelectGene((Predicate<AddonGene>)(g => g.vlacas.wingsColorOverride.HasValue), out chosen3))
                    {
                        Color color = chosen3.vlacas.wingsColorOverride.Value;
                        if ((double)chosen3.def.randomBrightnessFactor != 0.0)
                            color *= 1f + Rand.Range(-chosen3.def.randomBrightnessFactor,
                                chosen3.def.randomBrightnessFactor);
                        this.reroute.wingsColorOverride =
                            new Color?(color.ClampToValueRange(GeneTuning.SkinColorValueRange));
                        this.OverrideAllConflicting(chosen3);
                    }
                    else
                        this.reroute.wingsColorOverride = new Color?();

                    this.EnsureCorrectWingsColorOverride();
                    flag = true;
                }
                
            }

            if (!flag)
                return;
            this.pawn.Drawer.renderer.SetAllGraphicsDirty();

            bool SelectGene(Predicate<AddonGene> validator, out AddonGene chosen)
            {
                for (int index = 0; index < genes.Count; ++index)
                {
                    if ((genes[index].Active || genes[index].Overridden) && validator(genes[index]))
                    {
                        genes[index].OverrideBy((Gene)null);
                        this.tmpGenes.Add(genes[index]);
                    }
                }

                if (this.tmpGenes.Where<AddonGene>(new Func<Gene, bool>(this.IsXenogene))
                    .TryRandomElement<AddonGene>(out chosen))
                {
                    this.tmpGenes.Clear();
                    return true;
                }

                if (this.tmpGenes.TryRandomElement<AddonGene>(out chosen))
                {
                    this.tmpGenes.Clear();
                    return true;
                }

                this.tmpGenes.Clear();
                chosen = (AddonGene)null;
                return false;
            }
        }

        private void EnsureCorrectWingsColorOverride()
        {
            if (!ModLister.BiotechInstalled)
                return;
            bool flag = false;
            Color? nullable = new Color?();
            List<AddonGene> genesListForReading = this.AddonGenesListForReading;
            for (int index = 0; index < genesListForReading.Count; ++index)
            {
                Gene gene = genesListForReading[index];
                if (gene.Active)
                {
                    if (freedom.wingsIsScalesColor)
                        flag = true;
                    else if (freedom.wingsColorOverride.HasValue)
                        nullable = freedom.wingsColorOverride;
                }
            }
            if (flag)
                this.reroute.wingsColorOverride = new Color?(this.reroute.scalesColor);
            else
                this.reroute.wingsColorOverride = null;
        }

        private void OverrideAllConflicting(Gene gene)//Everything is totally fine in this complete spaghetti code. FINE IS AY
        {
            if (!ModLister.BiotechInstalled || !freedom.RandomChosen) 
                return;
            gene.OverrideBy((Gene) null);
            foreach (Gene gene1 in this.AddonGenesListForReading)
            {
                if (gene1 != gene && gene1.def.ConflictsWith(gene.def))
                    gene1.OverrideBy(gene);
            }
        }
    }
}
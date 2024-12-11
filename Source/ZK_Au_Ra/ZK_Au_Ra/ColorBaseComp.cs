using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ZK_Au_Ra
{
    public class ColorBaseComp : ThingComp

    {
    public AddonColorsComp addonColors;
    private Pawn pawn;

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Deep.Look<AddonColorsComp>(ref this.addonColors, "addonColors", (object)this);
        BackCompatibility.PostExposeData((object)this);
    }
    }
}
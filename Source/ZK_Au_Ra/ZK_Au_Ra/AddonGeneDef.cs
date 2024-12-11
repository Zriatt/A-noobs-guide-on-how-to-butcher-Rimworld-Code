using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ZK_Au_Ra
{
    public class AddonGeneDef : GeneDef
    {
        private Color? iconColorAddon;
        public List<PawnRenderNodeProperties_Addon> renderNodeProperties;
        public bool wingsIsScalesColor;
        public bool randomChosen;
        public Color? wingsColorBase;
        public Color? scalesColorOverride;
        public Color? wingsColorOverride;


        public bool RandomChosen
        {
            get
            {
                if (this.randomChosen)
                    return true;
                if (this.biostatArc > 0 || this.biostatCpx != 0 || this.biostatMet != 0)
                    return false;
                return this.scalesColorOverride.HasValue || this.wingsColorBase.HasValue ||
                       this.wingsColorOverride.HasValue;
            }
        }
        
    }
}
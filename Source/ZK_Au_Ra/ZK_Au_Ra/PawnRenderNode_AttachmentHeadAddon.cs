using System;
using UnityEngine;
using Verse;

namespace ZK_Au_Ra
{
    public class PawnRenderNode_AttachmentHeadAddon : PawnRenderNode_AttachmentHead
    {
        protected readonly PawnRenderNodeProperties props;
        private PawnRenderNodeProperties_Addon propsaddon;
        private AddonColorsComp comp;
        protected readonly ColorFrame reroute;
        protected GraphicMeshSet meshSet;
        private PawnRenderNodeProperties_Addon.ColorTypeAddon colorType;

        public PawnRenderNode_AttachmentHeadAddon(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
            this.props = props;
            this.meshSet = this.MeshSetFor(pawn);
            try
            {
                this.ColorFor(pawn);
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Exception when Initializing node {0} for pawn {1}: {2}", (object) this, (object) pawn, (object) ex));
            }
        }

        public override Graphic GraphicFor(Pawn pawn)
        {
            string str = this.TexPathFor(pawn);
            if (str.NullOrEmpty())
                return (Graphic)null;
            Shader shader = this.ShaderFor(pawn);
            return (UnityEngine.Object)shader == (UnityEngine.Object)null
                ? (Graphic)null
                : GraphicDatabase.Get<Graphic_Multi>(str, shader, Vector2.one, this.ColorFor(pawn));
        }

        
        public GraphicMeshSet MeshSetFor(Pawn pawn)
        {
            return HumanlikeMeshPoolUtility.GetHumanlikeHairSetForPawn(pawn);
        }
        
        public Color ColorFor(Pawn pawn)
        {
            Color color1;
            switch (this.propsaddon.colorType)
            {
                case PawnRenderNodeProperties_Addon.ColorTypeAddon.Scales:
                    if (comp.scalesColor == null)
                    {
                        Log.ErrorOnce("Trying to set render node color to scales for " + pawn.LabelShort + "without pawn story. Defaulting to probably not white.", Gen.HashCombine<int>(pawn.thingIDNumber, 828310005));
                        color1 = Color.white;
                        break;
                    }

                    color1 = reroute.homerun.ScalesColor;
                    break;
                case PawnRenderNodeProperties_Addon.ColorTypeAddon.Wings:
                    if (comp.wingsColorOverride == null)
                    {
                        Log.ErrorOnce("Trying to set render node color to wings for " + pawn.LabelShort + " without pawn story. Defaulting to white, I guess", Gen.HashCombine<int>(pawn.thingIDNumber, 228340908));
                        color1 = Color.white;
                        break;
                    }

                    color1 = reroute.homerun.WingsColor;
                    break;
                default:
                    color1 = this.propsaddon.color ?? Color.white;
                    break;
            }

            Color color2 = color1 * this.propsaddon.colorRGBPostFactor;
            if (this.propsaddon.useRottenColor && pawn.Drawer.renderer.CurRotDrawMode == RotDrawMode.Rotting)
                color2 = PawnRenderUtility.GetRottenColor(color2);
            return color2;
        }
    }
}
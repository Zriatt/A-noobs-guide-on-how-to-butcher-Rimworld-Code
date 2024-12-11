using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ZK_Au_Ra
{
    public class PawnRenderNodeProperties_Addon : PawnRenderNodeProperties
    {
        public new ColorTypeAddon colorType;
        public ColorFrame reroute;
        public Color? color;
        public float colorRGBPostFactor = 1f;

        public enum ColorTypeAddon
        {
            Scales,
            Wings,
        }
    }
}
/*
* (C) Copyright 2014, Valerian Gaudeau
* 
* Kerbal Space Program is Copyright (C) 2013 Squad. See http://kerbalspaceprogram.com/. This
* project is in no way associated with nor endorsed by Squad.
* 
* This code is licensed under the Apache License Version 2.0. See the LICENSE.txt
* file for more information.
*/

using System.Collections.Generic;
using System.Collections;
using System;
using System.IO;
using UnityEngine;

namespace PlanetShine
{
    public class Utils
    {
        public static LineRenderer CreateDebugLine(Color startColor, Color endColor)
        {
            GameObject obj = new GameObject ("Line");
            LineRenderer line = obj.AddComponent< LineRenderer > ();
            line.material = new Material (Shader.Find ("Particles/Additive"));
            line.endColor = endColor;
            line.startColor = startColor;
            line.startWidth = 0.05f;
            line.endWidth = 0.05f;
            line.positionCount = 2;
            return line;
        }

        public static Color GetUnreadableTextureAverageColor(Texture2D texture)
        {
            Texture2D readableTexture = CreateReadable(texture);
            Color color = GetTextureAverageColor(readableTexture);
            UnityEngine.Object.Destroy(readableTexture);
            return color;
        }

        public static Color GetPixelsAverageColor(Color[] texColors)
        {
            int total = texColors.Length;

            float r = 0;
            float g = 0;
            float b = 0;

            foreach (Color pixel in texColors)
            {
                r += pixel.r;
                g += pixel.g;
                b += pixel.b;
            }
            return new Color(r / total, g / total, b / total, 1.0f);
        }

        public static Color GetTextureAverageColor(Texture2D texture)
        {
            return GetPixelsAverageColor(texture.GetPixels());
        }

        public static Color GetRimOuterColor(Texture2D texture, float fraction)
        {
            return GetPixelsAverageColor(texture.GetPixels(0, 0, (int) Math.Round(texture.width * fraction), texture.height));
        }

        public static Texture2D CreateReadable(Texture2D original)
        {
            // Checks
            if (original == null) return null;
            if (original.width == 0 || original.height == 0) return null;

            // Create the new texture
            Texture2D finalTexture = new Texture2D(original.width, original.height);

            // isn't read or writeable ... we'll have to get tricksy
            RenderTexture rt = RenderTexture.GetTemporary(original.width, original.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB, 1);
            Graphics.Blit(original, rt);
            RenderTexture.active = rt;

            // Load new texture
            finalTexture.ReadPixels(new Rect(0, 0, finalTexture.width, finalTexture.height), 0, 0);

            // Kill the old one
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);

            // Return
            return finalTexture;
        }
    }


    public class DisplaySettingOption<T>
    {
        public string label { get; private set; }
        public T value { get; private set; }

        public DisplaySettingOption(string label, T value)
        {
            this.label = label;
            this.value = value;
        }
    }


}


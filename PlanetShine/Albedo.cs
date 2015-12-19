using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlanetShine
{
    public class Albedo
    {
        public AlbedoCamera localCamera;
        public AlbedoCamera scaledCamera;
        //public AlbedoCamera atmosphereCamera;
        public Texture2D combinedTexture;

        public CelestialBody targetBody;
        public int pixelSize;
        public bool active;
        public float elevation = 0f;

        public Albedo(int pixelSize)
        {
            active = false;
            this.pixelSize = pixelSize;
            combinedTexture = new Texture2D(pixelSize, pixelSize, TextureFormat.RGB24, false);
            localCamera = new AlbedoCamera(false, LayerMask.LocalScenery, pixelSize);
            scaledCamera = new AlbedoCamera(true, LayerMask.ScaledScenery, pixelSize);
        }

        public void Activate(CelestialBody target)
        {
            localCamera.Activate(target);
            scaledCamera.Activate(target);
            targetBody = target;
            active = true;
        }

        public void Deactivate()
        {
            localCamera.Deactivate();
            scaledCamera.Deactivate();
            targetBody = null;
            active = false;
        }

        public bool Update()
        {
            if (!active)
                return false;

            /*
            position = FlightGlobals.ActiveVessel.transform.position;
            position += (position - targetBody.transform.position).normalized * elevation;
            float distance = (float) (position - targetBody.position).magnitude;
            fov = 2 * Mathf.Rad2Deg * Mathf.Acos(Mathf.Sqrt(Mathf.Max((distance * distance) - (float)(targetBody.Radius * targetBody.Radius), 1)) / distance);
            float nearClipPlane = Mathf.Max(0.001f, distance - (((float)targetBody.Radius - (float)targetBody.atmosphereDepth) * 1.5f));
            float farClipPlane = distance + ((float)targetBody.Radius);
            */

            //localCamera.Update(position, nearClipPlane, farClipPlane);
            //scaledCamera.Update(position, nearClipPlane, farClipPlane);

            localCamera.Update(targetBody, elevation);
            scaledCamera.Update(targetBody, elevation);
            //localCamera.camera.fieldOfView = fov;
            //scaledCamera.camera.fieldOfView = fov;
            return true;
        }

        public bool Render()
        {
            if (!active)
                return false;
            localCamera.Render();
            scaledCamera.Render();
            return true;
        }

        public void Combine()
        {
            Color pixelLocal = Color.white;
            Color pixelScaled = Color.white;

            for (int i = 0; i < 128; i++)
                for (int j = 0; j < 128; j++)
                {
                    /*pixel = localCamera.texture.GetPixel(i, j);
                    pixel = Color.Lerp(pixel, scaledCamera.texture.GetPixel(i, j), pixel.a * 2.5f);*/
                    pixelLocal = localCamera.texture.GetPixel(i, j);
                    pixelScaled = scaledCamera.texture.GetPixel(i, j);
                    if (pixelLocal.Intensity() > pixelScaled.Intensity())
                        combinedTexture.SetPixel(i, j, pixelLocal);
                    else
                        combinedTexture.SetPixel(i, j, pixelScaled);
                }
            combinedTexture.Apply();
        }

        public Color ExtractColor(float ratio = 1f)
        {
            int size = (int)(pixelSize * ratio);
            int position = (int)((pixelSize / 2f) * (1f - ratio));
            return combinedTexture.GetAverageColorSlow(position, position, size, size);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NoiseGen : MonoBehaviour
{
    [MenuItem("Noise/Noise Generation/Generate Noise Value")]
    static void CreateNoiseValue() {
        print(Perlin.Noise(0.4f, 0.4f));
    }

    [MenuItem("Noise/Noise Generation/Generate Noise Texture")]
    static void CreateTexture3D()
    {
        // Configure the texture
        int size = 32;
        TextureFormat format = TextureFormat.RGBA32;
        TextureWrapMode wrapMode =  TextureWrapMode.Clamp;

        // Create the texture and apply the configuration
        Texture3D texture = new Texture3D(size, size, size, format, false);
        texture.wrapMode = wrapMode;

        // Create a 3-dimensional array to store color data
        Color[] colors = GenerateNoise(size, 5);

        // Copy the color values to the texture
        texture.SetPixels(colors);
        texture.Apply();        
        string noise = System.DateTime.Now.ToFileTime().ToString();
        AssetDatabase.CreateAsset(texture, "Assets/_Main/Sean/Noise/" + noise + ".asset");
    }

    [MenuItem("Noise/Noise Generation/Generate Hash Volume")]
    static void CreateHashVolume()
    {
        // Configure the texture
        int size = 128;
        TextureFormat format = TextureFormat.RGBA32;
        TextureWrapMode wrapMode =  TextureWrapMode.Clamp;

        // Create the texture and apply the configuration
        Texture3D texture = new Texture3D(size, size, size, format, false);
        texture.wrapMode = wrapMode;

        // Create a 3-dimensional array to store color data
        Color[] colors = GenerateVolume(size);

        // Copy the color values to the texture
        texture.SetPixels(colors);
        texture.Apply();        
        string noise = System.DateTime.Now.ToFileTime().ToString();
        AssetDatabase.CreateAsset(texture, "Assets/_Main/Sean/Noise/Hash_Volume" + noise + ".asset");
    }

    public static Color[] GenerateVolume (int size)
    {
        var voxels = new Color[size*size*size];
        int i = 0;
        Color color = Color.black;
        for (int z = 0; z < size; ++z)
        {
            for (int y = 0; y < size; ++y)
            {
                for (int x = 0; x < size; ++x, ++i)
                {
                    float rand = Perlin.Noise(x / size, y / size, z / size);
                    color.r = rand;
                    color.g = rand;
                    color.b = rand;
                    voxels[i] = color;
                    print(color);
                }
            }
        }
        return voxels;
    }

    [MenuItem("Noise/Noise Generation/Generate 3D Worley Noise")]
    static void Create3DWorleyNoise()
    {
        // Configure the texture
        int size = 128;
        TextureFormat format = TextureFormat.RGBA32;
        TextureWrapMode wrapMode =  TextureWrapMode.Clamp;

        // Create the texture and apply the configuration
        Texture3D texture = new Texture3D(size, size, size, format, false);
        texture.wrapMode = wrapMode;

        // Create a 3-dimensional array to store color data
         Color[] colors = GenerateWorleyNoiseCube(size, 50);

        // Copy the color values to the texture
        texture.SetPixels(colors);
        texture.Apply();        
        string noise = System.DateTime.Now.ToFileTime().ToString();
        AssetDatabase.CreateAsset(texture, "Assets/_Main/Sean/Noise/3DWorley_" + noise + ".asset");
    }

    [MenuItem("Noise/Noise Generation/Generate 2D Worley Noise")]
    static void CreateWorleyNoise()
    {
        // Configure the texture
        int size = 128;
        TextureFormat format = TextureFormat.RGBA32;
        TextureWrapMode wrapMode =  TextureWrapMode.Clamp;

        // Create the texture and apply the configuration
        Texture2D texture = new Texture2D(size, size, format, false);
        texture.wrapMode = wrapMode;

        // Create a 3-dimensional array to store color data
        Color[] colors = GenerateWorleyNoiseFrame(size, 50);
        // Copy the color values to the texture
        texture.SetPixels(colors);
        texture.Apply();        
        string noise = System.DateTime.Now.ToFileTime().ToString();
        AssetDatabase.CreateAsset(texture, "Assets/_Main/Sean/Noise/Worley_" + noise + ".asset");
    }

    public static Color[] GenerateWorleyNoiseFrame(int resolution, int seeds) {
        Color[] outputImage = new Color[resolution * resolution];
        int[] seedX = new int[seeds];
        int[] seedY = new int[seeds];

        for(int i = 0; i < seeds; i++) {
            seedX[i] = (int)UnityEngine.Random.Range(0, resolution-1);
            seedY[i] = (int)UnityEngine.Random.Range(0, resolution-1);
        }

        float maxDist = 0.1f;
        float[] distances = new float[seeds];
        for(int my = 0; my < resolution; my++) {
            for(int mx = 0; mx < resolution; mx++) {
                for(int i = 0; i < seeds; i++) {
                    distances[i] = Vector2.Distance(new Vector2(seedX[i], seedY[i]), new Vector2(mx, my));
                    Array.Sort(distances);
                    if(distances[0] > maxDist) {
                        maxDist = distances[0];
                    }
                    float val = 1.0f - (distances[0] / maxDist);
                    outputImage[mx + (my * resolution)] = new Color(val, val, val, 1.0f);
                }
            }
        } 
        return outputImage; 
    }

    // WARNING: This will probably take a long time to compute, only do this when necessary
    public static Color[] GenerateWorleyNoiseCube(int resolution, int seeds) {
        Color[] outputImage = new Color[resolution * resolution * resolution];
        int i = 0;
        for(int z = 0; z < resolution; z++) {
            Color[] generatedNoiseFrame = GenerateWorleyNoiseFrame(resolution, seeds);
            for(int y = 0; y < resolution; y++) {
                for(int x = 0; x < resolution; x++) {
                    outputImage[i] = generatedNoiseFrame[i];
                }
            }
        }
        return outputImage;
    }

    static Color[] GenerateNoise(int resolution, int octaves) {
        Color[] outputNoise = new Color[resolution * resolution * resolution];
        float inv = 1f / 255.0f;
         for (int z = 0; z < resolution; z++)
        {
            int zOffset = z * resolution * resolution;
            for (int y = 0; y < resolution; y++)
            {
                int yOffset = y * resolution;
                for (int x = 0; x < resolution; x++)
                {
                    float noiseValue = UnityEngine.Random.Range(0, 1);
                    noiseValue *= inv;
                    outputNoise[x + yOffset + zOffset] = new Color(noiseValue, noiseValue, noiseValue, noiseValue);
                }
            }
        }
        return outputNoise;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float GenerateNoiseMap(int mapWidth, int mapHeight, int mapDepth, int seed, float scale, int octaves, float persistance, float lacunarity, Vector3 offset)
    {
        float noiseMap = 0;
        System.Random prng = new System.Random(seed);
        Vector3[] octaveOffset = new Vector3[octaves];
        for(int i = 0; i < octaves; i++){
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            float offsetZ = prng.Next(-100000, 100000) + offset.z;
            octaveOffset[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0) { scale = 0.0001f; }
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;
        float halfDepth = mapDepth / 2f;

        for(int z = 0; z < mapDepth + 1; z++) {
            for(int y = 0; y < mapHeight + 1; y++) {
                for(int x = 0; x < mapWidth + 1; x++) {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;
                    for(int i = 0; i < octaves; i++){
                        float sampleX = (x - halfWidth) / scale * frequency + octaveOffset[i].x;
                        float sampleY = (y - halfHeight) / scale * frequency + octaveOffset[i].y;
                        float sampleZ = (z - halfHeight) / scale * frequency + octaveOffset[i].z;

                        float perlinValue = Perlin3d(sampleX, sampleY, sampleZ) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }
                    if (noiseHeight > maxNoiseHeight){
                        maxNoiseHeight = noiseHeight;
                    }
                    else if (noiseHeight < minNoiseHeight){
                        minNoiseHeight = noiseHeight;
                    }
                    noiseMap = noiseHeight;
                }
            }
        }

        for(int z = 0; z < mapDepth + 1; z++) {   
            for(int y = 0; y < mapHeight + 1; y++){
                for(int x = 0; x < mapWidth + 1; x++){
                    noiseMap = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap);
                }
            }
        }

        return noiseMap;
    }

    public static float Perlin3d(float x, float y, float z) {
	    float AB = Mathf.PerlinNoise(x, y);
	    float BC = Mathf.PerlinNoise(y, z);
		float AC = Mathf.PerlinNoise(x, z);
		float BA = Mathf.PerlinNoise(y, x);
		float CB = Mathf.PerlinNoise(z, y);
		float CA = Mathf.PerlinNoise(z, x);
		float ABC = AB + BC + AC + BA + CB + CA;
		return ABC / 6f;
	}
}

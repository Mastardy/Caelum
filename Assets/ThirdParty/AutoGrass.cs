using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AutoGrass : MonoBehaviour
{
    public bool Place;
    public Terrain[] terrains;

    [SerializeField] private int myTextureLayer = 1; //Assumed to be the first non-base texture layer - this is the ground texture from which we wish to sprout grass
    [SerializeField] private int myDetailLayer = 0; //Assumed to be the first detail layer - this is the grass we wish to auto-populate
    public int seed;

    private void Update()
    {
        if (Place)
        {
            Place = false;
            for(int i = 0; i < terrains.Length; i++)
            {
                PlaceGrass(terrains[i]);
            }
        }
    }

    // Use this for initialization
    private void PlaceGrass(Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;

        // get the alhpa maps - i.e. all the ground texture layers
        float[,,] alphaMapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        //get the detail map for the grass layer we're after
        int[,] map = terrainData.GetDetailLayer(0, 0, terrainData.detailWidth, terrainData.detailHeight, myDetailLayer);

        //now copy-paste the alpha map onto the detail map, pixel by pixel
        for (int x = 0; x < terrainData.alphamapWidth; x++)
        {
            for (int y = 0; y < terrainData.alphamapHeight; y++)
            {
                //Check the Detail Resolution and the Control Texture Resolution in the terrain settings.
                //By default the detail resolution is twice the alpha resolution! So every detail co-ordinate is going to have to affect a 2x2 square!
                //Would be nice if I could so some anti aliasing but this will have to do for now
                int x1 = x * 2;
                int x2 = (x * 2) + 1;
                int y1 = y * 2;
                int y2 = (y * 2) + 1;
                map[x1, y1] = (int)alphaMapData[x, y, myTextureLayer] * 10;
                map[x1, y2] = (int)alphaMapData[x, y, myTextureLayer] * 10;
                map[x2, y1] = (int)alphaMapData[x, y, myTextureLayer] * 10;
                map[x2, y2] = (int)alphaMapData[x, y, myTextureLayer] * 10;
                //if the resolution was the same we could just do the following instead: map [x, y] = (int)alphaMapData [x, y, myTextureLayer] * 10;
            }
        }
        terrainData.SetDetailLayer(0, 0, myDetailLayer, map);
    }
}

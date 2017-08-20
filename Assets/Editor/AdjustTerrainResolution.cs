using UnityEditor;
using UnityEngine;

internal class AdjustTerrainResolution : ScriptableWizard
{
    //private static TerrainData _terrainData;
    public Terrain sourceTerrain;
    public Terrain targetTerrain;
    public int newResolutionX;
    public int newResolutionY;

    [MenuItem("Terrain/Adjust Terrain Resolution")]
    public static void CreateWizard()
    {
        string buttonText = "Cancel";
        //_terrainData = null;

        //Terrain terrainObject = Selection.activeGameObject.GetComponent<Terrain>() ?? Terrain.activeTerrain;
        //Terrain terrainObject = Selection.activeObject as Terrain ?? Terrain.activeTerrain;

        //if (terrainObject)
        //{
        //    _terrainData = terrainObject.terrainData;
            buttonText = "Adjust Terrain Resolution";
        //}

        DisplayWizard<AdjustTerrainResolution>("Adjust Terrain Resolution", buttonText);
    }

    private void OnWizardUpdate()
    {
        //if (!_terrainData)
        //{
        //    helpString = "No terrain found";
        //    return;
        //}

        //HeightAdjustment = Mathf.Clamp(HeightAdjustment, -1.0f, 1.0f);
        //helpString = (_terrainData.size.y * HeightAdjustment) + " meters (" + (HeightAdjustment * 100.0) + "%)";
        helpString = "Select all data";
    }

    private void OnWizardCreate()
    {
        //if (!_terrainData) return;

        Undo.RegisterCompleteObjectUndo(targetTerrain.terrainData, "Adjust Terrain Resolution");

        TerrainData sourceData = sourceTerrain.terrainData;
        float[,] sourceHeights = sourceData.GetHeights(0, 0, sourceData.heightmapWidth, sourceData.heightmapHeight);
        float[,] newHeights = new float[newResolutionX, newResolutionY];

        for (int y = 0; y < newResolutionY; y++)
        {
            for (int x = 0; x < newResolutionX; x++)
            {
                newHeights[y, x] = sourceHeights[
                    Mathf.FloorToInt((float)y * (float)sourceData.heightmapHeight / (float)newResolutionY), 
                    Mathf.FloorToInt((float)x * (float)sourceData.heightmapWidth / (float)newResolutionX)
                    ];
            }
        }
        int kernelRadius = 5;
        float[,] kernel = GenerateGuassianKernel(2 * kernelRadius + 1, 10);
        

        for (int y = kernelRadius; y < newResolutionY - kernelRadius; y++)
        {
            for (int x = kernelRadius; x < newResolutionX - kernelRadius; x++)
            {
                float valueSum = 0.0f;
                //float weightSum = 0.0f;
                for (int i = -kernelRadius; i <= kernelRadius; i++)
                {
                    for (int j = -kernelRadius; j <= kernelRadius; j++)
                    {
                        float weight = kernel[i+kernelRadius, j+kernelRadius];
                        //weightSum += weight;
                        valueSum += weight * newHeights[y + i, x + j];
                    }
                }
                newHeights[y, x] = valueSum;
            }
        }

        targetTerrain.terrainData.SetHeights(0, 0, newHeights);
        //_terrainData = null;
    }

    public static float[,] GenerateGuassianKernel(int length, float weight)
    {
        float[,] Kernel = new float[length, length];
        float sumTotal = 0;


        int kernelRadius = length / 2;
        float distance = 0;


        float calculatedEuler = 1.0f /
        (2.0f * Mathf.PI * Mathf.Pow(weight, 2));


        for (int filterY = -kernelRadius;
             filterY <= kernelRadius; filterY++)
        {
            for (int filterX = -kernelRadius;
                filterX <= kernelRadius; filterX++)
            {
                distance = ((filterX * filterX) +
                           (filterY * filterY)) /
                           (2 * (weight * weight));


                Kernel[filterY + kernelRadius,
                       filterX + kernelRadius] =
                       calculatedEuler * Mathf.Exp(-distance);


                sumTotal += Kernel[filterY + kernelRadius,
                                   filterX + kernelRadius];
            }
        }


        for (int y = 0; y < length; y++)
        {
            for (int x = 0; x < length; x++)
            {
                Kernel[y, x] = Kernel[y, x] *
                               (1.0f / sumTotal);
            }
        }


        return Kernel;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public PlayerController player;
    public GameObject prefab;
    public Terrain terrain;

    public float yOffset = 0.5f;

    private float terrainWidth;
    private float terrainLength;

    private float xTerrainPos;
    private float zTerrainPos;


    void Start()
    {
        terrainWidth = terrain.terrainData.size.x;
        terrainLength = terrain.terrainData.size.z;

        xTerrainPos = terrain.transform.position.x;
        zTerrainPos = terrain.transform.position.z;

        PlaceNewChestOnTerrain();
    }

    public void PlaceNewChestOnTerrain()
    {
        Vector3 chestPosition = RandomPositionOnTerrain(yOffset);
        GameObject objInstance = (GameObject)Instantiate(prefab, chestPosition, Quaternion.identity);

        Debug.Log(chestPosition);
        Debug.Log(player.ChestPositionToMapCoords(chestPosition));
        player.targetPosition = player.ChestPositionToMapCoords(chestPosition);
    }

    public Vector3 RandomPositionOnTerrain(float heightOffset){
        float randX = UnityEngine.Random.Range(xTerrainPos, xTerrainPos + terrainWidth);
        float randZ = UnityEngine.Random.Range(zTerrainPos, zTerrainPos + terrainLength);
        float yVal = Terrain.activeTerrain.SampleHeight(new Vector3(randX, 0, randZ));
        yVal = yVal + heightOffset;

        return new Vector3(randX, yVal, randZ);
    }
}

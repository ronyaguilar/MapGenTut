using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapGenerator : MonoBehaviour {

	public int width;
	public int height;

	// Seeds are used to generate map, if same seed is used, same map is generated
	public string seed;
	public bool useRandomSeed;

	[Range(0,100)] // constrains int from 0 - 100
	public int randomFillPercent;

	int[,] map;
	public int smoothness = 5;
	public int neighborConstraint = 4;

	void Start(){
		GenerateMap ();
	}
	void Update(){
		if(Input.GetMouseButtonDown(0)){
			GenerateMap ();
		}
	}

	void GenerateMap(){
		map = new int[width, height];
		RandomFillMap ();

		for(int i = 0; i < smoothness; i ++){
			SmoothMap ();
		}

		MeshGenerator meshGen = GetComponent<MeshGenerator> ();
		meshGen.GenerateMesh (map, 1);
	}

	void RandomFillMap() {
		if(useRandomSeed) {
			// Causes random seed to always be different
			seed = Time.realtimeSinceStartup.ToString();
		}

		System.Random psuedoRandom = new System.Random (seed.GetHashCode ()); // Converts the seed string to an int
		for(int x = 0; x < width; x++){
			for(int y = 0; y < height; y++){
				if(x == 0 || x == width-1 || y == 0 || y == height - 1){
					map [x, y] = 1; // cell on the walls must always be filled
				}else{
					map [x, y] = (psuedoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0; // iterate through each cell in map and either assign value of 1 or 0 to know if its a wall. 
				}

			}
		}
	}

	void SmoothMap(){
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				int neighborWallTiles = GetNeighborWallCount (x, y);

				if (neighborWallTiles > neighborConstraint) {
					map [x, y] = 1;
				} else if (neighborWallTiles < neighborConstraint) {
					map [x, y] = 0;
				}
			}
		}
	}

	// checks all surrounding cells of a given cell and counts how many of those surrounding cells is a wall
	int GetNeighborWallCount(int gridX, int gridY){
		int wallCount = 0;
		for(int neighborX = gridX - 1; neighborX <= gridX + 1; neighborX++){
				
			for(int neighborY = gridY - 1; neighborY <= gridY + 1; neighborY++){
				if(neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height){ //checks to see if neighbor is within bounds
					if(neighborX != gridX || neighborY != gridY){
						wallCount += map [neighborX, neighborY]; // if its not the given cell add its value, wall count will increment if its a wall
					}
				}else { //if not, increment anyways to encourage walls on boundaries
					wallCount++;
				}
			}
		}
		return wallCount;
	}
	/*
	void OnDrawGizmos(){
		if(map != null){
			for(int x = 0; x < width; x++){
				for(int y = 0; y< height; y++){
					Gizmos.color = (map [x, y] == 1) ? Color.black : Color.white;
					Vector3 pos = new Vector3 (-width / 2 + x + 0.5f, 0, -height / 2 + y + 0.5f);
					Gizmos.DrawCube (pos, Vector3.one);
				}
			}
		}
	}
	*/
}

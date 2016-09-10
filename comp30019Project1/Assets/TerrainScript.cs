using UnityEngine;
using System;

public class TerrainScript : MonoBehaviour {

	public int dimension;
	private int size;
	public int seed;
	public float rangeOfNoise; 
//	public float[] rangeOfNoiseList;
	private float[,] heightMap;
	private Vector3[] vertices;
	private int scale = 10;
	// Use this for initialization
	void Start () {

		dimension = 7;
		size = (int)(int)Math.Pow(2,dimension)+1;
		seed = 500;
		rangeOfNoise = 500.0f;
//		rangeOfNoiseList = new float[]{30.0f,15.0f,10.0f,7.0f,5.0f,4.0f};
		heightMap = new float[size, size];
		vertices = new Vector3[size * size];

		MeshFilter terrainMesh = GetComponent<MeshFilter> ();
		terrainMesh.mesh = this.CreateTerrainMesh ();
		//MeshRenderer renderer = this.gameObject.AddComponent<MeshRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		MeshRenderer renderer = this.gameObject.GetComponent<MeshRenderer>();

		// Set blend uniform parameter in an oscillating fashion to demonstrate 
		// blending shader (challenge question)
		renderer.material.SetFloat("_BlendFct", ((int)Mathf.Sin(Time.time) + 1.0f) / 2.0f);	
	}

	private Mesh CreateTerrainMesh(){
		Mesh m = new Mesh();
		m.name = "Terrain demo";


		/***** Generate HeightMap *****/
		// Four cornors 
		heightMap [0,0] = (float)(seed + addError());
		heightMap [0, size - 1] = (float)(seed + addError());
		heightMap [size - 1, 0] = (float)(seed + addError());
		heightMap [size - 1, size - 1] = (float)(seed + addError());

		// "dimension" times iterations
		for (int i = 0; i < dimension; i++) {
			int step = size / (int)Math.Pow (2, i + 1);
			int sqrt = (int)Math.Pow (2, i); // sqrt of number of cornors

			// Diamond 
			for (int j = 0; j < (int)Math.Pow(4, i); j++){
				int x = 2 * (j / sqrt) * step + step;
				int y = 2 * (j % sqrt) * step + step;
				float tempHeight = 0;
				heightMap [x, y] = (
					heightMap [x - step, y - step] +
					heightMap [x - step, y + step] +
					heightMap [x + step, y - step] +
					heightMap [x + step, y + step]) / 4 + addError();

				// "square" surrounding each "diamond" 
				float temp = 0;

				// up
				temp =
					heightMap [x - step, y - step] +
					heightMap [x - step, y + step] +
					heightMap [x, y];
				if (x - 2 * step > 0)
					heightMap [x - step, y] = (temp + heightMap [x - 2 * step, y]) / 4 + addError();
				else
					heightMap [x - step, y] = temp / 3 + addError();
				
				// down
				temp = 
					heightMap [x + step, y - step] +
					heightMap [x + step, y + step] +
					heightMap [x, y];
				if (x + 2 * step < size)
					heightMap [x + step, y] = (temp + heightMap [x + 2 * step, y]) / 4 + addError();
				else
					heightMap [x + step, y] = temp / 3 + addError();
				
				// left
				temp =
					heightMap [x - step, y - step] +
					heightMap [x + step, y - step] +
					heightMap [x, y]; 
				if (y - 2 * step > 0)
					heightMap [x, y - step] = (temp + heightMap [x, y - 2 * step]) / 4 + addError();
				else
					heightMap [x, y - step] = temp / 3 + addError();
				
				// right
				temp = 
					heightMap [x - step, y + step] +
					heightMap [x + step, y + step] +
					heightMap [x, y];
				if (y + 2 * step < size)
					heightMap [x, y + step] = (temp + heightMap [x, y + 2 * step]) / 4 + addError();
				else
					heightMap [x, y + step] = temp / 3 + addError();
			}

			// Reduce range of noise for each iteration
			//rangeOfNoise = rangeOfNoise - rangeOfNoise / (float)dimension / 2;
//			rangeOfNoise = rangeOfNoiseList[i];
			rangeOfNoise *= 0.5f;
		}
		/***** Generate HeightMap *****/

		// Define the vertices. 
		for (int i = 0; i < vertices.Length; i++) {
			vertices [i] = new Vector3 ((float)(i % size * scale), heightMap [i / size, i % size], (float)((size - i / size) * scale));
//			Debug.Log (heightMap [i / size, i % size]);
		}
		m.vertices = vertices;

		// Automatically define the triangles based on the number of vertices
		int numOfSquares = (int)Math.Pow(4,dimension);
		int sqrtOfNumOfSquares = (int)Math.Pow (2, dimension);
		int[] triangles = new int[numOfSquares * 2 * 3];
		for (int i = 0; i < numOfSquares; i++) {
			int topLeftCornorIndex = (i / sqrtOfNumOfSquares) * size + i % sqrtOfNumOfSquares;
			triangles[2 * 3 * i + 0] = topLeftCornorIndex;
			triangles[2 * 3 * i + 1] = topLeftCornorIndex + 1;
			triangles[2 * 3 * i + 2] = topLeftCornorIndex + size;
			triangles[2 * 3 * i + 3] = topLeftCornorIndex + size;
			triangles[2 * 3 * i + 4] = topLeftCornorIndex + 1;
			triangles[2 * 3 * i + 5] = topLeftCornorIndex + size + 1;

		}

		m.triangles = triangles;

		return m;
	
	}

	private float addError(){
		System.Random rnd = new System.Random();
		float result;
		result = (float)rnd.NextDouble() * rangeOfNoise - rangeOfNoise / 2;
		return result;
	}
}

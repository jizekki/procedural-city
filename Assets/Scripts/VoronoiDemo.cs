using UnityEngine;
using System.Collections.Generic;
using Delaunay;
using Delaunay.Geo;
using UnityEngine.AI;



public class VoronoiDemo : MonoBehaviour
{

	public Material land;
	public Texture2D tx;
	public const int NPOINTS = 100;
	public const int WIDTH = 200;
	public const int HEIGHT = 200;
	public GameObject carRoad, bicycleRoad, skyscraper, house, car, bike, police, ambulance;

  	private List<Vector2> m_points;
  	private List<LineSegment> m_edges = null;
	private List<LineSegment> m_spanningTree;
	private List<LineSegment> m_delaunayTriangulation;
	private List<GameObject> housesSpawned = new List<GameObject>();

	private List<GameObject> listRoads;




    private float [,] createMap() 
    {
        float [,] map = new float[WIDTH, HEIGHT];
        for (int i = 0; i < WIDTH; i++)
            for (int j = 0; j < HEIGHT; j++)
                map[i, j] = Mathf.PerlinNoise(0.02f * i + 0.43f, 0.018f * j + 0.22f);
        return map;
    }


	List<GameObject> generateRoads(float [,] map, Color[] pixels) {

		List<GameObject> listRoads = new List<GameObject>();

		/* Create random points points */
		m_points = new List<Vector2> ();
		List<uint> colors = new List<uint> ();

		/* Randomly pick vertices */
		for (int i = 0; i < NPOINTS; i++) {
			colors.Add ((uint)0);
			Vector2 vec = new Vector2(Random.Range(0, WIDTH - 1), Random.Range(0, HEIGHT - 1));
			if (Random.Range(0.0f, 1.0f) <= map[(int)vec.x, (int)vec.y])
				m_points.Add(vec);
			else
				i--;
		}

		/* Generate Graphs */
		Delaunay.Voronoi v = new Delaunay.Voronoi (m_points, colors, new Rect (0, 0, WIDTH, HEIGHT));
		m_edges = v.VoronoiDiagram ();
		m_spanningTree = v.SpanningTree (KruskalType.MINIMUM);
		m_delaunayTriangulation = v.DelaunayTriangulation ();

		for (int i = 0; i < m_edges.Count; i++) {
			LineSegment seg = m_edges [i];				
			Vector2 left = (Vector2)seg.p0;
			Vector3 leftScaled = new Vector3(left.y * 10f / WIDTH - 5f, 0.0f, left.x * 10f / HEIGHT - 5f);
			GameObject r;

			if(Random.Range(0.0f, 1.0f) < 0.8f) {
				r = GameObject.Instantiate(carRoad, leftScaled, Quaternion.identity);
			} else {
				r = GameObject.Instantiate(bicycleRoad, leftScaled, Quaternion.identity);
			}
			Vector2 right = (Vector2)seg.p1;
			Vector3 rightScaled = new Vector3(right.y * 10f / WIDTH - 5f, r.transform.position.y, right.x * 10f / HEIGHT - 5f);
			float size = Vector2.Distance(new Vector2(left.x * 10f / WIDTH, left.y * 10f / HEIGHT), new Vector2(right.x * 10f / WIDTH, right.y * 10f / HEIGHT));
			r.transform.LookAt(rightScaled);
			r.transform.localScale = new Vector3(r.transform.localScale.x, r.transform.localScale.y, size);
			r.transform.position = (leftScaled + rightScaled) / 2;
      		buildNearHouses((left/ WIDTH * 10 - new Vector2(5f,5f)) * 1, (right/ WIDTH * 10 - new Vector2(5f,5f)) * 1, size);

			listRoads.Add(r);

		}

		return listRoads;
	}

	void Start ()
	{
    	float [,] map = createMap();

		listRoads = generateRoads(map, createPixelMap(map));

		foreach(NavMeshSurface surface in GetComponents<NavMeshSurface>()) {
            surface.BuildNavMesh();
        }

		for(int i=0; i< 20; i++) {
			GameObject r = listRoads[Random.Range(0, listRoads.Count)];
			if(i == 0) {
				Instantiate(ambulance, r.transform.position, Quaternion.identity);
			} else if (i == 1) {
				GameObject policeCar = Instantiate(police, r.transform.position, Quaternion.identity) as GameObject;
				policeCar.GetComponent<NavMeshAgent>().speed *= 1.5f;
			} else {
				if(Random.Range(0.0f, 1.0f) < 0.8f) {
					Instantiate(car, r.transform.position, Quaternion.identity);
				} else {
					Instantiate(bike, r.transform.position, Quaternion.identity);
				}
			}
		}

	}

	 void buildNearHousesOneSide(Vector2 v1, Vector2 v2, float sz)
    {
		Vector2 pointer = v2 - v1;
		float rnb = Vector2.Distance(v1, v2) / 0.30f; // Maximum number of buildings
		int lenRdm = (int)rnb;
		for (int k = 0; k < lenRdm; k++)
        {
        
			float randpos = 0.6f; // Position on road
			float rot = Vector2.SignedAngle(Vector2.right, v2 - v1);
			Vector3 posx = new Vector3(v1.y + randpos * pointer.y, 0, v1.x + randpos * pointer.x);
      
      if (sz <0.8){
        GameObject building = Instantiate(skyscraper, posx, Quaternion.Euler(0, 90 + rot, 0));
			  housesSpawned.Add(building);

      }

      if (sz >=0.8){
        // Area of houses
        // Debug.Log(sz);
        GameObject building = Instantiate(house, posx, Quaternion.Euler(0, 90 + rot, 0));
			  housesSpawned.Add(house);

      } 

		}
	}

	void buildNearHouses(Vector2 v1, Vector2 v2, float sz)
	{
		buildNearHousesOneSide(v1, v2, sz);
		buildNearHousesOneSide(v2, v1, sz);
	}

    /* Functions to create and draw on a pixel array */
    private Color[] createPixelMap(float[,] map)
    {
        Color[] pixels = new Color[WIDTH * HEIGHT];
        for (int i = 0; i < WIDTH; i++)
            for (int j = 0; j < HEIGHT; j++)
            {
                pixels[i * HEIGHT + j] = Color.Lerp(Color.white, Color.black, map[i, j]);
            }
        return pixels;
    }
}
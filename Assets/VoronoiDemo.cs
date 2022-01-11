using UnityEngine;
using System.Collections.Generic;
using Delaunay;
using Delaunay.Geo;
using UnityEngine.AI;

public class VoronoiDemo : MonoBehaviour
{

    public Material land;
    public const int NPOINTS = 100;
    public const int WIDTH = 200;
    public const int HEIGHT = 200;
	public GameObject road;
	public UnityEngine.AI.NavMeshAgent car;

	private Texture2D tx;
	private List<Vector2> m_points;
	private List<LineSegment> m_edges = null;
	private List<LineSegment> m_spanningTree;
	private List<LineSegment> m_delaunayTriangulation;
	private List<GameObject> roads;
	private List<UnityEngine.AI.NavMeshAgent> cars;



	private float [,] createMap() 
    {
        float [,] map = new float[WIDTH, HEIGHT];
        for (int i = 0; i < WIDTH; i++)
            for (int j = 0; j < HEIGHT; j++)
                map[i, j] = Mathf.PerlinNoise(0.02f * i + 0.43f, 0.018f * j + 0.22f);
        return map;
    }

	void Start ()
	{
        float [,] map = createMap();
        Color[] pixels = createPixelMap(map);
		Debug.Log(pixels[0]);
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

		Color color = Color.blue;
		Transform tr = GetComponent<Transform>();
		roads = new List<GameObject>();
		/* Shows Voronoi diagram */
		for (int i = 0; i < m_edges.Count; i++) {
			LineSegment seg = m_edges [i];				
			Vector2 left = (Vector2)seg.p0;
			Vector3 leftScaled = new Vector3(left.y * 10f / WIDTH - 5f, 0.0f, left.x * 10f / HEIGHT - 5f);
			GameObject r = GameObject.Instantiate(road, leftScaled, Quaternion.identity, tr);
			Vector2 right = (Vector2)seg.p1;
			Vector3 rightScaled = new Vector3(right.y * 10f / WIDTH - 5f, r.transform.position.y, right.x * 10f / HEIGHT - 5f);
			float size = Vector2.Distance(new Vector2(left.x * 10f / WIDTH, left.y * 10f / HEIGHT), new Vector2(right.x * 10f / WIDTH, right.y * 10f / HEIGHT));
			r.transform.LookAt(rightScaled);
			r.transform.localScale = new Vector3(r.transform.localScale.x, r.transform.localScale.y, size);
			r.transform.position = (leftScaled + rightScaled) / 2;
			roads.Add(r);
		}


		color = Color.red;

        /* Apply pixels to texture */
        tx = new Texture2D(WIDTH, HEIGHT);
        land.SetTexture ("_MainTex", tx);
		tx.SetPixels (pixels);
		tx.Apply ();

		cars = new List<UnityEngine.AI.NavMeshAgent>();

		NavMeshSurface surface = GetComponent<NavMeshSurface>();
		surface.BuildNavMesh();

		UnityEngine.AI.NavMeshAgent a = Instantiate(car, roads[Random.Range(0, roads.Count)].transform.position, Quaternion.identity, tr);
		cars.Add(a);
		cars[0].destination = new Vector3(Random.Range(3f, 5f), 0f, Random.Range(3f, 5f));

	}



	void Update()
    {
		foreach(UnityEngine.AI.NavMeshAgent car in cars)
        {
			if(car.remainingDistance == 0)
            {
				car.destination = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
			}
        }
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
using UnityEngine;
using System.Collections.Generic;
using Delaunay;
using Delaunay.Geo;
using UnityEngine.AI;



public class VoronoiDemo : MonoBehaviour
{

  public Material land;
  public Texture2D tx;
  public const int NPOINTS = 60;
  public const int WIDTH = 200;
  public const int HEIGHT = 200;
	public GameObject road, bicycleRoad, skyscraper, house, car, bike;

  private List<Vector2> m_points;
	private List<LineSegment> m_edges = null;
	private List<LineSegment> m_spanningTree;
	private List<LineSegment> m_delaunayTriangulation;
	private List<GameObject> housesSpawned = new List<GameObject>();
  public NavMeshSurface surface;




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
    float [,] map=createMap();
    Color[] pixels = createPixelMap(map);
		//Debug.Log(pixels[0]);
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
		/* Shows Voronoi diagram */
		for (int i = 0; i < m_edges.Count; i++) {
			LineSegment seg = m_edges [i];				
			Vector2 left = (Vector2)seg.p0;
			Vector3 leftScaled = new Vector3(left.y * 10f / WIDTH - 5f, 0.0f, left.x * 10f / HEIGHT - 5f);
			GameObject r ;
      if (i%7==0){
        r =  GameObject.Instantiate(bicycleRoad, leftScaled, Quaternion.identity);
      }
      else {
        r = GameObject.Instantiate(road, leftScaled, Quaternion.identity);
      }
			Vector2 right = (Vector2)seg.p1;
			Vector3 rightScaled = new Vector3(right.y * 10f / WIDTH - 5f, r.transform.position.y, right.x * 10f / HEIGHT - 5f);
			float size = Vector2.Distance(new Vector2(left.x * 10f / WIDTH, left.y * 10f / HEIGHT), new Vector2(right.x * 10f / WIDTH, right.y * 10f / HEIGHT));
			r.transform.LookAt(rightScaled);
			r.transform.localScale = new Vector3(r.transform.localScale.x, r.transform.localScale.y, size);
			r.transform.position = (leftScaled + rightScaled) / 2;
      buildNearHouses((left/ WIDTH * 10 - new Vector2(5f,5f)) * 1, (right/ WIDTH * 10 - new Vector2(5f,5f)) * 1, size);  
			//DrawLine (pixels, left, right, color);
      Instantiate(car, r.transform.position, Quaternion.identity);

      surface = GetComponent<NavMeshSurface>();
      surface.BuildNavMesh();
		}

		color = Color.red;
        /* Apply pixels to texture */
        tx = new Texture2D(WIDTH, HEIGHT);
        land.SetTexture ("_MainTex", tx);
		tx.SetPixels (pixels);
		tx.Apply ();

    Instantiate(bike, bike.transform.position, Quaternion.identity);


	}

	 void buildNearHousesOneSide(Vector2 v1, Vector2 v2, float sz)
    {
		Vector2 pointer = v2 - v1;
		float rnb = Vector2.Distance(v1, v2) / 0.30f; // Maximum number of buildings
		int lenRdm = (int)rnb;
		//List<float> rdmP = new List<float>;
		for (int k = 0; k < lenRdm; k++)
        {
        
			float randpos = 0.6f; // Position on road
			float rot = Vector2.SignedAngle(Vector2.right, v2 - v1);
			Vector3 posx = new Vector3(v1.y + randpos * pointer.y, 0, v1.x + randpos * pointer.x);
      
      if (sz <0.8){ // if road size < 0.8 => concentration of population => concentration of skyscrapers (not always true)
        // Area of Skyscrapers
        Debug.Log(sz);
        GameObject building = Instantiate(skyscraper, posx, Quaternion.Euler(0, 90 + rot, 0));
			  housesSpawned.Add(building);

      }

      if (sz >=0.8){
        // Area of houses
        Debug.Log(sz);
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
    private void DrawPoint (Color [] pixels, Vector2 p, Color c) {
		if (p.x<WIDTH&&p.x>=0&&p.y<HEIGHT&&p.y>=0) 
		    pixels[(int)p.x*HEIGHT+(int)p.y]=c;
	}
	// Bresenham line algorithm
	private void DrawLine(Color [] pixels, Vector2 p0, Vector2 p1, Color c) {
		int x0 = (int)p0.x;
		int y0 = (int)p0.y;
		int x1 = (int)p1.x;
		int y1 = (int)p1.y;

		int dx = Mathf.Abs(x1-x0);
		int dy = Mathf.Abs(y1-y0);
		int sx = x0 < x1 ? 1 : -1;
		int sy = y0 < y1 ? 1 : -1;
		int err = dx-dy;
		while (true) {
            if (x0>=0&&x0<WIDTH&&y0>=0&&y0<HEIGHT)
    			pixels[x0*HEIGHT+y0]=c;

			if (x0 == x1 && y0 == y1) break;
			int e2 = 2*err;
			if (e2 > -dy) {
				err -= dy;
				x0 += sx;
			}
			if (e2 < dx) {
				err += dx;
				y0 += sy;
			}
		}
	}
}
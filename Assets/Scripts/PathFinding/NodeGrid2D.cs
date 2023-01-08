namespace PathFinding {
	using UnityEngine;

	public class NodeGrid2D : MonoBehaviour {
		public static NodeGrid2D instance;
		void Awake() {
			if (!instance) { instance = this; }
			else if (instance != this) { Destroy(gameObject); }
		}

		public Vector2 GridSize;
		public float nodeSize = 1;
		public Vector2 center;

		public Node[,] nodeGrid { get; private set; }
		public int gridX => Mathf.FloorToInt(GridSize.x / nodeSize) + 1;
		public int gridY => Mathf.FloorToInt(GridSize.y / nodeSize) + 1;
		public Vector2 Center {
			get => (Vector2) transform.position + center;
			set => center = value - (Vector2) transform.position;
		}

		public Node GetNode(int x, int y) => nodeGrid[x, y];

		public void Initialize(Vector2Int GridSize) {
			this.GridSize = GridSize;
			nodeGrid = new Node[GridSize.x + 1, GridSize.y + 1];
			for (int x = 0; x <= GridSize.x; x++) {
				for (int y = 0; y <= GridSize.y; y++) {
					nodeGrid[x, y] = new Node(true, new Vector2(x, y), x, y);
				}
			}

			foreach (var item in nodeGrid) { item.Cache(this); }
			RequestPath.Initialize();
		}
	}

}

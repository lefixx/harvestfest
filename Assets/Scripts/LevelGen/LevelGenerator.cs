using System.Collections.Generic;
using System.Linq;

using PathFinding;

using UnityEngine;

namespace LevelGen {
	public class LevelGenerator : MonoBehaviour {
		public GridCell[,] cells;
		[SerializeField] int houseAmount = 30;
		List<House> spawnedHouses = new List<House>();

		[SerializeField] NodeGrid2D grid;
		[SerializeField] AssetMap assetMap;

		[SerializeField, HideInInspector] List<SpriteRenderer> spawnedGraphics = new List<SpriteRenderer>();

		[SerializeField] bool autoCenter = true; // should be disabled ingame.


		void Start() {
			GenerateLevel();
		}

		[ContextMenu("HOUSESSS")]
		public void GenerateLevel() {
			foreach (var g in spawnedGraphics) { Destroy(g.gameObject); }
			spawnedHouses.Clear();
			spawnedGraphics.Clear();

			// Generate Houses
			for (int i = 0; i < houseAmount; i++) {
				var houseSize = Vector2Int.one * (int) (Random.Range(1.5f, 3) * 2);
				var fineSpot = GetFinePlaceForSpawningHouse(Mathf.Max(houseSize.x, houseSize.y));
				spawnedHouses.Add(new House() { center = fineSpot, size = houseSize, DoorDirection = (Direction) Random.Range(0, 3) });
			}

			// Generate the grid
			var minX = spawnedHouses.Min(x => (x.center - x.size).x);
			var maxX = spawnedHouses.Max(x => (x.center + x.size).x);

			var minY = spawnedHouses.Min(x => (x.center - x.size).y);
			var maxY = spawnedHouses.Max(x => (x.center + x.size).y);

			var size = new Vector2Int(maxX - minX, maxY - minY) + Vector2Int.one * 12;
			if (autoCenter) { transform.position = -(Vector2) size / 2f; }

			cells = new GridCell[size.x, size.y];
			for (int x = 0; x < size.x; x++) {
				for (int y = 0; y < size.y; y++) {
					cells[x, y] = new GridCell() { clr = Color.white };
				}
			}
			grid.Initialize(size);


			// Populate it (colors for now)
			foreach (var house in spawnedHouses) {
				// Remap so there won't be negatives (leave a space of 5 units for the empty sides)
				house.center.x = Mathf.CeilToInt(MathExtensions.Remap(house.center.x, minX, maxX, 5, size.x - 5));
				house.center.y = Mathf.CeilToInt(MathExtensions.Remap(house.center.y, minY, maxY, 5, size.y - 5));

				for (int i = -house.size.x / 2; i <= house.size.x / 2; i++) {
					for (int j = -house.size.y / 2; j <= house.size.y / 2; j++) {
						cells[house.center.x + i, house.center.y + j].cellType = CellType.House;
						cells[house.center.x + i, house.center.y + j].clr = Color.green;
						cells[house.center.x + i, house.center.y + j].occupyingObject = house;
					}
				}

				cells[house.DoorPosition.x, house.DoorPosition.y].clr = Color.green * 0.8f;
			}

			// Generate Road to connect houses
			GenerateRoads();

			// Generate Props
			AddGraphics();
		}

		static int[] availableRanges = Enumerable.Range(10, 20).ToArray();              // All ints from [10,20]
		static Vector2Int[] directions = new[] {
			Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,			// straight lines
			(Vector2Int.up + Vector2Int.left), (Vector2Int.up + Vector2Int.right),		// diagonals up
			(Vector2Int.down + Vector2Int.left), (Vector2Int.down + Vector2Int.right)	// diagonals down
		};


		Vector2Int GetFinePlaceForSpawningHouse(int houseSize) {
			if (spawnedHouses.Count == 0) { return new Vector2Int(Random.Range(0, 10), Random.Range(0, 10)); }

			// For each spawned house, in random order, check all directions and all ranges to find if any spot matches the requirements
			var uncheckedHouses = new List<House>(spawnedHouses);
			while (uncheckedHouses.Count != 0) {
				var rndHouse = uncheckedHouses.GetRandom();

				// For the selected random house, check all available dirs.
				var dirs = new List<Vector2Int>(directions);
				while (dirs.Count != 0) {
					var rndDir = dirs.GetRandom();
					var extraOffset = Mathf.CeilToInt(Vector2.Dot(rndHouse.size, rndDir));

					// For the selected random direction, check all available ranges.
					var ranges = new List<int>(availableRanges);
					while (ranges.Count != 0) {
						var rndRange = availableRanges.GetRandom();

						// Check if the random position from this range satisfies our conditions
						var rndPos = rndHouse.center + rndDir * (rndRange + extraOffset);
						if (spawnedHouses.All(x => IsFarAwayEnough(x, rndPos))) { return rndPos; }

						ranges.Remove(rndRange);
					}

					dirs.Remove(rndDir);
				}

				uncheckedHouses.Remove(rndHouse);
			}
			throw new UnityException("Failed to find proper place to spawn a house. This shouldn't happen.");

			//Must be at least 10 units away from existing houses
			//Must be at most 30 units away from another house.
			bool IsFarAwayEnough(House house, Vector2 pos) => (house.center - pos).magnitude > 10 + Mathf.Max(house.size.x, house.size.y) + houseSize;
		}

		void GenerateRoads() {
			const int unwalkableOffsetPerHouse = 2;

			foreach (var house in spawnedHouses) {
				// First, mark the houses + a bit outside them as unwalkable to make room for pathfinding
				for (int i = -house.size.x / 2 - unwalkableOffsetPerHouse; i <= house.size.x / 2 + unwalkableOffsetPerHouse; i++) {
					for (int j = -house.size.y / 2 - unwalkableOffsetPerHouse; j <= house.size.y / 2 + unwalkableOffsetPerHouse; j++) {
						grid.nodeGrid[house.center.x + i, house.center.y + j].walkable = false;
					}
				}

				// Then add some road in front of the houses
				for (int i = 1; i <= 3; i++) { SetRoad(house.DoorPosition + house.DoorDirection.ToVector() * i); }
			}

			// Then we can do pathfinding and find mark the road 
			var allHouses = new List<House>(spawnedHouses);
			var roadStartPos = Vector2Int.right * cells.GetLength(0) / 2;

			while (allHouses.Count != 0) {
				var closestHouse = GetClosestHouse(roadStartPos, allHouses, out var path);
				allHouses.Remove(closestHouse);
				foreach (var point in path) {
					SetRoad(Vector2Int.FloorToInt(point.worldPosition), 1);
				}
				roadStartPos = MainRoadPos(closestHouse);
			}

			//TODO: Add width
			void SetRoad(Vector2Int pos, int width = 0) {
				for (int i = -width; i <= width; i++) {
					if (cells[pos.x, pos.y].cellType != CellType.Empty) { continue; } // Overlap (?)
					cells[pos.x, pos.y].cellType = CellType.Road;
					cells[pos.x, pos.y].clr = Color.gray;
				}
			}

			Vector2Int MainRoadPos(House house) => house.DoorPosition + house.DoorDirection.ToVector() * 3;
			House GetClosestHouse(Vector2Int closestTo, List<House> availableHouses, out List<Node> path) {
				if (availableHouses.Count == 0) {
					var randomHouse = spawnedHouses.GetRandom();
					path = new List<Node>();
					RequestPath.GetPath(closestTo, MainRoadPos(randomHouse), path);
					return randomHouse;
				}

				var pathLists = new List<List<Node>>(availableHouses.Count);
				var (min, minIndex) = (int.MaxValue, -1);
				for (int i = 0; i < availableHouses.Count; i++) {
					var newPath = new List<Node>();
					pathLists.Add(newPath);
					RequestPath.GetPath(closestTo, MainRoadPos(availableHouses[i]), newPath);
					if (newPath.Count < min) { (min, minIndex) = (newPath.Count, i); }
				}

				path = pathLists[minIndex];
				return availableHouses[minIndex];
			}
		}

		void AddGraphics() {
			for (int x = 0; x < cells.GetLength(0); x++) {
				for (int y = 0; y < cells.GetLength(1); y++) {
					var cell = cells[x, y];
					if (cell.cellType == CellType.Empty) { SpawnGraphic(x, y, assetMap.Ground.GetRandom(), Random.Range(0, 4) * 90f); }
					else if (cell.cellType == CellType.Road) { SpawnGraphic(x, y, assetMap.Road.GetRandom(), Random.Range(0, 4) * 90f); }
				}
			}

			foreach (var house in spawnedHouses) {
				var sprites = GetHouseSprites(out float rotation);
				foreach (var s in sprites) {
					SpawnGraphic(s.Pos.x, s.Pos.y, s.Spr, rotation);
				}

				// TODO: Move to modular house generator (oof sadly there might not be time)
				List<(Vector2Int Pos, Sprite Spr)> GetHouseSprites(out float rotation) {
					var s = new List<(Vector2Int, Sprite)>();
					for (int i = -house.size.x / 2; i <= house.size.x / 2; i++) {
						for (int j = -house.size.y / 2; j <= house.size.y / 2; j++) {
							var pos = new Vector2Int(house.center.x + i, house.center.y + j);
							var spr = pos == house.DoorPosition ? assetMap.HouseDoor.GetRandom() : assetMap.HouseTop.GetRandom();
							s.Add((pos, spr));
						}
					}
					rotation = house.DoorDirection switch {
						Direction.Left => -90,
						Direction.Right => 90,
						_ => (float) 0,
					};
					return s;
				}
			}

			SpriteRenderer SpawnGraphic(int x, int y, Sprite sprite, float rotation = 0) {
				var spr = new GameObject().AddComponent<SpriteRenderer>();
				spr.sprite = sprite;
				spr.transform.SetParent(this.transform);
				spr.transform.localPosition = new Vector3(x, y, 0);
				spr.transform.eulerAngles = new Vector3(0, 0, rotation);
				spawnedGraphics.Add(spr);
				return spr;
			}
		}

		void OnDrawGizmos() {
			if (cells == null) { return; }

			for (int x = 0; x < cells.GetLength(0); x++) {
				for (int y = 0; y < cells.GetLength(1); y++) {
					var pos = new Vector2(x, y);
					var size = Vector3.one * 0.9f;
					Gizmos.color = cells[x, y].clr;
					Gizmos.DrawCube(pos, size);
				}
			}
		}
	}
}
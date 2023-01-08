namespace PathFinding {
	using UnityEngine;
	using System;

	[Serializable]
	public class Node {
		public bool walkable;
		public readonly Vector2 worldPosition;
		public readonly int X, Y;

		[NonSerialized] public Neighbours neighbours;
		[NonSerialized] public int gCost, hCost;
		[NonSerialized] public int HeapIndex;
		[NonSerialized] public NodeBase parent;

		public Node(bool walkable, Vector2 worldPosition, int gridX, int gridY) {
			this.walkable = walkable;
			this.worldPosition = worldPosition;
			(X, Y) = (gridX, gridY);
		}

		public void Cache(NodeGrid2D grid) {
			neighbours = new Neighbours();
			for (int x = -1; x <= 1; x++) {
				for (int y = -1; y <= 1; y++) {
					if (x == 0 && y == 0) { continue; }
					if (x != 0 && y != 0) { continue; } //no diags
					int checkX = (int) X + x;
					int checkY = Y + y;
					bool canAdd = (checkX >= 0 && checkX < grid.gridX) && (checkY >= 0 && checkY < grid.gridY);
					if (canAdd) { neighbours.Add(new NodeBase(checkX, checkY)); }
				}
			}
		}

		public bool IsMoreEfficient(Node obj) {
			if ((gCost + hCost) == (obj.hCost + obj.gCost)) { return hCost < obj.hCost; }
			return (gCost + hCost) <= (obj.hCost + obj.gCost);
		}
	}

	//Helpers

	public struct NodeBase {
		public readonly int x, y;
		public NodeBase(int X, int Y) { x = X; y = Y; }

	}

	public class Neighbours {
		public NodeBase[] ns = new NodeBase[8];
		public int Count;
		public void Clear() => Count = 0;
		public void Add(NodeBase node) { ns[Count] = node; Count++; }
	}
}

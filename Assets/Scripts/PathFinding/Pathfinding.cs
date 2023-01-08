using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace PathFinding {
	public static class RequestPath {
		static NodeGrid2D grid;
		static Node[,] nodeGrid;
		static OpenHeap openSet;
		static ClosedHeap closedSet;
		public static void Initialize() {
			grid = Object.FindObjectOfType<NodeGrid2D>();
			nodeGrid = grid.nodeGrid;
			openSet = new OpenHeap(grid.gridX * grid.gridY);
			closedSet = new ClosedHeap(grid.gridX * grid.gridY);
		}
		public static void GetPath(Vector2Int start, Vector2Int target, List<Node> pp) {

			var startNode = nodeGrid[start.x, start.y];
			var targetNode = nodeGrid[target.x, target.y];

			openSet.Clear();
			closedSet.Clear();
			openSet.Add(startNode);
			startNode.hCost = startNode.gCost = 0;

			// do stuff until path is found..
			Node node, nbr;
			while (openSet.Count > 0) {
				node = openSet.RemoveFirst();
				closedSet.Add(node);

				// If a path was found, reverse and return the list.
				if (node == targetNode) {
					pp.Clear();
					Node currentNode = targetNode;
					while (currentNode != startNode) {
						pp.Add(currentNode);
						currentNode = nodeGrid[currentNode.parent.x, currentNode.parent.y];
					}
					pp.Reverse();
					return;
				}


				var nbs = node.neighbours.ns;
				int scanA = node.neighbours.Count;
				for (int i = 0; i < scanA; i++) {
					nbr = nodeGrid[nbs[i].x, nbs[i].y];
					if (!nbr.walkable) { continue; }
					if (closedSet.Contains(nbr)) { continue; }

					int cost = node.gCost + GetHeuristicCost(node, nbr);
					if (!openSet.Contains(nbr)) {
						nbr.gCost = cost;
						nbr.hCost = GetHeuristicCost(nbr, targetNode);
						nbr.parent = new NodeBase(node.X, node.Y);
						openSet.Add(nbr);
					}
					else if (cost < nbr.gCost) {
						nbr.gCost = cost;
						nbr.hCost = GetHeuristicCost(nbr, targetNode);
						nbr.parent = new NodeBase(node.X, node.Y);
					}
				}
			}
		}

		static int GetHeuristicCost(Node A, Node B) {
			int distX = (A.X > B.X) ? A.X - B.X : B.X - A.X;
			int distY = (A.Y > B.Y) ? A.Y - B.Y : B.Y - A.Y;

			if (distX > distY) { return 10 * (distX - distY); }
			else { return 10 * (distY - distX); }
		}

	}
}

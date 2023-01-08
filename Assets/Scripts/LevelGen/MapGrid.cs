using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum Direction { Right, Left, Down, Up}

public class House {
	public Vector2Int center;
	public Vector2Int size;

	public Direction DoorDirection;

	public Vector2Int DoorDir => Vector2Int.Scale(DoorDirection.ToVector(), size / 2);
	public Vector2Int DoorPosition => center + DoorDir;
}
public enum CellType { Empty, Road, House }
public class GridCell {
	public object occupyingObject;
	public Color clr;
	public CellType cellType;
}

static class Extensions {
	public static T GetRandom<T>(this IList<T> list) => list[Random.Range(0, list.Count)];
	public static Vector2Int ToVector(this Direction dir) {
		switch (dir) {
			case Direction.Right: return Vector2Int.right;
			case Direction.Left: return Vector2Int.left;
			case Direction.Up: return Vector2Int.up;
			case Direction.Down: return Vector2Int.down;
			default: throw new UnityException($"Unrecognized direction {(int) dir}");
		}
	}
}

static class MathExtensions {
	public static float Remap(this float f, float from1, float to1, float from2, float to2) {
		var t = Mathf.InverseLerp(from1, to1, f);
		return Mathf.Lerp(from2, to2, t);
	}

	public static T[,] RotateMatrixCounterClockwise<T>(T[,] oldMatrix) {
		T[,] newMatrix = new T[oldMatrix.GetLength(1), oldMatrix.GetLength(0)];
		int newColumn, newRow = 0;
		for (int oldColumn = oldMatrix.GetLength(1) - 1; oldColumn >= 0; oldColumn--) {
			newColumn = 0;
			for (int oldRow = 0; oldRow < oldMatrix.GetLength(0); oldRow++) {
				newMatrix[newRow, newColumn] = oldMatrix[oldRow, oldColumn];
				newColumn++;
			}
			newRow++;
		}
		return newMatrix;
	}
}
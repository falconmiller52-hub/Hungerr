using UnityEngine;

namespace Runtime.Common.Services.SaveLoad
{
	[System.Serializable]
	public struct SerializableVector2Int {
		public int x;
		public int y;

		public SerializableVector2Int(Vector2Int source) {
			x = source.x;
			y = source.y;
		}

		public readonly Vector2Int ToVector() => new(x, y);
	}
}
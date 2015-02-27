using System;

namespace ExpMapGen
{
	public static class MapEx
	{
		public static float[] MinMaxValue(this float[,] self)
		{
			float min = -1;
			float max = -1;
			bool firstRun = true;
			foreach (float current in self) {
				if (firstRun) {
					min = current;
					max = current;
					firstRun = false;
					continue;
				}

				if (min > current)
					min = current;

				if (max < current)
					max = current;
			}
			return new float[]{ min, max };
		}

		public static float ToNormalized(this float self)
		{
			return ((float)self + (global::World.Size / 2)) / Pixel.Resolution;
		}

		public static float ToWorldCoordinate(this float self)
		{
			return ((float)self * Pixel.Resolution) - (global::World.Size / 2);
		}
	}
}


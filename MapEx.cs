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

		public static float[,] Stretch(this float[,] self, int newsize)
		{
			float[,] result = new float[newsize, newsize];
			float sizemultiplier = newsize / self.GetLength(0);
			for (int x = 0; x < newsize; x++) {
				for (int z = 0; z < newsize; z++) {
					result[x, z] = self.GetAvrgValueAt(x / sizemultiplier, z / sizemultiplier);
				}
			}
			return result;
		}

		public static float GetAvrgValueAt(this float[,] self, float x, float z)
		{
			try {
				int xmin = UnityEngine.Mathf.FloorToInt(x);
				int zmin = UnityEngine.Mathf.FloorToInt(z);
				int xmax = xmin + 1;
				int zmax = zmin + 1;
				float xpercent = GetPercent(xmin, xmax, x);
				float zpercent = GetPercent(zmin, zmax, z);
				float xbw = GetValueAtPercent(self[xmin, zmin], self[xmax, zmin], xpercent);
				float zbw = GetValueAtPercent(self[xmin, zmin], self[xmin, zmax], zpercent);
				return (zbw + xbw) / 2;
			} catch (Exception ex) {
				return 0f;
			}
		}

		public static float GetPercent(float min, float max, float pointbetween)
		{
			return (pointbetween - min) / ((max - min) / 100);
		}

		public static float GetValueAtPercent(float min, float max, float percent)
		{
			return ((max - min) / 100) * percent + min;
		}
	}
}


using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace ExpMapGen
{
	public class Map
	{
		public Pixel[,] Pixels;

		public static Map map;
		public MapSettings mapSettings;

		public Map(float[,] heightmap, float[,,] biomemap, MapSettings settings)
		{
			mapSettings = settings;
			if (mapSettings.FinalResolution == -1)
				mapSettings.FinalResolution = biomemap.GetLength(0);

			if (mapSettings.FileName == "")
				mapSettings.FileName = ToString();

			map = this;
			var minmax = MinMaxValue(heightmap);
			Pixel.LowestPoint = minmax[0];
			Pixel.HeighestPoint = minmax[1];
			Pixels = new Pixel[biomemap.GetLength(1), biomemap.GetLength(0)];

			for (int x = 0; x < biomemap.GetLength(0); x++) {
				for (int z = 0; z < biomemap.GetLength(1); z++) {
					Pixels[x, z] = new Pixel(x, z, heightmap, biomemap);
				}
			}
		}

		public Pixel GetPixel(int x, int z)
		{
			return Pixels[x, z];
		}

		float[] MinMaxValue(float[,] array)
		{
			float min = -1;
			float max = -1;
			bool firstRun = true;
			foreach (float current in array) {
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

		public Texture2D GenerateTexture()
		{
			Texture2D result = new Texture2D(Pixels.GetLength(0), Pixels.GetLength(1));
			foreach (Pixel pix in Pixels) {
				result.SetPixel(pix.Z, pix.X, pix.GetColor());
			}
			result.Apply();
			return result;

		}

		public byte[] ToPNG(Texture2D t)
		{
			if (t == null)
				return null;
			return t.EncodeToPNG();
		}

		public override string ToString ()
		{
			return String.Format("RustMap_{0}_{1}@{2}.png", global::World.Seed, global::World.Size, mapSettings.FinalResolution);
		}
	}

	public class MapSettings {
		public int FinalResolution = -1;
		public bool drawSnow = true;
		public bool onlyHeights = false;
		public bool oneColorPerSplat = false;

		public string FileName = "";

		public MapSettings()
		{
		}
	}
}


using System;
using UnityEngine;

namespace ExpMapGen
{
	public class Pixel
	{
		public static float LowestPoint;
		public static float HeighestPoint;

		public static Color RockDarkest = new Color(0.05f, 0.05f, 0.05f);
		public static Color RockBrightest = new Color(0.50f, 0.50f, 0.50f);
		public static Color GrassDarkest = new Color(0.18f, 0.423f, 0.24f);
		public static Color GrassBrightest = new Color(0.234f, 0.58f, 0.34141f);
		public static Color SandDarkest = new Color(0.738f, 0.715f, 0.42f);
		public static Color SandBrightest = new Color(0.9375f, 0.885f, 0.547f);
		public static Color DirtDarkest = new Color(0.328f, 0.207f, 0.125f);
		public static Color DirtBrightest = new Color(0.4883f, 0.336f, 0.102f);
		public static Color ForestDarkest = new Color(0.07f, 0.18f, 0.07f);
		public static Color ForestBrightest = new Color(0.12f, 0.46f, 0.12f);
		public static Color TundraDarkest = new Color(0.6f, 0.6f, 0.6f);
		public static Color TundraBrightest = new Color(0.8f, 0.8f, 0.8f);
		public static Color SnowDarkest = new Color(0.8f, 0.8f, 0.8f);
		public static Color SnowBrightest = new Color(1f, 1f, 1f);
		public static Color PathDarkest = Color.cyan;
		public static Color PathBrightest = Color.cyan;
		public static Color NoneDarkest = Color.magenta;
		public static Color NoneBrightest = Color.magenta;

		public static int MapSize = 0;

		public int X;
		public int Z;

		public float Height;

		public float Rock;		// 0
		public float Grass; 	// 1
		public float Sand;		// 2
		public float Dirt;		// 3
		public float Forest;	// 4
		public float Tundra;	// 5
		public float Snow;		// 6
		public float Path;		// 7
		public float None;		// 8

		public bool Water;

		public Pixel(int x, int z, float[,] heightmap, float[,,] biomemap, bool twice)
		{
			X = x;
			Z = z;

			if (!twice) {
				Height = heightmap[x, z];
			} else {
				Height = heightmap[x * 2, z * 2];
			}

			Rock   = biomemap[x, z, 0];	
			Grass  = biomemap[x, z, 1]; 
			Sand   = biomemap[x, z, 2];	
			Dirt   = biomemap[x, z, 3];	
			Forest = biomemap[x, z, 4];
			Tundra = biomemap[x, z, 5];
			Snow   = biomemap[x, z, 6];
			Path   = biomemap[x, z, 7];
			None   = biomemap[x, z, 8];

			Init();
		}

		public void Init()
		{
			Water = Height < 0.5f;
		}

		public Color GetColor()
		{
			if (Water)
				return CalculateWaterColor();
			return CalculateSplatColor();
		}

		public Color CalculateWaterColor()
		{
			if (Map.map.mapSettings.oneColorPerSplat) {
				return new Color (0.125f, 0.595f, 0.664f);
			}
			Color wD = new Color (0.01f, 0.123f, 0.14f);
			Color wS = new Color (0.125f, 0.595f, 0.664f);
			return MoveByPercent(wD, wS, StretchedWaterLevel() * 100);
		}

		public float GetValueAtPercent(float min, float max, float percent)
		{
			return ((max - min) / 100) * percent + min;
		}

		public Color GetColorAtPercent(Color from, Color to, float percent)
		{
			return new Color(
				GetValueAtPercent(from.r, to.r, percent),
				GetValueAtPercent(from.g, to.g, percent),
				GetValueAtPercent(from.b, to.b, percent)
			);
		}

		public Color MoveByPercent(Color from, Color to, float factor)
		{
			if (factor == 0f || Single.IsNaN(factor))
				return from;

			if (Map.map.mapSettings.oneColorPerSplat) {
				factor = 50f;
			}

			return GetColorAtPercent(from, to, factor);
		}

		static Color result = new Color(0f,0f,0f);
		public Color CalculateSplatColor()
		{
			if (Map.map.mapSettings.onlyHeights) {
				float h = CalculatePercent() / 100f;
				return new Color(h, h, h);
			}

			Color rock = Color.black;
			Color grass = Color.black;
			Color sand = Color.black;
			Color dirt = Color.black;
			Color forest = Color.black;
			Color tundra = Color.black;
			if (Rock != 0) {
				rock = MoveByPercent(RockDarkest, RockBrightest, CalculatePercent());
				rock = MoveByPercent(result, rock, Rock * 100);
			}
			if (Grass != 0) {
				grass = MoveByPercent(GrassDarkest, GrassBrightest, CalculatePercent());
				grass = MoveByPercent(result, grass, Grass * 100);
			}
			if (Sand != 0) {
				sand = MoveByPercent(SandDarkest, SandBrightest, CalculatePercent());
				sand = MoveByPercent(result, sand, Sand * 100);
			}
			if (Dirt != 0) {
				dirt = MoveByPercent(DirtDarkest, DirtBrightest, CalculatePercent());
				dirt = MoveByPercent(result, dirt, Dirt * 100);
			}
			if (Forest != 0) {
				forest = MoveByPercent(ForestDarkest, ForestBrightest, CalculatePercent());
				forest = MoveByPercent(result, forest, Forest * 100);
			}
			if (Tundra != 0) {
				tundra = MoveByPercent(TundraDarkest, TundraBrightest, CalculatePercent());
				tundra = MoveByPercent(result, tundra, Tundra * 100);
			}
			result = MixColors(rock, grass, sand, dirt, forest, tundra);

			// Layers
			if (Map.map.mapSettings.drawSnow) {
				if (Snow != 0)
					result = MoveByPercent(SnowDarkest, SnowBrightest, CalculatePercent());
			}
			if (Path != 0)
				result = MoveByPercent(PathDarkest, PathBrightest, CalculatePercent());
			if (None != 0)
				result = MoveByPercent(NoneDarkest, NoneBrightest, CalculatePercent());

			return result;
		}

		public Color CalculateHeightColor()
		{
			float shv = StretchedHeightLevel();
			return new Color(shv, shv, shv);
		}

		public float CalculatePercent()
		{
			return 50f + ((LightLevel() - ShadowLevel()) * 12.5f) + (StretchedHeightLevel() * 25f) - 12.5f;
		}

		public float StretchedWaterLevel()
		{
			return (Height - LowestPoint) * (1 / (0.5f - LowestPoint));
		}

		public float StretchedHeightLevel()
		{
			return (Height - 0.5f) * (1 / (HeighestPoint - 0.5f));
		}

		public float StretchedLightShadowLevel()
		{
			return (LightLevel() - ShadowLevel() + 3f) / 6;
		}

		public int LightLevel()
		{
			try {
				int ll = 0;
				ll += Z == 0 ? 1 : (Map.map.GetPixel(X, Z - 1).Height > Height) ? 1 : 0;
				ll += X == 0 ? 1 :(Map.map.GetPixel(X - 1, Z).Height > Height) ? 1 : 0;
				ll += (X == 0 || Z == 0) ? 1 : (Map.map.GetPixel(X - 1, Z - 1).Height > Height) ? 1 : 0;
				return ll;
			} catch (Exception ex) {
				//Pluton.Logger.LogException (ex);
				return 0;
			}
		}

		public int ShadowLevel()
		{
			try {
				int ll = 0;
				ll += Z == MapSize ? 0 : (Map.map.GetPixel(X, Z + 1).Height > Height) ? 1 : 0;
				ll += X == MapSize ? 0 : (Map.map.GetPixel(X + 1, Z).Height > Height) ? 1 : 0;
				ll += (X == MapSize || Z == MapSize) ? 0 : (Map.map.GetPixel(X + 1, Z + 1).Height > Height) ? 1 : 0;
				return ll;
			} catch (Exception ex) {
				//Pluton.Logger.LogException (ex);
				return 0;
			}
		}

		public Color MixColors(params Color[] colors)
		{
			float r = 0f, g = 0f, b = 0f;
			int count = 0;
			for (int i = 0; i < colors.GetLength(0); i++) {
				Color current = colors[i];
				if (current == Color.black)
					continue;

				r += current.r;
				g += current.g;
				b += current.b;
				count++;
			}
			return new Color(r / count, g / count, b / count);
		}
	}
}


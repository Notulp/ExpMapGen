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
 		public float[,] HeightMap;
 		public float[,,] BiomeMap; 

		public Map(float[,] heightmap, float[,,] biomemap, MapSettings settings)
		{
			mapSettings = settings;

			if (mapSettings.FileName == "")
				mapSettings.FileName = ToString();

			map = this;
 		 	HeightMap = heightmap;
 		 	BiomeMap = biomemap;
 
			int heightLength = heightmap.GetLength(0) - 1;
			int biomeLength = biomemap.GetLength(0);

			bool twice = heightLength == biomeLength * 2;

			var minmax = heightmap.MinMaxValue();
			Pixel.LowestPoint = minmax[0];
			Pixel.HeighestPoint = minmax[1];
			Pixel.MapSize = biomeLength;
			if (mapSettings.FinalResolution == -1)
				mapSettings.FinalResolution = Pixel.MapSize;
			Pluton.Logger.LogWarning("Heightmap: " + heightLength + "x" + heightLength + " min-maxpoints: " + minmax[0].ToString() + " - " + minmax[1].ToString());
			Pluton.Logger.LogWarning("Biomemap: " + biomeLength + "x" + biomeLength + "x" + biomemap.GetLength(2));
			Pixels = new Pixel[biomeLength, biomeLength];

			for (int x = 0; x <biomeLength; x++) {
				for (int z = 0; z < biomeLength; z++) {
					Pixels[x, z] = new Pixel(x, z, twice);
				}
			}
		}

		public Pixel GetPixel(int x, int z)
		{
			try {
				return Pixels[x, z];
			} catch {
				throw new Exception(String.Format("Invalid array index {0}:{1} [{2}:{3}]", x, z, Pixels.GetLength(0), Pixels.GetLength(1)));
			}
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

		public void ParseCommand(MapGenCommand command)
		{
			int res = -1;
			switch (command.Command) {
			case "snow":
				res = 0;
				break;
			case "ho":
				res = 1;
				break;
			case "heightonly":
				res = 1;
				break;
			case "noheight":
				res = 2;
				break;
			case "nh":
				res = 2;
				break;
			case "out":
				res = 3;
				break;
			case "output":
				res = 3;
				break;
			case "file":
				res = 3;
				break;
			case "filename":
				res = 3;
				break;
			}
			switch (res) {
			case 0:
				if (!Boolean.TryParse(command.Args[0], out drawSnow)) {
					Pluton.Logger.LogWarning(String.Format("Couldn't parse: {0} {1}", command.Command, command.Args[0]));
				}
				break;
			case 1:
				if (!Boolean.TryParse(command.Args[0], out onlyHeights)) {
					Pluton.Logger.LogWarning(String.Format("Couldn't parse: {0} {1}", command.Command, command.Args[0]));
				}
				break;
			case 2:
				if (!Boolean.TryParse(command.Args[0], out oneColorPerSplat)) {
					Pluton.Logger.LogWarning(String.Format("Couldn't parse: {0} {1}", command.Command, command.Args[0]));
				}
				break;
			case 3:
				string fn = String.Join(" ", command.Args);
				fn = fn.Replace("%seed", global::World.Seed.ToString())
					.Replace("%size", global::World.Size.ToString());

				if (!fn.ToLower().EndsWith(".png"))
					fn += ".png";

				if (!String.IsNullOrEmpty(fn)) {
					FileName = fn;
				} else {
					Pluton.Logger.LogWarning(String.Format("Couldn't parse: {0} {1}", command.Command, String.Join(" ", command.Args)));
				}
				break;
			case -1:
				Pluton.Logger.LogWarning(String.Format("Couldn't parse: {0} {1}", command.Command, String.Join(" ", command.Args)));
				break;
			}
		}
	}
}


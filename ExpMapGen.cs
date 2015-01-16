using System;
using System.IO;
using Pluton;
using UnityEngine;

namespace ExpMapGen
{
	public class ExpMapGen : CSharpPlugin
	{
		float[,] heightmap;
		float[,,] splatmap;

		public void On_PluginInit()
		{
			Logger.LogError("Works!");
			ServerConsoleCommands.Register("genmap")
				.setCallback(Generate)
				.setDescription("Generates a map and saves to a .png file!")
				.setUsage("");
		}

		// TODO: handle args
		public void Generate(string[] args)
		{
			MapSettings settings = new MapSettings();
			GenerateMap(settings);
		}

		public void GenerateMap(MapSettings settings)
		{
			try {
				TerrainGenerator tg = SingletonComponent<TerrainGenerator>.Instance;

				heightmap = (float[,])tg.GetFieldValue("heightmap");
				splatmap = (float[,,])tg.GetFieldValue("splatmap");

				Map map = new Map(heightmap, splatmap, settings);

				SaveImage(map.mapSettings.FileName, map.ToPNG(map.GenerateTexture()));

			} catch (Exception ex) {
				Logger.LogError(ex.StackTrace);
			}
		}

		void SaveImage(string name, byte[] image)
		{
			if (image != null)
				File.WriteAllBytes(Path.Combine(Plugin.RootDir.FullName, name), image);
			else
				Logger.LogWarning(name + " is a null texture");
		}
	}
}


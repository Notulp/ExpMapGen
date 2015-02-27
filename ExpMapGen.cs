using System;
using System.Collections.Generic;
using System.IO;
using Pluton;
using UnityEngine;

namespace ExpMapGen
{
	public class ExpMapGen : CSharpPlugin
	{
		public void On_PluginInit()
		{
			ServerConsoleCommands.Register("genmap")
				.setCallback(Generate)
				.setDescription("Generates a map and saves to a .png file!")
				.setUsage("");
		}

		public void Generate(string[] args)
		{
			string cmd = "";
			List<string> arg = new List<string>();
			MapSettings settings = new MapSettings();

			for (int i = 0; i < args.Length; i++) {
				string current = args[i];
				bool last = i == (args.Length - 1);
				if (current.StartsWith("-")) {
					if (cmd != "") {
						MapGenCommand command = new MapGenCommand(cmd, arg);
						settings.ParseCommand(command);
						arg = new List<string>();
					}
					cmd = current.Remove(0, 1);
				} else {
					if (cmd == "") {
						continue;
					}
					arg.Add(current);
				}
				if (last) {
					MapGenCommand command = new MapGenCommand(cmd, arg);
					settings.ParseCommand(command);
				}
			}
			GenerateMap(settings);
			Map.map = null;
		}

		public void GenerateMap(MapSettings settings)
		{
			Map map = new Map(settings);

			SaveImage(map.mapSettings.FileName.Replace("%res", map.mapSettings.FinalResolution.ToString()), map.ToPNG(map.GenerateTexture()));
		}

		void SaveImage(string name, byte[] image)
		{
			if (image != null) {
				File.WriteAllBytes (Path.Combine (Plugin.RootDir.FullName, name), image);
				Debug.Log("Saved map: " + name);
			} else {
				Logger.LogWarning (name + " is a null texture");
			}
		}
	}
}


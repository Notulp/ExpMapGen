using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpMapGen
{
	public class MapGenCommand
	{
		public static List<string> ValidCommands = new List<string>(){
			"snow",
			"heightonly",
			"ho",
			"noheight",
			"nh",
			"file",
			"output",
			"filename",
			"out",
			"resolution",
			"res"
		};

		public String Command;
		public String[] Args;

		public MapGenCommand(string cmd, List<string> args)
		{
			if (ValidCommands.Contains(cmd)) {
				Command = cmd;
			}
			Args = args.ToArray();
		}
	}
}


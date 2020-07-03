using System;
using System.Collections.Generic;
using System.Linq;
using TilesWalk.Gameplay.Condition;
using TilesWalk.General;
using TilesWalk.Tile.Rules;

namespace TilesWalk.Building.Level
{
	[Serializable]
	public class LevelMap : GenericMap
	{
		public int StarsRequired;
		public int Target;
		public FinishCondition FinishCondition;

		public LevelMap() : base()
		{
		}

		public LevelMap(LevelMap copyFrom) : base(copyFrom)
		{
			this.StarsRequired = copyFrom.StarsRequired;
			Target = copyFrom.Target;
			FinishCondition = copyFrom.FinishCondition;
		}

		public static void FromQRString(string text, out LevelMap map, out MapFinishCondition condition)
		{
			var split = text.Split('%');
			var header = split[0];

			var headerSplit = header.Split('$');

			var id = headerSplit[0];
			var instructionCount = int.Parse(headerSplit[1]);
			var tilesCount = int.Parse(headerSplit[2]);
			var mapSize = int.Parse(headerSplit[3]);
			var targetPoints = int.Parse(headerSplit[4]);

			var finishCondition = FinishCondition.MovesLimit;
			var limit = int.Parse(headerSplit[6]);

			switch (headerSplit[5])
			{
				case "T":
					finishCondition = FinishCondition.TimeLimit;
					break;
				case "M":
					finishCondition = FinishCondition.MovesLimit;
					break;
			}

			var listInstructions = new List<InsertionInstruction>();
			// avoid repetitions
			var listTiles = new HashSet<int>();

			for (int i = 1; i < split.Length; i++)
			{
				var instructionSplit = split[i].Split('$');
				var root = int.Parse(instructionSplit[0]);
				var tile = int.Parse(instructionSplit[1]);
				var instruction = instructionSplit[2];
				var cardinalDirection = CardinalDirection.None;
				var rule = NeighborWalkRule.Plain;

				switch (instruction[0])
				{
					case 'N':
						cardinalDirection = CardinalDirection.North;
						break;
					case 'S':
						cardinalDirection = CardinalDirection.South;
						break;
					case 'E':
						cardinalDirection = CardinalDirection.East;
						break;
					case 'W':
						cardinalDirection = CardinalDirection.West;
						break;
				}

				switch (instruction[1])
				{
					case 'U':
						rule = NeighborWalkRule.Up;
						break;
					case 'P':
						rule = NeighborWalkRule.Plain;
						break;
					case 'D':
						rule = NeighborWalkRule.Down;
						break;
				}

				listTiles.Add(root);
				listTiles.Add(tile);
				listInstructions.Add(new InsertionInstruction()
				{
					Root = root,
					Tile = tile,
					Direction = cardinalDirection,
					Rule = rule
				});
			}

			map = new LevelMap()
			{
				Id = id,
				Instructions = listInstructions,
				Tiles = listTiles.ToList(),
				MapSize = mapSize,
				StarsRequired = 0,
				Target = targetPoints,
				FinishCondition = finishCondition,
			};

			condition = null;

			switch (finishCondition)
			{
				case FinishCondition.TimeLimit:
					condition = new TimeFinishCondition(id, limit);
					break;
				case FinishCondition.MovesLimit:
					condition = new MovesFinishCondition(id, limit);
					break;
			}
		}

		public string ToQRString(int conditionLimit)
		{
			var result = string.Empty;
			// first define the header
			result += $"{Id}${Instructions.Count}${Tiles.Count}${MapSize}${Target}";

			switch (FinishCondition)
			{
				case FinishCondition.TimeLimit:
					result += $"$T${conditionLimit}%";
					break;
				case FinishCondition.MovesLimit:
					result += $"$M${conditionLimit}%";
					break;
			}

			Dictionary<int, int> tileIdRemap = new Dictionary<int, int>();
			var i = 0;
			// remap to smaller ids
			foreach (var tile in Tiles)
			{
				tileIdRemap[tile] = i++;
			}

			for (int j = 0; j < Instructions.Count; j++)
			{
				var instruction = Instructions[j];

				result += $"{tileIdRemap[instruction.Root]}${tileIdRemap[instruction.Tile]}";

				switch (instruction.Direction)
				{
					case CardinalDirection.North:
						result += "$N";
						break;
					case CardinalDirection.South:
						result += "$S";
						break;
					case CardinalDirection.East:
						result += "$E";
						break;
					case CardinalDirection.West:
						result += "$W";
						break;
				}

				switch (instruction.Rule)
				{
					case NeighborWalkRule.Up:
						result += "U";
						break;
					case NeighborWalkRule.Plain:
						result += "P";
						break;
					case NeighborWalkRule.Down:
						result += "D";
						break;
				}

				if (j != Instructions.Count - 1) result += "%";
			}

			return result;
		}
	}
}
using System;
using System.Collections.Generic;
using TilesWalk.Gameplay.Condition;
using TilesWalk.General;
using TilesWalk.Tile.Rules;
using UnityEngine;

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
			var roots = split[1];
			var instructions = split[2];

			// extract header
			var headerSplit = header.Split('$');

			var id = headerSplit[0];
			var mapSize = int.Parse(headerSplit[1]);
			var targetPoints = int.Parse(headerSplit[2]);

			var finishCondition = FinishCondition.MovesLimit;
			var limit = int.Parse(headerSplit[4]);

			switch (headerSplit[3])
			{
				case "T":
					finishCondition = FinishCondition.TimeLimit;
					break;
				case "M":
					finishCondition = FinishCondition.MovesLimit;
					break;
			}

			// extract roots
			var listRoots = new List<RootTile>();
			var rootsSplit = roots.Split('#');

			for (int i = 0; i < rootsSplit.Length; i++)
			{
				var rootInstruction = rootsSplit[i].Split('$');

				var key = int.Parse(rootInstruction[0]);

				var position = new Vector3
				(
					float.Parse(rootInstruction[1]),
					float.Parse(rootInstruction[2]),
					float.Parse(rootInstruction[3])
				);

				var rotation = new Vector3
				(
					float.Parse(rootInstruction[4]),
					float.Parse(rootInstruction[5]),
					float.Parse(rootInstruction[6])
				);

				listRoots.Add(new RootTile()
				{
					Key = key,
					Position = position,
					Rotation = rotation
				});
			}

			// extract instructions
			var listInstructions = new List<InsertionInstruction>();
			var instructionSplits = instructions.Split('#');

			for (int i = 0; i < instructionSplits.Length; i++)
			{
				var instructionSplit = instructionSplits[i].Split('$');
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
				Roots = listRoots,
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
			result += $"{Id}${MapSize}${Target}";

			switch (FinishCondition)
			{
				case FinishCondition.TimeLimit:
					result += $"$T${conditionLimit}%";
					break;
				case FinishCondition.MovesLimit:
					result += $"$M${conditionLimit}%";
					break;
			}

			// remap to smaller ids since instance hash is usually used, we need to compress
			// the data within 2700 characters
			Dictionary<int, int> tileIdRemap = new Dictionary<int, int>();

			foreach (var instruction in Instructions)
			{
				if (!tileIdRemap.TryGetValue(instruction.Root, out _))
				{
					tileIdRemap[instruction.Root] = tileIdRemap.Count;
				}

				if (!tileIdRemap.TryGetValue(instruction.Tile, out _))
				{
					tileIdRemap[instruction.Tile] = tileIdRemap.Count;
				}
			}

			// insert roots
			for (int i = 0; i < Roots.Count; i++)
			{
				var rootTile = Roots[i];
				result += $"{tileIdRemap[rootTile.Key]}" +
				          $"${rootTile.Position.x:0.0}${rootTile.Position.y:0.0}${rootTile.Position.z:0.0}" +
				          $"${rootTile.Rotation.x:0.0}${rootTile.Rotation.y:0.0}${rootTile.Rotation.z:0.0}";

				if (i != Roots.Count - 1) result += "#";
			}

			result += "%";

			for (int i = 0; i < Instructions.Count; i++)
			{
				var instruction = Instructions[i];

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

				if (i != Instructions.Count - 1) result += "#";
			}

			return result;
		}
	}
}
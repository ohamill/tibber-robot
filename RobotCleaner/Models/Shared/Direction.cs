using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RobotCleaner.Models.Shared;

[JsonConverter(typeof(StringEnumConverter))]
public enum Direction
{
    North,
    South,
    East,
    West
}
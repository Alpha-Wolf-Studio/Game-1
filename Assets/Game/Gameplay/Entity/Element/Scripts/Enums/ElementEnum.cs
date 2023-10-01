using System;
using System.Runtime.Serialization;

[Serializable]
public enum ELEMENT_TYPE
{
    [EnumMember(Value = "Empty")]
    EMPTY,
    [EnumMember(Value = "water")]
    water,
    [EnumMember(Value = "fire")]
    fire,
    [EnumMember(Value = "dirt")]
    dirt,
    [EnumMember(Value = "wind")]
    wind,
    [EnumMember(Value = "Water")]
    WATER_POWERUP,
    [EnumMember(Value = "Fire")]
    FIRE_POWERUP,
    [EnumMember(Value = "Dirt")]
    DIRT_POWERUP,
    [EnumMember(Value = "Wind")]
    WIND_POWERUP,
    [EnumMember(Value = "Lava")]
    LAVA,
    [EnumMember(Value = "Cyclone")]
    CYCLONE,
    [EnumMember(Value = "Invalid")]
    INVALID
}
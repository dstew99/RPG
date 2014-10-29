// Project : RPG-csharp - C#
// File : Enums.cs
// Developer : Alireza Khodakarami
// Created : 2014 / 08 / 04
// Last Update : 2014 / 08 / 04

#region Documentation
/// <summary>   Values that represent enums. </summary>
///
/// <remarks>   Dave, 10/26/2014. </remarks>
#endregion

public enum Enums
{
        
}

#region Documentation
/// <summary>   Values that represent gizmo types. </summary>
///
/// <remarks>   Dave, 10/26/2014. </remarks>
#endregion

public enum GizmoTypes
{
    /// <summary>   An enum constant representing the box option. </summary>
    Box,            // DONE
    /// <summary>   An enum constant representing the sphere option. </summary>
    Sphere,         // DONE
    /// <summary>   An enum constant representing the capsule option. </summary>
    Capsule,        // DONE
    /// <summary>   An enum constant representing the character option. </summary>
    Character,
    /// <summary>   An enum constant representing the player option. </summary>
    Player,         // DONE
    /// <summary>   An enum constant representing the circle option. </summary>
    Circle,         // DONE
    /// <summary>   An enum constant representing the arrows option. </summary>
    Arrows,         // DONE
    /// <summary>   An enum constant representing the direction option. </summary>
    Direction,      // DONE
    /// <summary>   An enum constant representing the anchor option. </summary>
    Anchor,         // DONE
    /// <summary>   An enum constant representing the text option. </summary>
    Text,           // DONE
    /// <summary>   An enum constant representing the none option. </summary>
    None            // DONE
}

#region Documentation
/// <summary>   Values that represent point behaviors. </summary>
///
/// <remarks>   Dave, 10/26/2014. </remarks>
#endregion

public enum PointBehaviors
{
    /// <summary>   An enum constant representing the none option. </summary>
    None,           // DONE
    /// <summary>   An enum constant representing the player spawn option. </summary>
    PlayerSpawn,    
    /// <summary>   An enum constant representing the enemy spawn option. </summary>
    EnemySpawn,
    /// <summary>   An enum constant representing the npc spawn option. </summary>
    NPCSpawn,
    /// <summary>   An enum constant representing the anchor option. </summary>
    Anchor,         // DONE
    /// <summary>   An enum constant representing the way point option. </summary>
    WayPoint
}
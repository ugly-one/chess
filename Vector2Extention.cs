using System;
using System.Collections.Generic;
using Godot;

namespace Bla;

public static class Vector2Extention
{
    /// <summary>
    /// Returns the fields that are between start and end
    /// </summary>
    /// <param name="start">start position</param>
    /// <param name="end">end position</param>
    /// <returns>a collection of positions in between start and end</returns>
    public static Vector2[] GetFieldsOnPathTo(this Vector2 start, Vector2 end)
    {
        var path = new List<Vector2>();
        
        var diff = end - start;
        if (diff.Abs() == new Vector2(1, 0) || diff.Abs() == new Vector2(0, 1))
        { 
            // if we're moving only one field - no path to check
            return path.ToArray();
        }

        if (diff.X == 0 || diff.Y == 0)
        {
            var fieldsInBetween = diff.Length();
            var field = start;
            for (var i = 0; i < fieldsInBetween - 1; i++)
            {
                field += diff.Normalized();
                path.Add(field);
            }
            return path.ToArray();
        }

        if (Math.Abs(diff.X) == Math.Abs(diff.Y))
        {
            var xDirection = Math.Sign(diff.X);
            var yDirection = Math.Sign(diff.Y);

            var fieldsInBetween = Math.Abs(start.X - end.X);
            var field = start;
            for (var i = 0; i < fieldsInBetween - 1; i++)
            {
                field += new Vector2(xDirection, yDirection);
                path.Add(field);
            }
            return path.ToArray();
        }
        return path.ToArray();
    }
}
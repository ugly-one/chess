using System;

namespace Chess;

internal static class PieceTypeExtension
{
    public static PieceType[] PieceTypes = (PieceType[])Enum.GetValues(typeof(PieceType));
}


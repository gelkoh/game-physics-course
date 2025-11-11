using GameLibrary;
using Microsoft.Xna.Framework.Graphics;

namespace BallDrop;

public struct ConvexTilemapRuleTextures
{
    public Texture2D Default;
    public Texture2D Single;
        
    public Texture2D EdgeLeft;
    public Texture2D EdgeTopLeft;
    public Texture2D EdgeTop;
    public Texture2D EdgeTopRight;
    public Texture2D EdgeRight;
    public Texture2D EdgeBottomRight;
    public Texture2D EdgeBottom;
    public Texture2D EdgeBottomLeft;

    public Texture2D EdgeTopOneWide;
    public Texture2D EdgeBottomOneWide;
    public Texture2D CenterOneWide;

    public Texture2D EdgeLeftOneHigh;
    public Texture2D EdgeRightOneHigh;
    public Texture2D CenterOneHigh;

    public Texture2D GetTextureFromPositionAndDimensions(Vector2Int tilePosition, Vector2Int fullShapeDimensions)
    {
        if (fullShapeDimensions is { X: 1, Y: 1 }) return Single ?? Default;

        if (tilePosition.X == 0 && fullShapeDimensions.X > 1)
        {
            if (fullShapeDimensions.Y == 1) return EdgeLeftOneHigh ?? Default;
            if (tilePosition.Y == 0) return EdgeBottomLeft ?? Default;
            if (tilePosition.Y == fullShapeDimensions.Y - 1) return EdgeTopLeft ?? Default;
            return EdgeLeft ?? Default;
        }
        if (tilePosition.X == fullShapeDimensions.X - 1 && fullShapeDimensions.X > 1)
        {
            if (fullShapeDimensions.Y == 1) return EdgeRightOneHigh ?? Default;
            if (tilePosition.Y == 0) return EdgeBottomRight ?? Default;
            if (tilePosition.Y == fullShapeDimensions.Y - 1) return EdgeTopRight ?? Default;
            return EdgeRight ?? Default;
        }
        if (tilePosition.Y == 0 && fullShapeDimensions.Y > 1)
        {
            if (fullShapeDimensions.X == 1) return EdgeBottomOneWide ?? Default;
            return EdgeBottom ?? Default;
        }
        if (tilePosition.Y == fullShapeDimensions.Y - 1 && fullShapeDimensions.Y > 1)
        {
            if (fullShapeDimensions.X == 1) return EdgeTopOneWide ?? Default;
            return EdgeTop ?? Default;
        }
        if (fullShapeDimensions.X == 1) return CenterOneWide ?? Default;
        if (fullShapeDimensions.Y == 1) return CenterOneHigh ?? Default;

        return Default;
    }
}
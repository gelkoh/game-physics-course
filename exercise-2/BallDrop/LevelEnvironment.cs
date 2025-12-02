using GameLibrary;
using GameLibrary.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
// ReSharper disable NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract

namespace BallDrop;

public class LevelEnvironment
{
    private ContentManager _content;

    private GameObject.Template _yellowBoardTemplate3x1;
    private GameObject.Template _yellowBoardTemplate4x1;
    private GameObject.Template _sideBorderTemplate;
    private GameObject.Template _topBorderTemplate;
    private GameObject.Template _bounceBallTemplate;

    private const int TileSize = 40;
    private const float SpriteScale = 0.5f;
    
    public LevelEnvironment(ContentManager content)
    {
        _content = content;
        CreateTemplates();
    }
    
    public void Build()
    {
        _yellowBoardTemplate4x1.Instantiate(new Vector2(180, 300), MathHelper.ToRadians(45f));
        _yellowBoardTemplate4x1.Instantiate(new Vector2(420, 300), MathHelper.ToRadians(-45f));
        _yellowBoardTemplate3x1.Instantiate(new Vector2(210, 700), MathHelper.ToRadians(-25f));
        _yellowBoardTemplate3x1.Instantiate(new Vector2(390, 700), MathHelper.ToRadians(25f));
        _bounceBallTemplate.Instantiate(new Vector2(120, 500));
        _bounceBallTemplate.Instantiate(new Vector2(230, 500));
        _bounceBallTemplate.Instantiate(new Vector2(370, 500));
        _bounceBallTemplate.Instantiate(new Vector2(480, 500));
        _bounceBallTemplate.Instantiate(new Vector2(100, 800));
        _bounceBallTemplate.Instantiate(new Vector2(500, 800));
        _sideBorderTemplate.Instantiate(new Vector2(TileSize/2, 12 * TileSize));
        _sideBorderTemplate.Instantiate(new Vector2(14.5f * TileSize, 12 * TileSize));
        _topBorderTemplate.Instantiate(new Vector2(7.5f * TileSize, TileSize/2));
    }

    private void CreateTemplates()
    {
        var yellowBoardRuleTextures = new ConvexTilemapRuleTextures
        {
            EdgeLeftOneHigh = _content.Load<Texture2D>("Environment/tile_left"),
            EdgeRightOneHigh = _content.Load<Texture2D>("Environment/tile_right"),
            CenterOneHigh = _content.Load<Texture2D>("Environment/tile_center")
        };
        _yellowBoardTemplate3x1 = CreateBoxTileShapeTemplate(3, 1, yellowBoardRuleTextures);
        _yellowBoardTemplate4x1 = CreateBoxTileShapeTemplate(4, 1, yellowBoardRuleTextures);
        var borderWallsRuleTextures = new ConvexTilemapRuleTextures()
        {
            Default = _content.Load<Texture2D>("Environment/tile_grey")
        };
        _sideBorderTemplate = CreateBoxTileShapeTemplate(1, 24, borderWallsRuleTextures);
        _topBorderTemplate = CreateBoxTileShapeTemplate(13, 1, borderWallsRuleTextures);
        _bounceBallTemplate = new GameObject.Template([
            new SpriteRenderer.Builder(_content.Load<Texture2D>("Environment/blue_circle")).WithScale(SpriteScale).Build(),
            new BounceBallCollisionEffect(_content.Load<Texture2D>("Environment/yellow_circle")),
            // Todo: Add a Circle Collider => The size of the sprite in game is 40x40
            // Todo: The player ball should bounce off these objects... Maybe changing the elasticity on the collider could help?
            new CircleCollider(20f, 3f)
        ]);
    }

    private GameObject.Template CreateBoxTileShapeTemplate(int width, int height,
        ConvexTilemapRuleTextures ruleTextures)
    {
        var components = new List<Component>();
        var dimensions = new Vector2Int(width, height);
        var bottomLeft = new Vector2(
            width % 2 != 0 ? -width / 2 * TileSize : (-width / 2 + 0.5f) * TileSize,
            height % 2 != 0 ? height / 2 * TileSize : (height / 2 - 0.5f) * TileSize
        );
        
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var texture = ruleTextures.GetTextureFromPositionAndDimensions(new Vector2Int(x, y), dimensions);
                var offset = bottomLeft + new Vector2(x * TileSize, -y * TileSize);
                components.Add(new SpriteRenderer.Builder(texture).WithCenterOffset(offset).WithScale(SpriteScale)
                    .Build());
            }
        }
        
        // Todo: Add a BoxCollider to the components. The Size should be "TileSize" * width, "TileSize" * height
        // We recommend reducing the total size by one pixel on each axis to avoid issues with the outside blocks colliding with each other
        components.Add(new BoxCollider(TileSize * width - 1, TileSize * height - 1, 1.5f));
        
        return new GameObject.Template(components);
    }
}
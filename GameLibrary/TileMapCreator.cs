using System.Collections.Generic;
using System.IO;
using GameLibrary.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameLibrary
{
    /// <summary>
    /// Creates a TileMap from a filestream and a Catalog.
    /// </summary>
    public static class TileMapCreator
    {
        private static readonly int tileWidth = 128;
        private static readonly int tileHeight = 128;
        private const int positionOffset = 100;

        public static void LoadLevel(string levelPath, ContentManager content)
        {
            using Stream fileStream = TitleContainer.OpenStream(levelPath);
            Dictionary<char, GameObject.Template> tileCatalog = PrepareTileCatalog(content);
            LoadTiles(fileStream, tileCatalog);
        }

        private static void LoadTiles(Stream fileStream, Dictionary<char, GameObject.Template> tileCatalog)
        {
            // Load the level and ensure all of the lines are the same length.
            List<string> lines = [];
            
            using (StreamReader reader = new(fileStream))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    lines.Add(line);
                    line = reader.ReadLine();
                }
            }

            // Loop over every tile position,
            for (int y = 0; y < lines.Count; ++y)
            {
                string line = lines[y];
                for (int x = 0; x < line.Length; ++x)
                {
                    // to load each tile.
                    char tileType = lines[y][x];
                    if (tileCatalog.TryGetValue(tileType, out var tileTemplate)){
                        tileTemplate.Instantiate(new(x * tileWidth + positionOffset, y * tileHeight + positionOffset));
                    }
                }
            }
        }

        private static Dictionary<char, GameObject.Template> PrepareTileCatalog(ContentManager content)
        {
            Dictionary<char, GameObject.Template> tileCatalog = new()
            {
                { '|', CreateTileTemplate(content.Load<Texture2D>("Tiles/RacingTileSet/road_asphalt01")) },
                { '=', CreateTileTemplate(content.Load<Texture2D>("Tiles/RacingTileSet/road_asphalt02")) },
                { 'N', CreateTileTemplate(content.Load<Texture2D>("Tiles/RacingTileSet/road_asphalt03")) },
                { 'E', CreateTileTemplate(content.Load<Texture2D>("Tiles/RacingTileSet/road_asphalt05")) },
                { 'S', CreateTileTemplate(content.Load<Texture2D>("Tiles/RacingTileSet/road_asphalt41")) },
                { 'W', CreateTileTemplate(content.Load<Texture2D>("Tiles/RacingTileSet/road_asphalt39")) }
            };
            
            return tileCatalog;
        }

        /// <summary>
        /// Creates a Tile template.
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        private static GameObject.Template CreateTileTemplate(Texture2D texture)
        {
            return new GameObject.Template([
                new SpriteRenderer(texture),
                new BoxCollider(tileWidth, tileHeight, 2.0f)
            ]);
        }
    }
}
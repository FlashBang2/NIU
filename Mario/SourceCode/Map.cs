using System.Linq;

namespace Mario
{
    internal class Map
    {
        public Tile[,] Tiles;

        public int CameraOffset;
        public int FlagDescend = 0;
        public int BumpAnimation;

        public static readonly int TileWidth = 48;
        public static readonly int TileHeight = 48;

        private System.Data.DataSet _set;
        private TextureManager.TextureInfo[] _textureData;

        public struct Tile
        {
            public int Value;
            public bool WasHit;

            public bool IsBottomSideDoors()
            {
                return Value == 28;
            }

            public bool IsQuestionMarkPlacedHere()
            {
                return Value == 2;
            }

            public bool IsEmpty()
            {
                return Value == 0;
            }

            public bool IsLeftSideOfFlagPlacedHere()
            {
                return Value == 24;
            }

            public bool IsFlagPolePlacedHere()
            {
                return Value == 23;
            }

            public bool IsRightSideOfFlagPlacedHere()
            {
                return Value == 25;
            }

            public bool ShouldRenderAsSolidBlock()
            {
                return !IsEmpty() && !IsQuestionMarkPlacedHere() &&
                        !IsLeftSideOfFlagPlacedHere() && !IsRightSideOfFlagPlacedHere() &&
                        !WasHit;
            }
        }

        public Map(string pathToMapFile)
        {
            var xml = new System.Xml.XmlDocument();
            xml.Load(pathToMapFile);
            _set = new System.Data.DataSet();
            _set.ReadXml(new System.IO.StringReader(xml.InnerXml));

            _textureData = new TextureManager.TextureInfo[_set.Tables["texture"].Rows.Count];
            int textureIndex = 0;

            foreach (System.Data.DataRow row in _set.Tables["texture"].Rows)
            {
                _textureData[textureIndex] = TextureManager.LoadTexture(row[1].ToString());
                textureIndex++;
            }

            LoadMap();
        }

        private void LoadMap()
        {
            Tiles = new Tile[_set.Tables["row"].Rows.Count, _set.Tables["row"].Rows[0].GetChildRows("row_column").Count()];
            int rowIndex = 0;

            foreach (System.Data.DataRow row in _set.Tables["row"].Rows)
            {
                int columnIndex = 0;

                foreach (System.Data.DataRow innerRow in row.GetChildRows("row_column"))
                {
                    Tiles[rowIndex, columnIndex].Value = int.Parse(innerRow[1].ToString());
                    Tiles[rowIndex, columnIndex].WasHit = false;
                    columnIndex++;
                }

                rowIndex++;
            }
        }

        public bool IsSideDoorOnTile(int x, int y)
        {
            return Tiles[x, y].IsBottomSideDoors();
        }

        private void UpdateCameraOffset()
        {
            bool shouldUpdateCameraOffset = CameraOffset >= 0 && CameraOffset < Tiles.GetLength(1) * TileWidth - App.ScreenWidth;

            if (shouldUpdateCameraOffset)
            {
                CameraOffset += Game.ScrollSpeed;

                if (CameraOffset < 0)
                {
                    CameraOffset = 0;
                }
                else if (CameraOffset > Tiles.GetLength(1) * TileWidth - App.ScreenWidth)
                {
                    CameraOffset = (Tiles.GetLength(1) * TileWidth - App.ScreenWidth) - 1;
                }
            }
        }

        public void UpdateMap()
        {
            UpdateCameraOffset();
        }

        public void DrawMap()
        {
            for (int rowIndex = 0; rowIndex < Tiles.GetLength(0); rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < Tiles.GetLength(1); columnIndex++)
                {
                    RenderTileAt(rowIndex, columnIndex);
                }
            }
        }

        private void RenderTileAt(int rowIndex, int columnIndex)
        {
            ref Tile currentRendererdTile = ref Tiles[rowIndex, columnIndex];

            if (currentRendererdTile.WasHit)
            {
                RenderHitBlock(rowIndex, columnIndex);
            }

            if (currentRendererdTile.ShouldRenderAsSolidBlock())
            {
                TextureManager.DrawTexture(_textureData[currentRendererdTile.Value - 1], 
                    columnIndex * TileWidth - CameraOffset, rowIndex * TileHeight, 1);
            }
            if (currentRendererdTile.IsQuestionMarkPlacedHere() && !currentRendererdTile.WasHit)
            {
                TextureManager.DrawTexture(_textureData[currentRendererdTile.Value - 1], columnIndex * TileWidth - CameraOffset, rowIndex * TileHeight, 3);
            }

            if (currentRendererdTile.IsLeftSideOfFlagPlacedHere() || currentRendererdTile.IsRightSideOfFlagPlacedHere())
            {
                AnimateFlag(rowIndex, columnIndex);
            }
        }

        private void RenderHitBlock(int rowIndex, int columnIndex)
        {
            int frames = 1;
            ref Tile currentRendererdTile = ref Tiles[rowIndex, columnIndex];

            if (currentRendererdTile.IsQuestionMarkPlacedHere())
            {
                frames = 3;
            }

            UpdateHitAnimationOfBlockAt(rowIndex, columnIndex, frames);
        }

        private void AnimateFlag(int rowIndex, int columnIndex)
        {
            ref Tile currentRendererdTile = ref Tiles[rowIndex, columnIndex];

            if (Game._player.IsWinning)
            {
                bool hasFlagDescendLowEnough = rowIndex * TileHeight + FlagDescend >= 816;

                if (!hasFlagDescendLowEnough)
                {
                    FlagDescend += 2;
                }
                if (currentRendererdTile.IsRightSideOfFlagPlacedHere())
                {
                    TextureManager.DrawTexture(_textureData[23 - 1], columnIndex * TileWidth - CameraOffset, rowIndex * TileHeight, 1);
                }
            }

            TextureManager.DrawTexture(_textureData[currentRendererdTile.Value - 1], columnIndex * TileWidth - CameraOffset, rowIndex * TileHeight + FlagDescend, 1);
        }

        private void UpdateHitAnimationOfBlockAt(int rowIndex, int columnIndex, int frames)
        {
            if (BumpAnimation > 0)
            {
                BumpAnimation -= 4;
            }
            if (BumpAnimation == 0)
            {
                Tiles[rowIndex, columnIndex].WasHit = false;
            }

            TextureManager.DrawTexture(_textureData[Tiles[rowIndex, columnIndex].Value - 1], columnIndex * TileWidth - CameraOffset, rowIndex * TileHeight - BumpAnimation, frames);
        }

        public void CleanMapTexture()
        {
            for (int i = 0; i < _textureData.Length; i++)
            {
                SDL2.SDL.SDL_DestroyTexture(_textureData[i].Texture);
            }
        }
    }
}

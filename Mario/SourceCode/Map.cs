using SDL2;
using System.Linq;

namespace Mario
{
    internal class Map
    {
        public int[,] data;
        public int cameraOffset;
        private System.Data.DataSet _set;
        private TextureManager.TextureInfo[] textureData;

        public Map(string path)
        {
            System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
            xml.Load(path);
            _set = new System.Data.DataSet();
            _set.ReadXml(new System.IO.StringReader(xml.InnerXml));

            textureData = new TextureManager.TextureInfo[_set.Tables["texture"].Rows.Count];
            int index = 0;

            foreach (System.Data.DataRow row in _set.Tables["texture"].Rows)
            {
                textureData[index] = TextureManager.LoadTexture(row[1].ToString());
                index++;
            }

            LoadMap();
        }

        public void LoadMap()
        {
            data = new int[_set.Tables["row"].Rows.Count, _set.Tables["row"].Rows[0].GetChildRows("row_column").Count()];
            int rowIndex = 0, columnIndex = 0;

            foreach (System.Data.DataRow row in _set.Tables["row"].Rows)
            {
                columnIndex = 0;
                foreach (System.Data.DataRow innerRow in row.GetChildRows("row_column"))
                {
                    data[rowIndex, columnIndex] = int.Parse(innerRow[1].ToString());
                    columnIndex++;
                }
                rowIndex++;
            }

        }

        public void UpdateCameraOffset()
        {
            if (cameraOffset >= 0 && cameraOffset < data.GetLength(1) * 48 - App.screenWidth)
            {
                cameraOffset += Game.ScrollSpeed;
                if (cameraOffset < 0)
                {
                    cameraOffset = 0;
                }
                if (cameraOffset > data.GetLength(1) * 48 - App.screenWidth)
                {
                    cameraOffset = (data.GetLength(1) * 48 - App.screenWidth) - 1;
                }
            }
        }

        public void DrawMap()
        {
            for (int rowIndex = 0; rowIndex < data.GetLength(0); rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < data.GetLength(1); columnIndex++)
                {
                    if (data[rowIndex, columnIndex] != 0 && data[rowIndex, columnIndex] != 2)
                    {
                        TextureManager.DrawTexture(textureData[data[rowIndex, columnIndex] - 1], columnIndex * 48 - cameraOffset, rowIndex * 48, 1);
                    }
                    if (data[rowIndex, columnIndex] == 2)
                    {
                        TextureManager.DrawTexture(textureData[data[rowIndex, columnIndex] - 1], columnIndex * 48 - cameraOffset, rowIndex * 48, 3);
                    }
                }
            }
        }

        public void CleanMapTexture()
        {
            for (int i = 0; i < textureData.Length; i++)
            {
                SDL.SDL_DestroyTexture(textureData[i].Texture);
            }
        }
        
    }
}

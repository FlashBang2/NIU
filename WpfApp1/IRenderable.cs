using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfApp1
{
    public enum RenderMode
    {
        Sprite,
        Rect,
        FilledRect,
        Circle,
        FilledCircle,
        Line,
        Point
    }

    public interface IRenderable
    {
        bool ShouldDraw { get; }

        string SpriteTextureId { get; }

        int ZIndex { get; set; }

        Rect Bounds { get; set; }

        double PosX { get; set; }
        double PosY { get; set; }

        /// <summary>
        /// Applied only to sprite. Rotation of the sprite in degrees
        /// </summary>
        double RotationAngle { get; set; }

        double CircleRadius { get; set; }

        /// <summary>
        /// Color that will be applied to proxy shapes such as rectangle, cirle, line or point
        /// </summary>
        Color ProxyShapeColor { get; set; }

        /// <summary>
        /// Portion of the texture that will be copied to backbuffer during rendering
        /// </summary>
        Rect SourceTextureBounds { get; set; }

        bool IsVisible(Rect cameraBounds);
        object LinkedEntity { get; }

        RenderMode RenderingMode { get; }

        event Action<int> ZIndexChanged;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace WpfApp1
{
    public class NullRenderable : IRenderable
    {
        private readonly IEntity _entity;

        public NullRenderable(IEntity entity)
        {
            _entity = entity;
        }

        public bool ShouldDraw => false;

        public string SpriteTextureId => "";

        public int ZIndex { get =>0; set => throw new NotImplementedException(); }
        public Rect Bounds { get => new Rect(new Vector(), new Vector()); set => throw new NotImplementedException(); }
        public double PosX { get => 0; set => throw new NotImplementedException(); }
        public double PosY { get => 0; set => throw new NotImplementedException(); }
        public double RotationAngle { get => 0; set => throw new NotImplementedException(); }
        public double CircleRadius { get => 0; set => throw new NotImplementedException(); }
        public Color ProxyShapeColor { get => Color.FromRgb(0, 0, 0); set => throw new NotImplementedException(); }
        public Rect SourceTextureBounds { get => Bounds; set => throw new NotImplementedException(); }

        public object LinkedEntity => _entity;

        public RenderMode RenderingMode => RenderMode.Rect;

        public event Action<int> ZIndexChanged;

        public bool IsVisible(Rect cameraBounds)
        {
            return false;
        }
    }
}

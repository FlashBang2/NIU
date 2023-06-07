using System;

namespace WpfApp1
{
    public partial class SDLApp
    {
        class Sprite : Component, IRenderable
        {
            public double RotationAngle { get => 0; set => throw new NotImplementedException(); }

            bool IRenderable.ShouldDraw => true;

            Rect IRenderable.SourceTextureBounds { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public string spriteId = "firstPlatform";

            public override void ReceiveRender()
            {
                base.ReceiveRender();

                SDLRendering.DrawSprite(spriteId, Owner.Bounds, Rect.Unlimited, RotationAngle);
            }
        }
    }
}

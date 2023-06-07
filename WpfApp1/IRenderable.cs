namespace WpfApp1
{
    public interface IRenderable
    {
        bool ShouldDraw { get; }

        /// <summary>
        /// Applied only to sprite. Rotation of the sprite in degrees
        /// </summary>
        double RotationAngle { get; set; }

        /// <summary>
        /// Portion of the texture that will be copied to backbuffer during rendering
        /// </summary>
        Rect SourceTextureBounds { get; set; }
    }
}

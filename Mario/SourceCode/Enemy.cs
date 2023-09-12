using SDL2;

namespace Mario
{
    internal class Enemy : GameObject
    {
        public Enemy(string path, int positionX, int positionY, int frames) : base(path, positionX, positionY, frames) { }

        public override void Update()
        {
           base.Update();
           surface = App.AssignValuesForRectangle((textureInfo.Width / _frames) * (int)((SDL.SDL_GetTicks() / 300) % _frames), 0, textureInfo.Width / _frames, textureInfo.Height);
           destination = App.AssignValuesForRectangle(_positionX - Game._CurrentLevel.cameraOffset, _positionY, textureInfo.Width / _frames, textureInfo.Height);
        }
    }
}

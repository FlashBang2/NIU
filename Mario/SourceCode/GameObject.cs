namespace Mario
{
    internal class GameObject
    {
        public int _positionX, _positionY, _frames, velocityX, velocityY;
        public SDL2.SDL.SDL_RendererFlip flipFlag = SDL2.SDL.SDL_RendererFlip.SDL_FLIP_NONE;

        public bool IsTouchingGround = true;
        public bool IsWinning = false;
        public bool IsReseting = false;
        public bool IsEnding = false;
        public bool IsDying = false;
        public bool HasReachedPit = false;
        public bool HasLost = false;

        protected SDL2.SDL.SDL_Rect _surface, _destination;
        protected TextureManager.TextureInfo _textureInfo;
        protected SDL2.SDL.SDL_Point _point;

        public GameObject(string path, int positionX, int positionY, int frames)
        {
            _frames = frames;
            _positionX = positionX;
            _positionY = positionY;
            _point.x = 0;
            _point.y = 0;
            _textureInfo = TextureManager.LoadTexture(path);
        }

        public virtual void Update() {}

        public virtual void UpdateAnimation () { }
        public void Render ()
        {
            SDL2.SDL.SDL_RenderCopyEx(Game.Renderer, _textureInfo.Texture, ref _surface, ref _destination, 0, ref _point, flipFlag);
        }

        public void Clean()
        {
            SDL2.SDL.SDL_DestroyTexture(_textureInfo.Texture);
        }

        public TextureManager.TextureInfo GetTextureInfo()
        {
            return _textureInfo;
        }
    }
}

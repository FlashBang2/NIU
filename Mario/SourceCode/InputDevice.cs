namespace Mario
{
    public interface InputDevice
    {
        bool IsRightActionPressed { get; }
        bool IsLeftActionPressed { get; }
        bool IsJumpActionPressed { get; }

        void UpdateByEvent(ref SDL2.SDL.SDL_Event evt);

        void Cleanup();
    }


    public class Joystick : InputDevice
    {
        private bool _isRightActionPressed = false;
        private bool _isLeftActionPressed = false;
        private bool _isJumpActionPressed = false;

        // _joystick must be kept, because it would crash, if any of the unmanaged code use it
        private System.IntPtr _joystick;

        public Joystick()
        {
            _joystick = SDL2.SDL.SDL_JoystickOpen(0);
        }

        public bool IsRightActionPressed => _isRightActionPressed;
        public bool IsLeftActionPressed => _isLeftActionPressed;
        public bool IsJumpActionPressed => _isJumpActionPressed;

        public void UpdateByEvent(ref SDL2.SDL.SDL_Event evt)
        {

            if (Game._inMainMenu)
            {
                if (ShouldStartGame(evt))
                {
                    StartPlayingGame();
                }

                return;
            }

            if (!IsJoystickEvent(ref evt))
            {
                return;
            }

            if (evt.type == SDL2.SDL.SDL_EventType.SDL_JOYHATMOTION)
            {
                UpdateMovementButtons(ref evt);
                return;
            }


            SDL2.SDL.SDL_GameControllerButton button = (SDL2.SDL.SDL_GameControllerButton)evt.jbutton.button;
            switch (button)
            {
                case SDL2.SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X:
                    TryUpdateJumpButton(ref evt);
                    break;
            }
        }

        public void Cleanup()
        {
            SDL2.SDL.SDL_JoystickClose(_joystick);
        }

        private static bool ShouldStartGame(SDL2.SDL.SDL_Event evt)
        {
            return evt.type == SDL2.SDL.SDL_EventType.SDL_JOYBUTTONDOWN && evt.jbutton.button == (byte)SDL2.SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START;
        }

        private void UpdateMovementButtons(ref SDL2.SDL.SDL_Event evt)
        {
            if (evt.jhat.hatValue == SDL2.SDL.SDL_HAT_LEFT)
            {
                _isLeftActionPressed = true;
                _isRightActionPressed = false;
            }
            else if (evt.jhat.hatValue == SDL2.SDL.SDL_HAT_RIGHT)
            {
                _isRightActionPressed = true;
                _isLeftActionPressed = false;
            }
            else if (evt.jhat.hatValue == SDL2.SDL.SDL_HAT_CENTERED)
            {
                _isRightActionPressed = false;
                _isLeftActionPressed = false;
            }
        }

        private static bool IsJoystickEvent(ref SDL2.SDL.SDL_Event evt)
        {
            return evt.type == SDL2.SDL.SDL_EventType.SDL_JOYBUTTONDOWN || evt.type == SDL2.SDL.SDL_EventType.SDL_JOYBUTTONUP ||
                                evt.type == SDL2.SDL.SDL_EventType.SDL_JOYHATMOTION;
        }

        private static void StartPlayingGame()
        {
            Game._inMainMenu = false;
            SDL2.SDL_mixer.Mix_OpenAudio(44100, SDL2.SDL_mixer.MIX_DEFAULT_FORMAT, 2, 2048);
            Game.gameMusic = SDL2.SDL_mixer.Mix_LoadWAV("Assets/Music/OverworldTheme.wav");
            SDL2.SDL_mixer.Mix_Volume(-1, 20);
            SDL2.SDL_mixer.Mix_PlayChannel(-1, Game.gameMusic, -1);
        }

        private void TryUpdateJumpButton(ref SDL2.SDL.SDL_Event evt)
        {
            if (Game._player.IsTouchingGround)
            {
                _isJumpActionPressed = evt.type == SDL2.SDL.SDL_EventType.SDL_JOYBUTTONDOWN;
            }
            else
            {
                _isJumpActionPressed = false;
            }
        }
    }


    class Keyboard : InputDevice
    {
        private bool _isRightActionPressed = false;
        private bool _isLeftActionPressed = false;
        private bool _isJumpActionPressed = false;

        public bool IsRightActionPressed => _isRightActionPressed;

        public bool IsLeftActionPressed => _isLeftActionPressed;

        public bool IsJumpActionPressed => _isJumpActionPressed;

        public void Cleanup()
        {
        }

        public void UpdateByEvent(ref SDL2.SDL.SDL_Event evt)
        {
            if (Game._inMainMenu)
            {
                return;
            }

            if (evt.type == SDL2.SDL.SDL_EventType.SDL_KEYDOWN)
            {
                UpdateKeyDownStates(ref evt);
            }
            else if (evt.type == SDL2.SDL.SDL_EventType.SDL_KEYUP)
            {
                UpdateKeyUpStates(ref evt);
            }
        }

        private void UpdateKeyUpStates(ref SDL2.SDL.SDL_Event evt)
        {
            switch (evt.key.keysym.sym)
            {
                case SDL2.SDL.SDL_Keycode.SDLK_w:
                    _isJumpActionPressed = false;
                    break;
                case SDL2.SDL.SDL_Keycode.SDLK_a:
                    _isLeftActionPressed = false;
                    break;
                case SDL2.SDL.SDL_Keycode.SDLK_d:
                    _isRightActionPressed = false;
                    break;
                case SDL2.SDL.SDL_Keycode.SDLK_s:
                    Game.Controls.isPressingS = false;
                    break;
            }
        }

        private void UpdateKeyDownStates(ref SDL2.SDL.SDL_Event evt)
        {
            switch (evt.key.keysym.sym)
            {
                case SDL2.SDL.SDL_Keycode.SDLK_w:
                    if (Game._player.IsTouchingGround)
                    {
                        _isJumpActionPressed = true;
                    }
                    break;
                case SDL2.SDL.SDL_Keycode.SDLK_d:
                    _isRightActionPressed = true;
                    break;
                case SDL2.SDL.SDL_Keycode.SDLK_a:
                    _isLeftActionPressed = true;
                    break;
                case SDL2.SDL.SDL_Keycode.SDLK_s:
                    Game.Controls.isPressingS = true;
                    break;
                case SDL2.SDL.SDL_Keycode.SDLK_LSHIFT:
                    Game.Controls.isPressingShift = true;
                    break;
                case SDL2.SDL.SDL_Keycode.SDLK_RSHIFT:
                    Game.Controls.isPressingShift = true;
                    break;
            }
        }
    }   
}

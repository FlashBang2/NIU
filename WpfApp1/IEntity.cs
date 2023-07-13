using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static SDL2.SDL;

namespace WpfApp1
{
    public interface IEntity
    {
        float posX { get; set; }
        float posY { get; set; }

        /// <summary>
        /// Approximate bounds width
        /// </summary>
        float width { get; set; }
        float height { get; set; }

        string name { get; set; }
        SDL_Rect bounds { get; }

        void AddWorldOffset(float x, float y);

        bool isActive { get; set; }

        void SetActive(bool active);

        void Destroy();

        void Tick(float dt);

        T GetComponent<T>();
        bool HasComponent<T>();

        void AddComponent<T>() where T : Component;

        void ReceiveRender();

        void UndoLastTranslation();

        void AddToTickList(Component component);
        void RemoveFromTickList(Component component);
    }
}

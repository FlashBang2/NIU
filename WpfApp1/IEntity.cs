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
        float PosX { get; set; }
        float PosY { get; set; }

        /// <summary>
        /// Approximate bounds width
        /// </summary>
        float Width { get; set; }
        float Height { get; set; }

        string Name { get; set; }
        SDL_Rect Bounds { get; }

        void AddWorldOffset(float x, float y);

        void Rotate(float angle);

        void AttachChild(IEntity entity);
        bool RemoveChild(IEntity entity);

        bool IsActive { get; set; }

        void SetActive(bool active);

        void Destroy();
        IEntity FindChild(string name);

        void Tick(float dt);

        T GetComponent<T>();
        bool HasComponent<T>();

        void AddComponent<T>() where T : Component;

        void ReceiveRender();

        IEntity[] GetChildren();
        void UndoLastTranslation();

        void AddToTickList(Component component);
        void RemoveFromTickList(Component component);
    }
}

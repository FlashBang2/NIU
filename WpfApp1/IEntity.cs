using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public interface IEntity
    {
        double PosX { get; set; }
        double PosY { get; set; }

        /// <summary>
        /// Approximate bounds width
        /// </summary>
        double Width { get; set; }

        string Name { get; set; }

        /// <summary>
        /// Approximate bounds height
        /// </summary>
        double Height { get; set; }

        Vector Right { get; }
        Vector Left { get; }

        Vector Up { get; }
        Vector Down { get; }

        Rect Bounds { get; }

        void AddLocalOffset(double x, double y);
        void AddWorldOffset(double x, double y);

        void Rotate(double angle);

        void AttachChild(IEntity entity);
        bool RemoveChild(IEntity entity);

        bool IsActive { get; set; }

        void Destroy();
        IEntity FindChild(string name);

        void Tick(double dt);

        T GetComponent<T>();
        bool HasComponent<T>();

        void AddComponent<T>() where T : Component;

        void ReceiveRender();

        IEntity[] GetChildren();
        void UndoLastTranslation();
    }
}

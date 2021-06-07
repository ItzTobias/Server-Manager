using System.Windows;

namespace Server_Manager.Scripts
{
    public interface IHasTopMenuItems
    {
        public UIElement[] Items { get; }
    }
}

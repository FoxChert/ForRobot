using System;
using System.Linq;
using System.Windows.Interactivity;

using GalaSoft.MvvmLight.Messaging;
using AvalonDock;
using AvalonDock.Layout;

namespace ForRobot.Libr.Behavior
{
    /// <summary>
    /// Класс поведения <see cref="DockingManager"/>
    /// </summary>
    public class DockingManagerBehavior : Behavior<DockingManager>
    {
        private DockingManager _dockingManager = null;

        protected override void OnAttached()
        {
            base.OnAttached();
            Messenger.Default.Register<ForRobot.Libr.Messages.LayoutAnchorableMessage>(this, message => this.ChangedLayoutAnchorableVisible(message.ContentId));
            this._dockingManager = base.AssociatedObject;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            Messenger.Default.Unregister< ForRobot.Libr.Messages.LayoutAnchorableMessage> (this);
        }

        /// <summary>
        /// Изменение видимости <see cref="LayoutAnchorable"/>
        /// </summary>
        /// <param name="contentId">Id <see cref="LayoutContent"/></param>
        public void ChangedLayoutAnchorableVisible(string contentId)
        {
            var panel = _dockingManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == contentId);
            panel.IsVisible = !panel.IsVisible;
        }
    }
}

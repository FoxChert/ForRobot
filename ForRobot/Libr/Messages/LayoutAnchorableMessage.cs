using System;

namespace ForRobot.Libr.Messages
{
    /// <summary>
    /// Сообщение передачи ContentId <see cref="AvalonDock.Layout.LayoutAnchorable"/>
    /// </summary>
    public class LayoutAnchorableMessage
    {
        public string ContentId { get; private set; }

        public LayoutAnchorableMessage(string contentId)
        {
            this.ContentId = contentId;
        }
    }
}

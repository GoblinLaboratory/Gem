﻿using System;

namespace Gem.Gui.Controls
{
    public class Control<TEventArgs> : IControl<TEventArgs> 
        where TEventArgs : EventArgs
    {

        private readonly object sender;
        private readonly Func<TEventArgs> eventArgsProvider;

        public Control(object sender, Func<TEventArgs> eventArgsProvider)
        {
            this.sender = sender;
            this.eventArgsProvider = eventArgsProvider;
        }

        #region Events

        public event EventHandler<TEventArgs> LostFocus;

        public event EventHandler<TEventArgs> GotFocus;

        public event EventHandler<TEventArgs> Clicked;

        public event EventHandler<TEventArgs> GotMouseCapture;

        public event EventHandler<TEventArgs> LostMouseCapture;
        
        public event EventHandler<TEventArgs> DragEnter;

        public event EventHandler<TEventArgs> DragLeave;

        #endregion

        #region Aggregation

        internal void OnMouseCapture()
        {
            var handler = GotMouseCapture;
            if (handler != null)
            {
                handler(sender, eventArgsProvider());
            }
        }

        internal void OnLostMouseCapture()
        {
            var handler = LostMouseCapture;
            if (handler != null)
            {
                handler(sender, eventArgsProvider());
            }
        }

        internal void OnGotFocus()
        {
            var handler = GotFocus;
            if (handler != null)
            {
                handler(sender, eventArgsProvider());
            }
        }

        internal void OnLostFocus()
        {
            var handler = LostFocus;
            if (handler != null)
            {
                handler(sender, eventArgsProvider());
            }
        }

        internal void OnClicked()
        {
            var handler = Clicked;
            if (handler != null)
            {
                handler(sender, eventArgsProvider());
            }
        }

        #endregion

    }
}

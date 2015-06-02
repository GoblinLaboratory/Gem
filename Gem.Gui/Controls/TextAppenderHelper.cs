﻿using Gem.Gui.Input;
using Gem.Infrastructure.Input;
using Microsoft.Xna.Framework.Input;
using System;

namespace Gem.Gui.Controls
{
    public class TextAppenderHelper
    {
        private readonly KeyboardInput input;
        private readonly char cursor;

        public TextAppenderHelper(KeyboardInput input,
                                  char cursor = '|',
                                  double cursorFlickInterval = 500.0d,
                                  double keyRepeatStartDuration = 0.5d,
                                  double keyRepeatDuration = 0.04d)
        {
            this.input = input;
            this.KeyRepeatDuration = keyRepeatDuration;
            this.KeyRepeatStartDuration = keyRepeatStartDuration;
            this.ShouldHandleKey = (key,charRepresentation) => true;
            this.cursor = cursor;
            this.CursorFlickInterval = cursorFlickInterval;
        }

        public double KeyRepeatStartDuration { get; set; }
        public double KeyRepeatDuration { get; set; }
        public double KeyRepeatTimer { get; set; }
        public double CursorFlickInterval { get; set; }

        public Func<Keys,char,bool> ShouldHandleKey { get; set; }

        public KeyboardInput Input { get { return input; } }

        public char Cursor { get { return cursor; } }

        public static TextAppenderHelper Default
        {
            get
            {
                return new TextAppenderHelper(InputManager.Keyboard);
            }
        }
    }
}

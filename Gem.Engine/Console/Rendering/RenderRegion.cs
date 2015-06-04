﻿using System;
using Microsoft.Xna.Framework;

namespace Gem.Console.Rendering
{
    public class RenderRegion
    {
        private readonly Vector2 position;
        private readonly Vector2 size;
        private readonly Rectangle rectangle;

        public RenderRegion(Vector2 position, Vector2 size)
        {
            this.position = position;
            this.size = size;
            this.rectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        }

        public Vector2 Size { get { return size; } }
        public Vector2 Position { get { return position; } }
        public Rectangle Rectangle { get { return rectangle; } }
    }
}

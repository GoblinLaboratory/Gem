﻿using System;

namespace Gem.Engine.AI.FiniteStateMachine.Visualization
{

    public class StateVisualizationException : Exception
    {
        public StateVisualizationException()
        {
        }

        public StateVisualizationException(string message)
            : base(message)
        {
        }

        public StateVisualizationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

﻿using System;
using System.ComponentModel.Composition;
using Gemini.Framework;
using Gem.IDE.Modules.SpriteSheets.Views;
using Gem.DrawingSystem.Animations;

namespace Gem.IDE.Modules.SpriteSheets.ViewModels
{
    [Export(typeof(AnimationStripViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AnimationStripViewModel : Document
    {
        private ISceneView sceneView;

        public override bool ShouldReopenOnStart
        {
            get { return true; }
        }

        private AnimationStripSettings settings =>
            new AnimationStripSettings(
                FrameWidth,
                FrameHeight, 
                RelateiveToPathTexture, 
                Name, 
                FrameDelay,
                FirstFrame,
                LastFrame);

        private int frameWidth = 130;
        public int FrameWidth
        {
            get { return frameWidth; }
            set
            {
                frameWidth = value;
                NotifyOfPropertyChange(() => FrameWidth);
                sceneView?.Invalidate(settings);
            }
        }

        private int frameHeight = 150;
        public int FrameHeight
        {
            get { return frameHeight; }
            set
            {
                frameHeight = value;
                NotifyOfPropertyChange(() => FrameHeight);
                sceneView?.Invalidate(settings);
            }
        }

        private string relateiveToPathTexture;
        public string RelateiveToPathTexture
        {
            get { return relateiveToPathTexture; }
            set
            {
                relateiveToPathTexture = value;
                NotifyOfPropertyChange(() => RelateiveToPathTexture);
                sceneView?.Invalidate(settings);
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyOfPropertyChange(() => Name);
                sceneView?.Invalidate(settings);
            }
        }

        private int firstFrame = 0;
        public int FirstFrame
        {
            get { return firstFrame; }
            set
            {
                firstFrame = value;
                NotifyOfPropertyChange(() => FirstFrame);
                sceneView?.Invalidate(settings);
            }
        }

        private int lastFrame = -1;
        public int LastFrame
        {
            get { return lastFrame; }
            set
            {
                lastFrame = value;
                NotifyOfPropertyChange(() => LastFrame);
                sceneView?.Invalidate(settings);
            }
        }

        private int frameDelay = 300;
        public int FrameDelay
        {
            get { return frameDelay; }
            set
            {
                frameDelay = value;
                NotifyOfPropertyChange(() => FrameDelay);
                sceneView?.Invalidate(settings);
            }
        }

        public AnimationStripViewModel()
        {
            DisplayName = "SpriteSheets";
        }

        protected override void OnViewLoaded(object view)
        {
            sceneView = view as ISceneView;
            base.OnViewLoaded(view);
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                var view = GetView() as IDisposable;
                if (view != null)
                    view.Dispose();
            }

            base.OnDeactivate(close);
        }
    }
}
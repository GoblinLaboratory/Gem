﻿using Gem.Gui.Controls;
using Gem.Gui.Elements.Areas;
using Gem.Gui.Layout;
using Gem.Gui.Rendering;
using System;
using System.Collections.Generic;

namespace Gem.Gui.Elements
{

    public class GuiArea : AGuiComponent, IGuiArea
    {
        private readonly IDictionary<int, GuiAreaEntry> elements = new Dictionary<int, GuiAreaEntry>();


        public GuiArea(RenderTemplate renderTemplate,
                             Alignment layoutStyle,
                             Region region,
                             ControlTarget target,
                             IGuiComponent parent = null)
            : base(renderTemplate, layoutStyle, region, target, parent)
        { }

        public int AddComponent(IGuiComponent element)
        {
            int id = elements.Count;
            elements.Add(id, new GuiAreaEntry(element));

            return id;
        }

        public bool RemoveComponent(int id)
        {
            return elements.Remove(id);
        }

        public IGuiComponent this[int id]
        {
            get
            {
                return elements.ContainsKey(id) ?
                elements[id].Component : null;
            }
        }

        public int ComponentCount
        {
            get { return elements.Count; }
        }

        public IEnumerable<GuiAreaEntry> Entries()
        {
            foreach (var element in elements.Values)
            {
                yield return element;
            }
        }

        public event EventHandler<ElementEventArgs> OnAddComponent;

        public event EventHandler<ElementEventArgs> OnRemoveComponent;


        public Aggregation.ComponentIndex FocusIndex
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }

}

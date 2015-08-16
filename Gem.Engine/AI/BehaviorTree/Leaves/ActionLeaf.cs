﻿using System;
using System.Collections.Generic;

namespace Gem.AI.BehaviorTree.Leaves
{
    public class ActionLeaf<AIContext> : ILeaf<AIContext>
    {
        private Func<AIContext, BehaviorResult> behaveDelegate;
        public event EventHandler OnBehaved;

        public ActionLeaf(Func<AIContext, BehaviorResult> processedBehavior)
        {
            behaveDelegate = processedBehavior;
        }

        public IEnumerable<IBehaviorNode<AIContext>> SubNodes
        { get { yield break; } }

        public ActionLeaf(Func<AIContext, BehaviorResult> initialBehavior,
                          Func<AIContext, BehaviorResult> processedBehavior)
        {
            behaveDelegate = context =>
            {
                behaveDelegate = processedBehavior;
                return initialBehavior(context);
            };
        }

        public string Name { get; set; } = string.Empty;
        public BehaviorResult Behave(AIContext context)
        {
            var result =  behaveDelegate(context);
            OnBehaved?.Invoke(this, new BehaviorInvokationEventArgs(result));
            return result;
        }
    }
}

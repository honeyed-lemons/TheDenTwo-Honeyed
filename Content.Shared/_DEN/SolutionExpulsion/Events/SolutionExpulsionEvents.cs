using Content.Shared._DEN.SolutionExpulsion.Components;
using Content.Shared._DEN.SolutionExpulsion.EntitySystems;
using Content.Shared.DoAfter;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._DEN.SolutionExpulsion.Events;

public sealed partial class SolutionExpulsionEvents
{
    /// <summary>
    /// When this event is called, an expellable solution is added to the entity it's called on.
    /// Do not call this directly, and instead use <see cref="SolutionExpulsionSystem.AddExpellableSolution"/>
    /// </summary>
    /// <param name="expellableSolutionPrototype">Expellable solution prototype to add.</param>
    [Serializable, NetSerializable]
    public sealed class SolutionExpellableAdded(EntProtoId expellableSolutionPrototype) : EntityEventArgs
    {
        public EntProtoId ExpellableSolutionPrototype { get; } = expellableSolutionPrototype;
    }

    /// <summary>
    /// When this event is called, an expellable solution is removed from the entity it's called on.
    /// Do not call this directly, and instead use <see cref="SolutionExpulsionSystem.RemoveExpellableSolution"/>
    /// </summary>
    /// <param name="expellableSolutionPrototype">Expellable solution prototype to remove.</param>
    [Serializable, NetSerializable]
    public sealed class SolutionExpellableRemoved(EntProtoId expellableSolutionPrototype) : EntityEventArgs
    {
        public EntProtoId ExpellableSolutionPrototype { get; } = expellableSolutionPrototype;
    }

    /// <summary>
    /// Classic doafter event, called when attempting to fill a solution container with an expellable solution.
    /// </summary>
    [Serializable, NetSerializable]
    public sealed partial class SolutionExpulsionFillEvent : DoAfterEvent
    {
        /// <summary>
        /// Solution entity to fill with.
        /// </summary>
        public NetEntity ExpellableSolution;

        public SolutionExpulsionFillEvent(NetEntity expellableSolution)
        {
            ExpellableSolution = expellableSolution;
        }

        public override DoAfterEvent Clone()
        {
            return this;
        }
    }
}

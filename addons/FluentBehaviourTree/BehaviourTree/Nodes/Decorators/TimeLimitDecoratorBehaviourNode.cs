using BehaviourTree.FluentBuilder;
using Godot;
namespace fluent_behaviour_tree.addons.FluentBehaviourTree.BehaviourTree.Nodes.Decorators;

/**
 * Decorator has N time in milliseconds for it's child nodes to complete, otherwise, fail the nodes
 */
[Tool]
[GlobalClass]
public partial class TimeLimitDecoratorBehaviourNode : DecoratorBehaviourNode {

    [Export]
    public int timeToCompleteMilliseconds;

    public override void BuildNode(FluentBuilder<GodotBehaviourContext> builder) {
        builder.TimeLimit(Name, timeToCompleteMilliseconds);
    }
}

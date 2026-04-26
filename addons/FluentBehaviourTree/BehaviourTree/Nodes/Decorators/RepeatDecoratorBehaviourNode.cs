using BehaviourTree.FluentBuilder;
using Godot;
namespace fluent_behaviour_tree.addons.FluentBehaviourTree.BehaviourTree.Nodes.Decorators;

/**
 * Repeat the child node N times, as given by node input
 */
[Tool]
[GlobalClass]
public partial class RepeatDecoratorBehaviourNode : DecoratorBehaviourNode {

    [Export]
    public required int repeatTimes = 5;

    public override void BuildNode(FluentBuilder<GodotBehaviourContext> builder) {
        builder.Repeat(Name, repeatTimes);
    }
}

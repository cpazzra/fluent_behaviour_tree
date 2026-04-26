using BehaviourTree.FluentBuilder;
using Godot;
namespace fluent_behaviour_tree.addons.FluentBehaviourTree.BehaviourTree.Nodes.Decorators;

/**
 * Always fail the child node
 */
[Icon("res://addons/FluentBehaviourTree/BehaviourTree/Nodes/icons/BTDecoratorFail.svg")]
[Tool]
[GlobalClass]
public partial class FailerDecoratorBehaviourNode : DecoratorBehaviourNode {

    public override void BuildNode(FluentBuilder<GodotBehaviourContext> builder) {
        builder.AlwaysFail(Name);
    }
}

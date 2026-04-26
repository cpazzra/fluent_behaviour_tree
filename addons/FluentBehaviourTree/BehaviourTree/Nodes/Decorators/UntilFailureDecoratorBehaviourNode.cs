using BehaviourTree.FluentBuilder;
using Godot;
namespace fluent_behaviour_tree.addons.FluentBehaviourTree.BehaviourTree.Nodes.Decorators;

/**
 * Repeat until child nodes succeeds
 */
[Icon("res://addons/FluentBehaviourTree/BehaviourTree/Nodes/icons/BTDecoratorFail.svg")]
[Tool]
[GlobalClass]
public partial class UntilFailureDecoratorBehaviourNode : DecoratorBehaviourNode {

    public override void BuildNode(FluentBuilder<GodotBehaviourContext> builder) {
        builder.UntilSuccess(Name);
    }
}

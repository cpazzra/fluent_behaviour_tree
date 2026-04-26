using BehaviourTree.FluentBuilder;
using Godot;
namespace fluent_behaviour_tree.addons.FluentBehaviourTree.BehaviourTree.Nodes.Decorators;

[Icon("res://addons/FluentBehaviourTree/BehaviourTree/Nodes/icons/BTDecoratorSucceed.svg")]
[Tool]
[GlobalClass]
public partial class SucceederDecoratorBehaviourNode : DecoratorBehaviourNode {

    public override void BuildNode(FluentBuilder<GodotBehaviourContext> builder) {
        builder.AlwaysSucceed(Name);
    }
}

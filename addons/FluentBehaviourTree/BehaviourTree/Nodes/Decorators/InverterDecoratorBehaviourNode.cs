using BehaviourTree.FluentBuilder;
using Godot;
namespace fluent_behaviour_tree.addons.FluentBehaviourTree.BehaviourTree.Nodes.Decorators;

/**
 * Invert the output status of the child node
 * IE:
 *  SUCCESS -> FAILED
 *  RUNNING -> RUNNING (no change if in progress)
 *  FAILED -> SUCCESS
 */
[Icon("res://addons/FluentBehaviourTree/BehaviourTree/Nodes/icons/BTDecoratorNot.svg")]
[Tool]
[GlobalClass]
public partial class InverterDecoratorBehaviourNode : DecoratorBehaviourNode {

    public override void BuildNode(FluentBuilder<GodotBehaviourContext> builder) {
        builder.Invert(Name);
    }
}
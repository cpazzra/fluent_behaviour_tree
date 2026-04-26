using BehaviourTree.FluentBuilder;
using Godot;
namespace fluent_behaviour_tree.addons.FluentBehaviourTree.BehaviourTree.Nodes.Decorators;

/**
 * Has a n% chance to execute the child nodes
 */
[Icon("res://addons/FluentBehaviourTree/BehaviourTree/Nodes/icons/BTCompositeRandomSelector.svg")]
[Tool]
[GlobalClass]
public partial class RandomDecoratorBehaviourNode : DecoratorBehaviourNode {

    [Export]
    public float randomChance;

    public override void BuildNode(FluentBuilder<GodotBehaviourContext> builder) {
        builder.Random(Name, randomChance);
    }
}

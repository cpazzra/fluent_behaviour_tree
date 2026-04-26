using BehaviourTree.FluentBuilder;
using Godot;
namespace fluent_behaviour_tree.addons.FluentBehaviourTree.BehaviourTree.Nodes.Decorators;

/**
 * After node is composite or leaf is successful, wait N milliseconds before trying again
 */
[Tool]
[GlobalClass]
public partial class CooldownDecoratorBehaviourNode : DecoratorBehaviourNode {

    [Export]
    public int cooldownTimeInMilliseconds;

    public override void BuildNode(FluentBuilder<GodotBehaviourContext> builder) {
        builder.Cooldown(Name, cooldownTimeInMilliseconds);
    }
}
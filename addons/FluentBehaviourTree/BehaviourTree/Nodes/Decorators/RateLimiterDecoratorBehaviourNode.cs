using BehaviourTree.FluentBuilder;
using Godot;
namespace fluent_behaviour_tree.addons.FluentBehaviourTree.BehaviourTree.Nodes.Decorators;

/**
 * Cache the output of a completed call for N milliseconds. Where the cache is dirties and the composite or leaf will be called again
 */
[Icon("res://addons/FluentBehaviourTree/BehaviourTree/Nodes/icons/BTDecoratorLimiter.svg")]
[Tool]
[GlobalClass]
public partial class RateLimiterDecoratorBehaviourNode : DecoratorBehaviourNode {

    [Export]
    public int cacheResultsEveryMilliseconds;

    public override void BuildNode(FluentBuilder<GodotBehaviourContext> builder) {
        builder.LimitCallRate(Name, cacheResultsEveryMilliseconds);
    }
}

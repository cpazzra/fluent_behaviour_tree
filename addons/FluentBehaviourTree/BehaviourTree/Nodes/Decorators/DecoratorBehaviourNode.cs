using Godot;
using System.Collections.Generic;
namespace fluent_behaviour_tree.addons.FluentBehaviourTree.BehaviourTree.Nodes.Decorators;

/**
 * A base node for decorators
 */
[Icon("res://addons/FluentBehaviourTree/BehaviourTree/Nodes/icons/BTDecorator.svg")]
[Tool]
[GlobalClass]
public abstract partial class DecoratorBehaviourNode : BehaviourNode {

    /**
     * Decorators only support a single child node. Effectively acting as a "modifier" to a behaviour.
     * Try to enforce this through the editor via warnings.
     */
    public override string[] _GetConfigurationWarnings() {
        var warnings = new List<string>();

        var childCount = GetChildCount();
        switch (childCount) {
            case > 1:
            {
                warnings.Add($"Expected 1 child, but found {childCount}");
                break;
            }
            case 0:
            {
                warnings.Add("This node requires a child");
                break;
            }
        }

        return warnings.ToArray();
    }

}
using BehaviourTree.FluentBuilder;
using Godot;
namespace fluent_behaviour_tree.addons.FluentBehaviourTree.BehaviourTree.Nodes.Leaves.CommonConditions;

[GlobalClass]
public partial class BlackboardValueCheckBehaviourNode : ConditionBehaviourNode {

    [Export]
    public string blackboardPropertyName;

    [Export]
    public Variant expectedValue;

    public override void BuildNode(FluentBuilder<GodotBehaviourContext> builder) {
        builder.Condition(Name, context => {
            if (!context.blackboard.TryGetValue(blackboardPropertyName, out var value)) {
                GD.PrintErr($"Missing blackboard property {blackboardPropertyName}");
                return false;
            }
            return expectedValue.Obj != null && expectedValue.Obj.Equals(value.Obj);
        });
    }
}

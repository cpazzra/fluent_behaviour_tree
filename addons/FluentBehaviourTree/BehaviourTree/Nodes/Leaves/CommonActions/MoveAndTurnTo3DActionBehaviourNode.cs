#nullable enable
using BehaviourTree;
using BehaviourTree.FluentBuilder;
using Godot;
namespace fluent_behaviour_tree.addons.FluentBehaviourTree.BehaviourTree.Nodes.Leaves.CommonActions;

[GlobalClass]
public partial class MoveAndTurnTo3DActionBehaviourNode : ActionBehaviourNode {

    [Export]
    public required string targetNodeGroup = "player";

    /**
     * What node will handle rotation? Leave null if not doing rotation
     */
    [Export]
    public Node3D? rotatingNode;

    [Export]
    public float speed;

    [Export]
    public Vector3 forwardDirection = Vector3.Forward;

    [Export]
    public required string targetNodePath;

    [Export]
    public int maxDistance;

    [Export]
    public int minDistance;

    /**
     * Conditionally check for ledges. If disabled, ledges will not stop entity movement.
     */
    [ExportCategory("[Optional] - Ledge detection")]
    [Export]
    public bool checkForLedges;

    /**
     * Raycast that will perform the actual ledge detection. The collision layers checked are managed here.
     */
    [Export]
    public RayCast3D? ledgeCast;

    /**
     * If a ledge is preventing movement, fail the node?
     */
    [Export]
    public bool failIfDetected;


    public override void BuildNode(FluentBuilder<GodotBehaviourContext> builder) {
        builder.Do(Name, context => {

            // Need owner to be a character3d node
            if (context.owner is not CharacterBody3D characterBodyOwner) {
                GD.PrintErr($"{Name}: owner is not a CharacterBody3D");
                return BehaviourStatus.Failed;
            }

            if (GetCachedTargetNodeFromGroup(targetNodeGroup, context.blackboard) is not Node3D targetNode) {
                GD.PrintErr($"{Name}: targetNode is not valid");
                return BehaviourStatus.Failed;
            }

            if (checkForLedges) {
                if (ledgeCast == null) {
                    GD.PrintErr($"{Name}: ledgeCast is not valid when expecting a ledge check");
                    return BehaviourStatus.Failed;
                }

                ledgeCast.ForceRaycastUpdate();
                if (!ledgeCast.IsColliding()) {
                    return failIfDetected ? BehaviourStatus.Failed : BehaviourStatus.Running;
                }
            }

            var distanceTo = characterBodyOwner.GlobalPosition.DistanceTo(targetNode.GlobalPosition);

            // TODO: Optional timeout?
            if (distanceTo < minDistance) {
                characterBodyOwner.Velocity = Vector3.Zero;
                return BehaviourStatus.Succeeded;
            }

            if (distanceTo > maxDistance) {
                characterBodyOwner.Velocity = Vector3.Zero;
                return BehaviourStatus.Failed;
            }

            var directionTo = characterBodyOwner.GlobalPosition.DirectionTo(targetNode.GlobalPosition);
            var directionRotation = Mathf.Atan2(directionTo.X, directionTo.Z);

            characterBodyOwner.Velocity = (forwardDirection * speed).Rotated(Vector3.Up, directionRotation);
            characterBodyOwner.MoveAndSlide();

            if (rotatingNode != null) {
                rotatingNode.Rotation = new Vector3(0.0f, directionRotation, 0.0f);
            }

            return BehaviourStatus.Running;
        });
    }
}

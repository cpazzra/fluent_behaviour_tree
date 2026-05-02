using BehaviourTree;
using BehaviourTree.FluentBuilder;
using Godot;
namespace fluent_behaviour_tree.addons.FluentBehaviourTree.BehaviourTree.Nodes.Leaves.CommonActions;

/**
 * Simply play a provided audio stream and immediately succeed
 */
[GlobalClass]
public partial class PlayAudioStream3DActionBehaviourNode : ActionBehaviourNode {

    // TODO: Are there any other parameters or configuration that should be exposed here?
    [Export]
    public required AudioStreamPlayer3D audioStreamPlayer;

    [Export]
    public required bool succeedOnAudioFinish;

    private bool isFinished;

    public override void BuildNode(FluentBuilder<GodotBehaviourContext> builder) {
        if (succeedOnAudioFinish) {
            audioStreamPlayer.Finished += () => isFinished = true;
        }

        builder.Do(Name, context => {
            audioStreamPlayer.Play();

            if (!succeedOnAudioFinish) {
                return BehaviourStatus.Succeeded;
            }

            return isFinished ? BehaviourStatus.Succeeded : BehaviourStatus.Running;
        });
    }
}

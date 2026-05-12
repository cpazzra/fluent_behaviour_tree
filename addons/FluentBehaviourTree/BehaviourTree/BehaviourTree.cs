using BehaviourTree;
using BehaviourTree.Composites;
using BehaviourTree.Decorators;
using BehaviourTree.FluentBuilder;
using fluent_behaviour_tree.addons.FluentBehaviourTree.BehaviourTree.Debugging;
using fluent_behaviour_tree.addons.FluentBehaviourTree.BehaviourTree.Nodes;
using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using Node = Godot.Node;

namespace fluent_behaviour_tree.addons.FluentBehaviourTree.BehaviourTree;

/**
 * The entry point and root node for a behaviour tree.
 *
 * Leverages <see cref="FluentBuilder<GodotBehaviourContext>"/> under the hood the handle all the actual behaviour tree logic.
 */
[Icon("res://addons/FluentBehaviourTree/BehaviourTree/Nodes/icons/BTRoot.svg")]
[GlobalClass]
public partial class BehaviourTree : Node {

    [Export]
    public required bool enabled = true;

    [Export]
    public required Node3D treeOwner;

    /**
     * A hard coded blackboard value that determines if a behaviour tree can be interrupted via <see cref="Interrupt"/>
     */
    public static readonly string BB_PROP_CAN_INTERUPT = "CAN_INTERRUPT";

    /**
     * Properties bound to the behaviour tree. Includes defaults.
     */
    [Export]
    public Godot.Collections.Dictionary<string, Variant> blackboard = new() {
        // Default interrupts to true. Allows leaves to conditionally enable/disable interrupts 
        [BB_PROP_CAN_INTERUPT] = true
    };

    public IBehaviour<GodotBehaviourContext> behaviourTree { get; private set; }

    private string debuggerId;

    public override void _Ready() {
        base._Ready();
        var builder = new FluentBuilder<GodotBehaviourContext>();
        var behaviourNodes = GetChildren()
            .Where(node => node is BehaviourNode)
            .Cast<BehaviourNode>()
            .ToList();

        // Don't "end" branch since it's the root
        AddBranch(builder, behaviourNodes, false);
        behaviourTree = builder.Build();
        debuggerId = $"{Owner.Name}-{Owner.GetInstanceId()}";
        // Once built, register with debugger
        #if TOOLS
        BehaviourTreeDebugRegistrar.RegisterTree(treeOwner, this);
        #endif
    }

    public override void _Process(double delta) {
        base._Process(delta);

        if (!enabled) {
            return;
        }

        behaviourTree.Tick(new GodotBehaviourContext((float)delta, treeOwner, blackboard));
        #if TOOLS
        BehaviourTreeDebugRegistrar.UpdateTree(treeOwner, this);
        #endif
    }

    public override void _Notification(int what) {
        if (what == NotificationPredelete) {
            #if TOOLS
            BehaviourTreeDebugRegistrar.UnregisterTree(treeOwner, this);
            #endif
        }
    }

    /**
     * From a "new root" BT node with children (IE sequence or composite nodes)
     */
    private void AddBranch(
        FluentBuilder<GodotBehaviourContext> builder,
        List<BehaviourNode> childNodes,
        bool canEndBranch = true) {

        foreach (var behaviourNode in childNodes) {
            behaviourNode.BuildNode(builder);
            var behaviourNodes = behaviourNode.GetChildren()
                .Where(node => node is BehaviourNode)
                .Cast<BehaviourNode>()
                .ToList();

            if (behaviourNodes.Count != 0) {
                AddBranch(builder, behaviourNodes);
            }
        }

        if (canEndBranch) {
            builder.End();
        }
    }

    /**
     * Restart the behaviour tree from the top. Useful when, for example Player input demands the BT be recalculated from the start for hit stun/death branches.
     */
    public void Interrupt() {
        if (blackboard[BB_PROP_CAN_INTERUPT].AsBool()) {
            behaviourTree.Reset();
        }
    }

    /**
     * Build a variant-compatible dictionary for the debugger from the root node. Required since Godot handles
     * editor-application interactions through the networking interface via messaging, which only supports variants.
     * <param name="debuggerMessage">Includes debugger message for potential troubleshooting of debug tab</param>
     * <seealso cref="GetNodeDebuggerData"/>
     */
    public Dictionary GetTreeDebuggerData(string debuggerMessage) {
        return GetNodeDebuggerData(debuggerMessage, 0, behaviourTree);
    }

    /**
     * A payload for the debugger using variant-compatible dictionaries
     * Formatted as such
     * <code>
     *  {
     *      "depth": 0,
     *      "name": "root",
     *      "status": 1, // The status from the `BehaviourStatus` enum
     *      "childNodes" : [
     *          {
     *              "depth": 1,
     *              "name": "Sequence",
     *              "status": 0, // The status from the `BehaviourStatus` enum
     *              "childNodes": [...]
     *          },
     *          {
     *              ...
     *          },
     *          ...
     *      ],
     *      "blackboard": { // Only the root will have the blackboard
     *          ...
     *      }
     *  }
     * </code>
     * <param name="debuggerMessage">Includes debugger message for potential troubleshooting of debug tab</param>
     * <param name="depth"></param>
     * <param name="behaviourNode"></param>
     */
    private Dictionary GetNodeDebuggerData(string debuggerMessage,
        int depth,
        IBehaviour<GodotBehaviourContext> behaviourNode) {
        var nodeDebugMapping = new Dictionary();
        nodeDebugMapping["depth"] = depth;
        nodeDebugMapping["name"] = depth == 0 ? debuggerId : behaviourNode.Name;
        nodeDebugMapping["status"] = (int)behaviourNode.Status;


        // Only the root will have the blackboard 
        if (depth == 0) {
            nodeDebugMapping.Add("blackboard", blackboard);
        }

        var childDepth = depth + 1;

        var children = new Array<Dictionary>();

        switch (behaviourNode) {
            case CompositeBehaviour<GodotBehaviourContext> compositeBehaviour:
            {
                foreach (var child in compositeBehaviour.Children) {
                    children.Add(GetNodeDebuggerData(debuggerMessage, childDepth, child));
                }
                break;
            }
            case DecoratorBehaviour<GodotBehaviourContext> decoratorBehaviour:
            {
                children.Add(GetNodeDebuggerData(debuggerMessage, childDepth, decoratorBehaviour.Child));
                break;
            }
        }

        nodeDebugMapping["childNodes"] = children;

        return nodeDebugMapping;
    }

}
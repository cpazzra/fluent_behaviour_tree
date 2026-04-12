using Godot;
using Godot.Collections;
using System.Linq;
namespace fluent_behaviour_tree.addons.FluentBehaviourTree.BehaviourTree.Debugging;

public partial class BehaviourTreeDebugRegistrar : Node {

    /**
     * A map containing behaviour trees mapped by their owner's name
     */
    private Dictionary<string, BehaviourTree> registeredTrees = [];

    /**
     * Instance of singleton
     */
    private static BehaviourTreeDebugRegistrar _instance;

    public static BehaviourTreeDebugRegistrar Instance
    {
        get {
            if (_instance != null) return _instance;
            _instance =
                ((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull<BehaviourTreeDebugRegistrar>(
                    nameof(BehaviourTreeDebugRegistrar));

            if (_instance != null) return _instance;

            _instance = new BehaviourTreeDebugRegistrar();
            _instance.Name = nameof(BehaviourTreeDebugRegistrar);
            return _instance;
        }
    }

    public static void RegisterTree(Node owner, BehaviourTree tree) {
        Instance.registeredTrees[GetReadableTreeKey(owner)] = tree;
        if (CanSendMessage()) {
            var messageParams =
                new Array([tree.GetTreeDebuggerData(FluentBehaviourTreeDebugger.MESSAGE_REGISTER_TREE)]);
            EngineDebugger.SendMessage(FluentBehaviourTreeDebugger.MESSAGE_REGISTER_TREE, messageParams);
        }
    }

    public static void UpdateTree(Node owner, BehaviourTree tree) {
        Instance.registeredTrees[GetReadableTreeKey(owner)] = tree;
        if (CanSendMessage()) {
            var messageParams = new Array([tree.GetTreeDebuggerData(FluentBehaviourTreeDebugger.MESSAGE_UPDATE_TREE)]);
            EngineDebugger.SendMessage(FluentBehaviourTreeDebugger.MESSAGE_UPDATE_TREE, messageParams);
        }
    }

    public static void UnregisterTree(Node owner, BehaviourTree tree) {
        Instance.registeredTrees.Remove(GetReadableTreeKey(owner));
        // Send dictionary of current tree to debugger for removal
        if (CanSendMessage()) {
            var messageParams =
                new Array([tree.GetTreeDebuggerData(FluentBehaviourTreeDebugger.MESSAGE_UNREGISTER_TREE)]);
            EngineDebugger.SendMessage(FluentBehaviourTreeDebugger.MESSAGE_UNREGISTER_TREE, messageParams);
        }
    }

    public static Array<BehaviourTree> AvailableTreesAsList() {
        var list = new Array<BehaviourTree>();
        foreach (var behaviourTree in Instance.registeredTrees.Select(pair => pair.Value)) {
            list.Add(behaviourTree);
        }
        return list;
    }

    public static string GetReadableTreeKey(Node node) {
        // Use both the node name and it's session instanceID
        return $"{node.Name}-{node.GetInstanceId()}";
    }

    public static bool CanSendMessage() {
        // Only send message if using editor debugger and is supported
        return EngineDebugger.IsActive() && !Engine.IsEditorHint() && OS.HasFeature("editor");
    }
}

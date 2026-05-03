using Godot;
using System.Collections.Generic;
using Array = Godot.Collections.Array;
namespace fluent_behaviour_tree.addons.FluentBehaviourTree.BehaviourTree.Debugging;

[Tool]
public partial class FluentBehaviourTreeDebugger : EditorDebuggerPlugin {

    private static string MESSAGE_PREFIX = "FluentBehaviourTree";

    public static string MESSAGE_REGISTER_TREE = "FluentBehaviourTree:RegisterTree";

    public static string MESSAGE_UNREGISTER_TREE = "FluentBehaviourTree:UnregisterTree";

    public static string MESSAGE_UPDATE_TREE = "FluentBehaviourTree:UpdateTree";


    private BehaviourTreeDebuggerPanel debuggerPanel = new BehaviourTreeDebuggerPanel();

    private EditorDebuggerSession session;

    public override void _SetupSession(int sessionId) {
        session = GetSession(sessionId);
        session.Started += () => debuggerPanel.Start();
        session.Stopped += () => debuggerPanel.Stop();


        GD.Print("Adding debugger session tab");

        debuggerPanel.Name = "Behaviour Tree Live View";
        debuggerPanel.session = session;
        session.AddSessionTab(debuggerPanel);
    }

    public override bool _HasCapture(string capture) {
        return capture == MESSAGE_PREFIX;
    }

    public override bool _Capture(string message, Array data, int sessionId) {
        // GD.Print($"message: {message}, sessionId: {sessionId}, data: {data}");
        if (debuggerPanel == null) {
            return false;
        }

        if (message == MESSAGE_REGISTER_TREE) {
            var behaviourTree = data[0].AsGodotDictionary();
            debuggerPanel.TreeRegistered(behaviourTree);
            return true;
        }
        if (message == MESSAGE_UNREGISTER_TREE) {
            var behaviourTree = data[0].AsGodotDictionary();
            if (!debuggerPanel.CanUnregister()) {
                GD.PushWarning("No behaviour trees registered");
                return true;
            }
            debuggerPanel.TreeUnregistered(behaviourTree);
            return true;
        }
        if (message == MESSAGE_UPDATE_TREE) {
            var behaviourTree = data[0].AsGodotDictionary();
            var treeName = behaviourTree.GetValueOrDefault("name", "").AsString();
            // The debugger panel tree should never be empty, or missing this field
            // Double check here so we can log a useful message just in case 
            if (treeName != string.Empty) {
                // Only update currently selected tree. Message is valid regardless. But only parse relevant messages.
                if (treeName == debuggerPanel.GetTreeName(debuggerPanel.behaviour)) {
                    debuggerPanel.UpdateTree(behaviourTree);
                }
                return true;
            } else {
                GD.Print($"Error parsing update message: {behaviourTree}");
            }
        }

        return false;
    }
}

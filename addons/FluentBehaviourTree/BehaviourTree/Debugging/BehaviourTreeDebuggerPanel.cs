#if TOOLS
using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;
namespace fluent_behaviour_tree.addons.FluentBehaviourTree.BehaviourTree.Debugging;

[Tool]
public partial class BehaviourTreeDebuggerPanel : PanelContainer {

    internal EditorDebuggerSession session;

    internal Dictionary behaviour;

    private int index;

    private Array<Dictionary> treeArray = [];

    private OptionButton treeList = new OptionButton();

    /**
     * Contains the <see cref="treeList"/> and <see cref="debugContentContainer"/> in vertical order
     */
    private VSplitContainer rootContainer = new VSplitContainer();

    /**
     * Contains both the <see cref="treeContainer"/> for the behaviour tree and the <see cref="blackboardContainer"/> in horizontal order
     */
    private HSplitContainer debugContentContainer = new HSplitContainer();

    /**
     * Behaviour tree UI contents
     */
    private ScrollContainer treeContainer = new ScrollContainer();

    private VBoxContainer treeContainerVBox = new VBoxContainer();

    private BehaviourTreeViewContainer rootControl;

    /**
     * Blackboard UI contentsscroll
     */
    private ScrollContainer blackboardContainer = new ScrollContainer();

    private GridContainer blackboardGridContainer = new GridContainer();

    /**
     * References to the labels for the table by blackboard key
     * Should mean updates will affect these labels rather than reconstructing the table each tick
     */
    private Godot.Collections.Dictionary<string, Array<Label>> blackboardDataTable = [];

    private Array<Label> headerLabels = [];


    public override void _Ready() {
        rootContainer.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(rootContainer);

        rootContainer.AddChild(treeList);
        rootContainer.AddChild(debugContentContainer);
        // Setup signals for new behaviour trees post game startup
        treeList.ItemSelected += index => { SelectTree(treeArray[(int)index]); };

        // BT UI
        debugContentContainer.AddChild(treeContainer);
        treeContainerVBox.Alignment = BoxContainer.AlignmentMode.Begin;
        treeContainer.AddChild(treeContainerVBox);
        // TODO: This is a temporary solution. Not sure why the rich labels or tree container are collapsing to nothing
        treeContainer.CustomMinimumSize = new Vector2(800, 0);

        // BB UI
        var keyLabel = new Label();
        keyLabel.Text = "Blackboard Key:";
        var valueLabel = new Label();
        valueLabel.Text = "Value:";
        headerLabels.Add(keyLabel);
        headerLabels.Add(valueLabel);

        debugContentContainer.AddChild(blackboardContainer);
        blackboardContainer.AddChild(blackboardGridContainer);
        blackboardGridContainer.Columns = 2;
        blackboardContainer.CustomMinimumSize = new Vector2(250, 25);

        blackboardGridContainer.AddChild(keyLabel);
        blackboardGridContainer.AddChild(valueLabel);
    }

    private void SelectTree(Dictionary tree) {
        behaviour = tree;
        // Remove if already exists. Prevents multiple trees from rendering at once.
        if (treeContainerVBox.GetChildren().Contains(rootControl)) {
            treeContainerVBox.RemoveChild(rootControl);
        }
        rootControl = new BehaviourTreeViewContainer(behaviour);
        treeContainerVBox.AddChild(rootControl);

        // If more than just the headers exist, the blackboard is populated and needs to be cleared
        if (blackboardGridContainer.GetChildren().Count > 2) {
            ResetBlackboardTable();
        }

        var blackboardDictionary = tree["blackboard"].AsGodotDictionary();
        UpdateBlackboardTable(blackboardDictionary);
    }

    private void ResetBlackboardTable() {
        blackboardDataTable.Clear();
        foreach (var item in blackboardGridContainer.GetChildren()) {
            blackboardGridContainer.RemoveChild(item);
        }
        foreach (var headerLabel in headerLabels) {
            blackboardGridContainer.AddChild(headerLabel);
        }
    }

    private void UpdateBlackboardTable(Dictionary blackboardDictionary) {

        foreach (var keyValuePair in blackboardDictionary) {
            var stringKey = keyValuePair.Key.AsString();
            var value = keyValuePair.Value;

            if (!blackboardDataTable.TryGetValue(stringKey, out var entryLabels)) {
                var keyLabel = new Label();
                keyLabel.Text = stringKey;
                var valueLabel = new Label();
                valueLabel.Text = $"{value.ToString()}";
                blackboardDataTable[stringKey] = [keyLabel, valueLabel];
                blackboardGridContainer.AddChild(keyLabel);
                blackboardGridContainer.AddChild(valueLabel);
            } else {
                entryLabels[1].Text = $"{value.ToString()}";
            }
        }
    }

    public void Start() {}

    internal void TreeRegistered(Dictionary tree) {
        InsertTree(tree);
    }

    internal void TreeUnregistered(Dictionary tree) {
        // The list and the option control should have synced indices
        var derivedIndex = treeArray.ToList()
            .FindIndex(dictionary => dictionary["name"].AsString() == tree["name"].AsString());
        treeList.RemoveItem(derivedIndex);
        treeArray.RemoveAt(derivedIndex);
    }

    private void InsertTree(Dictionary tree) {
        if (behaviour == null) {
            SelectTree(tree);
        }
        treeList.AddItem(GetTreeName(tree));
        treeArray.Add(tree);
    }

    public void Stop() {
        behaviour = null;
        treeContainerVBox.RemoveChild(rootControl);
        ResetBlackboardTable();
        treeList.Clear();
        treeArray.Clear();
    }

    public void UpdateTree(Dictionary behaviourTree) {
        rootControl?.UpdateData(behaviourTree);
        UpdateBlackboardTable(behaviourTree["blackboard"].AsGodotDictionary());
    }

    public bool CanUnregister() {
        return treeArray.Count > 0;
    }

    public string GetTreeName(Dictionary tree) {
        return tree.GetValueOrDefault("name", "").AsString();
    }
}
#endif

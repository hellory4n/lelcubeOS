using Godot;
using System;

public class ActiveThing : Control {
    public override void _Process(float delta) {
        base._Process(delta);
        // just in case the app keeps adding nodes
        Raise();
    }

    // the pass mouse filter doesn't work lol haha
    public override void _Input(InputEvent @event) {
        if (@event is InputEventMouseButton m) {
            if (m.Pressed) {
                if (GetRect().HasPoint(m.Position)) {
                    // getparent
                    if (GetParent().GetIndex() != GetParent().GetParent().GetChildCount()-1) {
                        GetParent().Raise();
                    }
                }
            }
        }
        base._Input(@event);
    }
}

using Godot;
using System;

namespace Lelsktop.Toolkit;

[GlobalClass]
public partial class Copy : Button
{
    [Export]
    public string TextToCopy = "";

    public override void _Pressed()
    {
        base._Pressed();
        DisplayServer.ClipboardSet(TextToCopy);
    }
}

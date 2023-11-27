using Godot;
using Kickstart.Drivers;
using System;
using System.Collections.Generic;
using Dashboard.Wm;
using Dashboard.Toolkit;
using Dashboard.Pinboard;
using Kickstart.Records;
using Kickstart.Cabinetfs;

namespace Dashboard;

public partial class Dashboard : Control
{
	/// <summary>
	/// If true, the user is currently using either the dock, panel, app menu, quick settings, or the workspace switcher.
	/// </summary>
	public static bool InteractingWithDashboardInterface = false;
	[Export]
	public SubViewport Windows;
	[Export]
	public SubViewport Interface;
	[Export]
	Panel Dock;
	[Export]
	Panel Panel;
	[Export]
	Panel QuickSettings;
	[Export]
	Panel AppMenu;
	[Export]
	AnimationPlayer Animator;
	[Export]
	SubViewport Pinboard;
	[Export]
	PackedScene Sticker;
	[Export]
	PackedScene StickyNote;

	public override void _Ready()
	{
		base._Ready();

		Vector2I bruh = ResolutionManager.Resolution;

		DashboardConfig suffer = RecordManager.Load<DashboardConfig>();

		// load the wallpaper
		// is it a default wallpaper?
		if (ResourceLoader.Exists(suffer.Wallpaper))
		{
			string wallpaperPath = suffer.Wallpaper;
			Texture2D wallpaper = GD.Load<Texture2D>(wallpaperPath);
			GetNode<Sprite2D>("Wallpaper").Texture = wallpaper;
		// is it a cabinetfs file?
		}
		else if (CabinetfsManager.IdExists(suffer.Wallpaper))
		{
			var epicFile = CabinetfsManager.LoadFile(suffer.Wallpaper);
			Texture2D wallpaper = ResourceManager.LoadImage(epicFile.Data["Resource"].ToString());
			GetNode<Sprite2D>("Wallpaper").Texture = wallpaper;

			// scale wallpaper thing :))))
			GetNode<ImageBackground>("Wallpaper").OriginalSize = wallpaper.GetSize();
			float scale;
			if (bruh > wallpaper.GetSize())
			{
				scale = (Mathf.Max(bruh.X, bruh.Y) - Mathf.Max(wallpaper.GetSize().X, wallpaper.GetSize().Y)) /
					Mathf.Max(wallpaper.GetSize().X, wallpaper.GetSize().X);
				scale += 1;
			}
			else
				scale = Mathf.Max(bruh.X, bruh.X) / Mathf.Max(wallpaper.GetSize().X, wallpaper.GetSize().Y);

			GetNode<Sprite2D>("Wallpaper").Scale = new Vector2(scale, scale);
			GetNode<Sprite2D>("Wallpaper").Position = bruh/2;
		}
		// ok it's broken, just load the default wallpaper
		else
		{
			var wallpaper = GD.Load<Texture2D>("res://Assets/Wallpapers/HighPeaks.jpg");
			GetNode<Sprite2D>("Wallpaper").Texture = wallpaper;
		}

		// startup sound :)
		/*SoundManager sounds = GetNode<SoundManager>("/root/SoundManager");
		sounds.PlaySoundEffect(SoundManager.SoundEffects.Startup);*/

		Dock.Size = new Vector2(75, bruh.Y);

		// play the animation for the dock and make sure the position on the animation is correct :)
		// i could use a tween but last time i tried tweens it looked like shit
		Animation animationomg = Animator.GetAnimation("Startup");
		int keyStart = animationomg.TrackFindKey(0, 0);
		int keyEnd = animationomg.TrackFindKey(0, 0.5f);
		animationomg.TrackSetKeyValue(0, keyStart, new Vector2(bruh.X, 0));
		animationomg.TrackSetKeyValue(0, keyEnd, new Vector2(bruh.X-75, 0));

		// and also fix the animation for the quick settings
		Animation animationOrSomething = Animator.GetAnimation("OpenQuickSettings");
		int keyStartOrSomething = animationOrSomething.TrackFindKey(0, 0);
		int keyEndOrSomething = animationOrSomething.TrackFindKey(0, 0.5f);
		animationOrSomething.TrackSetKeyValue(0, keyStartOrSomething, new Vector2(bruh.X-375, -475));
		animationOrSomething.TrackSetKeyValue(0, keyEndOrSomething, new Vector2(bruh.X-375, 40));

		Animation animationButDifferent = Animator.GetAnimation("CloseQuickSettings");
		int keyStartButDifferent = animationButDifferent.TrackFindKey(0, 0);
		int keyEndButDifferent = animationButDifferent.TrackFindKey(0, 0.5f);
		animationButDifferent.TrackSetKeyValue(0, keyStartButDifferent, new Vector2(bruh.X-375, 40));
		animationButDifferent.TrackSetKeyValue(0, keyEndButDifferent, new Vector2(bruh.X-375, -475));

		// load theme
		Theme theme = GD.Load<Theme>($"res://Assets/Themes/{suffer.Theme}/Theme.tres");
		Windows.GetNode<Control>("ThemeThing").Theme = theme;
		Dock.Theme = theme;
		QuickSettings.Theme = theme;
		AppMenu.Theme = theme;
		Panel.Theme = theme;

		// quick launch stuff
		foreach (var app in suffer.QuickLaunch)
		{
			PackedScene packedScene = GD.Load<PackedScene>("res://OS/Dashboard/QuickLaunchButton.tscn");
			OpenWindow yes = packedScene.Instantiate<OpenWindow>();
			yes.Icon = GD.Load<Texture2D>(app.Icon);
			yes.WindowScene = app.Executable;
			yes.TooltipText = app.DisplayName;
			Dock.GetNode("DockStuff/QuickLaunch").AddChild(yes);
		}

		// load the pinboard stuff :)))
		foreach (var item in suffer.Pinboard)
		{
			if (item.Value.IsStickyNote)
			{
				var bullshit = StickyNote.Instantiate<StickyNote>();
				bullshit.Position = item.Value.Position;
				bullshit.PinboardItem = item.Key;
				Pinboard.AddChild(bullshit);
				bullshit.GetNode<TextEdit>("Text").Text = item.Value.Text;
			}
			else
			{
				var sticker = Sticker.Instantiate<Sticker>();
				sticker.Position = item.Value.Position;
				sticker.Scale = new Vector2((float)item.Value.Scale, (float)item.Value.Scale);
				sticker.Texture = ResourceManager.LoadImage(item.Value.TexturePath);
				sticker.PinboardItem = item.Key;
				Pinboard.AddChild(sticker);
			}
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		// if the user is editing the pinboard stuff we don't need to process this shit anyway
		/*if (Pinboard.Pinboard.EditingPinboard)
		{
			InteractingWithDashboardInterface = false;
			return;
		}

		Vector2 pain = ResolutionManager.Resolution;
		SubViewport bruh1 = GetNode<SubViewport>("/root/Dashboard/1/Windows");
		SubViewport bruh2 = GetNode<SubViewport>("/root/Dashboard/2/Windows");
		SubViewport bruh3 = GetNode<SubViewport>("/root/Dashboard/3/Windows");
		SubViewport bruh4 = GetNode<SubViewport>("/root/Dashboard/4/Windows");
		Panel appMenu = GetNode<Panel>("/root/DashboardInterface/AppMenu");
		Panel quickSettings = GetNode<Panel>("/root/DashboardInterface/QuickSettings");
		Panel workspaces = GetNode<Panel>("/root/DashboardInterface/Workspaces");
		Color invisible = new(1, 1, 1, 0);

		if (GetGlobalMousePosition().Y < 40 || GetGlobalMousePosition().X > pain.X-75 ||
		appMenu.Modulate != invisible || quickSettings.Modulate != invisible
		|| workspaces.Modulate != invisible)
		{
			bruh1.GuiDisableInput = true;
			bruh2.GuiDisableInput = true;
			bruh3.GuiDisableInput = true;
			bruh4.GuiDisableInput = true;
			
			InteractingWithDashboardInterface = true;
		}
		else
		{
			// suffering
			if (WindowManager.CurrentWorkspace == bruh1 true)
				bruh1.GuiDisableInput = false;
			if (WindowManager.CurrentWorkspace == bruh2 true)
				bruh2.GuiDisableInput = false;
			if (WindowManager.CurrentWorkspace == bruh3 true)
				bruh3.GuiDisableInput = false;
			if (WindowManager.CurrentWorkspace == bruh4 true)
				bruh4.GuiDisableInput = false;

			InteractingWithDashboardInterface = false;
		}*/
	}
}

// Copyright (c) 2017 Leacme (http://leac.me). View LICENSE.md for more information.
using Godot;
using System;

public class Main : Spatial {

	public AudioStreamPlayer Audio { get; } = new AudioStreamPlayer();

	private void InitSound() {
		if (!Lib.Node.SoundEnabled) {
			AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), true);
		}
	}

	public override void _Notification(int what) {
		if (what is MainLoop.NotificationWmGoBackRequest) {
			GetTree().ChangeScene("res://scenes/Menu.tscn");
		}
	}

	public override void _Ready() {
		var env = GetNode<WorldEnvironment>("sky").Environment;
		env.BackgroundColor = new Color(Lib.Node.BackgroundColorHtmlCode);
		env.BackgroundMode = Godot.Environment.BGMode.Sky;
		env.BackgroundSky = new PanoramaSky() { Panorama = ((Texture)GD.Load("res://assets/stars.hdr")) };
		env.BackgroundSkyRotationDegrees = new Vector3(0, 0, 0);
		env.BackgroundEnergy = 0.5f;
		env.BackgroundSkyCustomFov = 130;
		env.DofBlurFarEnabled = true;
		env.GlowEnabled = true;
		env.GlowIntensity = 0.45f;
		env.GlowStrength = 0.9f;
		env.GlowBlendMode = Godot.Environment.GlowBlendModeEnum.Additive;
		env.GlowHdrThreshold = 0;
		env.GlowBicubicUpscale = true;

		InitSound();
		AddChild(Audio);

		GetNode<CSGTorus>("PlanetPivot5/Planet/Rings").MaterialOverride = new SpatialMaterial() {
			AlbedoColor = Color.FromHsv(GD.Randf(), 0.5f, 1)
		};

		for (int i = 1; i <= 5; i++) {
			GetNode<Spatial>("PlanetPivot" + i).RotateY(Mathf.Deg2Rad((float)GD.RandRange(-180, 180)));
			var planetMesh = GetNode<Spatial>("PlanetPivot" + i).GetNode("Planet").GetNode<MeshInstance>("Star");
			var randCol = Color.FromHsv(GD.Randf(), 0.5f, 1);
			var randNoise = new OpenSimplexNoise() {
				Period = 40,
				Persistence = 0,
				Lacunarity = 0.1f,
			};
			var randNoiseTx = new NoiseTexture() {
				Noise = randNoise
			};
			var randNoiseTxBm = new NoiseTexture() {
				Noise = randNoise,
				AsNormalmap = true,
				BumpStrength = 1
			};
			planetMesh.MaterialOverride = new SpatialMaterial() {
				AlbedoColor = randCol,
				AlbedoTexture = randNoiseTx,
				NormalEnabled = true,
				NormalTexture = randNoiseTxBm,
				EmissionEnabled = true,
				Emission = randCol,
				DepthEnabled = true,
				DepthTexture = randNoiseTx,
				DepthScale = 0.5f,
				EmissionEnergy = 0.02f

			};
		}

	}

	public override void _Process(float delta) {
		GetNode<Spatial>("PlanetPivot1").RotateY(Mathf.Deg2Rad(-12 * delta));
		GetNode<Spatial>("PlanetPivot2").RotateY(Mathf.Deg2Rad(-8 * delta));
		GetNode<Spatial>("PlanetPivot3").RotateY(Mathf.Deg2Rad(-10 * delta));
		GetNode<Spatial>("PlanetPivot4").RotateY(Mathf.Deg2Rad(-10 * delta));
		GetNode<Spatial>("PlanetPivot5").RotateY(Mathf.Deg2Rad(-6 * delta));

		for (int i = 1; i <= 5; i++) {
			var planet = GetNode<Spatial>("PlanetPivot" + i).GetNode<Spatial>("Planet");
			planet.RotateY(Mathf.Deg2Rad(25 / planet.Scale.x * delta));
			planet.RotateZ(Mathf.Deg2Rad(5 * delta));
		}
	}

}

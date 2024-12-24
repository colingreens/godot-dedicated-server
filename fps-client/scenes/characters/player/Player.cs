using System;
using Godot;

namespace Game.Scenes.Characters.Player;
public partial class Player : CharacterBody3D
{
    [Export(PropertyHint.Range, "0,1")] public float MouseSensitivityH { get; set; } = 0.01f;
    [Export(PropertyHint.Range, "0,1")] public float MouseSensitivityV { get; set; } = 0.01f;

    private Camera3D camera;
    private CharacterMover characterMover;
    private HealthComponent health;

    private float _verticalRotation = 0.0f;
    private float _clampedLookRads = Mathf.DegToRad(90);

    private bool _dead = false;

    public override void _Ready()
    {
        camera = GetNode<Camera3D>("Camera3D");
        characterMover = GetNode<CharacterMover>("CharacterMover");
        health = GetNode<HealthComponent>("HealthComponent");

        Input.MouseMode = Input.MouseModeEnum.Captured;

        health.Died += Kill;
    }

    public override void _Input(InputEvent @event)
    {
        if (_dead)
        {
            return;
        }

        if (@event is InputEventMouseMotion motionEvent)
        {
            RotateY(-motionEvent.Relative.X * MouseSensitivityH);


            // Update and clamp the vertical rotation
            _verticalRotation += -motionEvent.Relative.Y * MouseSensitivityV;
            _verticalRotation = Mathf.Clamp(_verticalRotation, -_clampedLookRads, _clampedLookRads);

            // Apply the clamped rotation to the camera
            var cameraRotation = camera.Rotation;
            camera.Rotation = new Vector3(_verticalRotation, cameraRotation.Y, cameraRotation.Z);

        }
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("quit"))
        {
            GetTree().Quit();
        }

        if (Input.IsActionJustPressed("restart"))
        {
            GetTree().ReloadCurrentScene();
        }

        if (Input.IsActionJustPressed("fullscreen"))
        {
            var fs = DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen;
            if (fs)
            {
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
            }
            else
            {
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
            }
        }

        if (_dead)
        {
            return;
        }

        var inputDir = Input.GetVector("move_left", "move_right", "move_forwards", "move_backwards");
        var moveDir = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        characterMover.SetMoveDirection(moveDir);
        if (Input.IsActionJustPressed("jump"))
        {
            characterMover.Jump();
        }
    }

    private void Kill()
    {
        _dead = true;
        characterMover.SetMoveDirection(Vector3.Zero);
    }
}

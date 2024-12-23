using Godot;

namespace Game.Scenes.Characters;
public partial class CharacterMover : Node3D
{
    [Export(PropertyHint.Range, "0,30")] public float JumpForce { get; set; } = 15.0f;
    [Export(PropertyHint.Range, "0,60")] public float Gravity { get; set; } = 30.0f;
    [Export(PropertyHint.Range, "0,30")] public float MaxSpeed { get; set; } = 15.0f;
    [Export(PropertyHint.Range, "0,30")] public float MoveAccel { get; set; } = 5.0f;
    [Export(PropertyHint.Range, "0,1")] public float StopDrag { get; set; } = 0.9f;

    private CharacterBody3D characterBody;
    private float moveDrag = 0.0f;
    private Vector3 moveDir;

    public override void _Ready()
    {
        characterBody = GetParent<CharacterBody3D>();
        moveDrag = MoveAccel / MaxSpeed;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (characterBody.Velocity.Y > 0.0 && characterBody.IsOnCeiling())
        {
            characterBody.Velocity = new Vector3(characterBody.Velocity.X, 0.0f, characterBody.Velocity.Z);
        }
        if (!characterBody.IsOnFloor())
        {
            characterBody.Velocity += new Vector3(0, (float)(-Gravity * delta), 0);
        }

        var drag = moveDrag;
        if (Mathf.IsZeroApprox(moveDrag))
        {
            drag = StopDrag;
        }

        var flatVelo = characterBody.Velocity;
        flatVelo.Y = 0.0f;
        characterBody.Velocity += MoveAccel * moveDir - flatVelo * drag;

        characterBody.MoveAndSlide();
    }

    public void SetMoveDirection(Vector3 newMoveDir)
    {
        moveDir = newMoveDir;
    }

    public void Jump()
    {
        if (characterBody.IsOnFloor())
        {
            characterBody.Velocity = new Vector3(characterBody.Velocity.X, characterBody.Velocity.Y + JumpForce, characterBody.Velocity.Z);
        }
    }
}

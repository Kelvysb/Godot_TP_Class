using Godot;

public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	public const float MouseSensitivity = 0.05f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public Node3D Pivot { get; set; }

	public Camera3D Camera { get; set; }

	public Node3D Geometry { get; set; }

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
		else if (Input.IsActionPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		base._UnhandledInput(@event);
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
			var prevY = Geometry.Rotation.Y;
			Geometry.LookAt(direction + Position);
			var targetY = Geometry.Rotation.Y;
			Geometry.Rotation = new Vector3(Geometry.Rotation.X, (float)Mathf.LerpAngle(prevY, targetY, delta * 10), Geometry.Rotation.Z);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			RotateY(Mathf.DegToRad(-(@event as InputEventMouseMotion).Relative.X * MouseSensitivity));
			Geometry.RotateY(Mathf.DegToRad((@event as InputEventMouseMotion).Relative.X * MouseSensitivity));
			Pivot.RotateX(Mathf.DegToRad(-(@event as InputEventMouseMotion).Relative.Y * MouseSensitivity));
			Pivot.Rotation = new Vector3(Mathf.DegToRad(Mathf.Clamp(Mathf.RadToDeg(Pivot.Rotation.X), -90, 90)), Pivot.Rotation.Y, Pivot.Rotation.Z);
		}

		base._Input(@event);
	}

	public override void _Ready()
	{
		Pivot = GetNode<Node3D>("CameraMount");
		Camera = GetNode<Camera3D>("CameraMount/Camera3D");
		Geometry = GetNode<Node3D>("Geometry");
		base._Ready();
	}
}

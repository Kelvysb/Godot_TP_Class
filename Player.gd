extends CharacterBody3D


const SPEED = 5.0
const JUMP_VELOCITY = 4.5
const MOUSE_SENSITIVITY = 0.05
const TURN_SPEED = 10

@onready var pivot = $Pivot
@onready var geometry = $Geometry

# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity = ProjectSettings.get_setting("physics/3d/default_gravity")

func _input(event):
	if event is InputEventMouseButton:
		Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
	elif Input.is_action_just_pressed("ui_cancel"):
		Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
		
	if event is InputEventMouseMotion and Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
		rotate_y(deg_to_rad(-(event as InputEventMouseMotion).relative.x * MOUSE_SENSITIVITY))
		geometry.rotate_y(deg_to_rad((event as InputEventMouseMotion).relative.x * MOUSE_SENSITIVITY))
		pivot.rotate_x(deg_to_rad(-(event as InputEventMouseMotion).relative.y * MOUSE_SENSITIVITY))
		pivot.rotation.x = deg_to_rad(clamp(rad_to_deg(pivot.rotation.x), -90, 90))
		
		
func _physics_process(delta):
	# Add the gravity.
	if not is_on_floor():
		velocity.y -= gravity * delta

	# Handle Jump.
	if Input.is_action_just_pressed("jump") and is_on_floor():
		velocity.y = JUMP_VELOCITY

	# Get the input direction and handle the movement/deceleration.
	# As good practice, you should replace UI actions with custom gameplay actions.
	var input_dir = Input.get_vector("left", "right", "up", "down")
	var direction = (transform.basis * Vector3(input_dir.x, 0, input_dir.y)).normalized()
	if direction:
		velocity.x = direction.x * SPEED
		velocity.z = direction.z * SPEED
		var prev_y = geometry.rotation.y
		geometry.look_at(Vector3(position.x, position.y + 1, position.z) + direction)
		var target_y = geometry.rotation.y
		geometry.rotation.y = lerp_angle(prev_y, target_y, delta * TURN_SPEED)
	else:
		velocity.x = move_toward(velocity.x, 0, SPEED)
		velocity.z = move_toward(velocity.z, 0, SPEED)

	move_and_slide()

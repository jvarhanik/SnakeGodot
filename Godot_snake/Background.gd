extends Node2D


var backgrounds: Array = []

# Velikost jednoho bloku pozadí
var block_size: Vector2 = Vector2(16, 16)

func _ready():
	var dir = DirAccess.open("res://graphics/tiles")
	if dir:
		dir.list_dir_begin()
		var file_name = dir.get_next()
		while file_name != "":
			if file_name.ends_with(".png"):
				backgrounds.append(load("res://graphics/tiles/" + file_name))
			file_name = dir.get_next()
		dir.list_dir_end()
	
	if backgrounds.size() == 0:
		push_error("Nenalezeny žádné pozadí v res://graphics/backgrounds")
		return
	
	generate_random_background()

	get_viewport().size_changed.connect(Callable(self, "_on_screen_resized"))

func _on_screen_resized():
	generate_random_background()

func generate_random_background():
	for child in get_children():
		remove_child(child)
		child.queue_free()

	var viewport_size = get_viewport().size
	var num_cols = int(ceil(viewport_size.x / block_size.x))
	var num_rows = int(ceil(viewport_size.y / block_size.y))

	for y in range(num_rows):
		for x in range(num_cols):
			var random_texture = backgrounds[randi() % backgrounds.size()]
			var block = Sprite2D.new()
			block.texture = random_texture
			block.position = Vector2(x * block_size.x, y * block_size.y)
			block.centered = false
			add_child(block)

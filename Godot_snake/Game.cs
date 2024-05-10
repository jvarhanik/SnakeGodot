using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node2D
{
	private Random randomNumber = new Random();
	private Node2D snakeHead;
	private List<Node2D> snakeBody = new List<Node2D>();
	private Node2D berry;
	private Vector2 direction = Vector2.Right;
	private int score = 5;
	private float speed = 15;
	private float moveTimer = 0;
	private bool gameOver = false;
	private GameOverMenu gameOverMenu;
	private PackedScene snakePartScene = (PackedScene)ResourceLoader.Load("res://SnakePart.tscn");

	private TileMap backgroundTileMap;

	public override void _Ready()
	{
		snakeHead = GetNode<Node2D>("SnakeHead");
		berry = GetNode<Node2D>("Berry");
		RespawnBerry();
		InitializeSnakeBody();

		PackedScene gameOverMenuScene = (PackedScene)ResourceLoader.Load("res://GameOverMenu.tscn");
		if (gameOverMenuScene != null)
		{
			gameOverMenu = (GameOverMenu)gameOverMenuScene.Instantiate();
			AddChild(gameOverMenu);
			gameOverMenu.Visible = false;
		}
	}

	private void InitializeSnakeBody()
	{
		for (int i = 1; i <= 5; i++)
		{
			Node2D snakePart = snakePartScene.Instantiate<Node2D>();
			snakePart.Position = new Vector2(560 - i, 336);
			AddChild(snakePart);
			snakeBody.Add(snakePart);
		}
	}

	public override void _Process(double delta)
	{
		if (!IsInstanceValid(snakeHead) || gameOver)
			return;

		HandleInput();
		moveTimer += (float)delta;

		if (moveTimer >= 1.0 / speed)
		{
			MoveSnake();
			moveTimer = 0;
		}

		CheckCollisions();
	}

	private void HandleInput()
	{
		if (Input.IsActionPressed("ui_right") && direction != Vector2.Left)
			direction = Vector2.Right;
		else if (Input.IsActionPressed("ui_left") && direction != Vector2.Right)
			direction = Vector2.Left;
		else if (Input.IsActionPressed("ui_up") && direction != Vector2.Down)
			direction = Vector2.Up;
		else if (Input.IsActionPressed("ui_down") && direction != Vector2.Up)
			direction = Vector2.Down;
	}

	private void MoveSnake()
	{
		Vector2 previousPosition = snakeHead.Position;
		snakeHead.Position += direction * 32;

		// Update snake head rotation
		snakeHead.Rotation = DirectionToRotation(direction);

		// Move and rotate the body parts
		for (int i = snakeBody.Count - 1; i > 0; i--)
		{
			snakeBody[i].Position = snakeBody[i - 1].Position;
			snakeBody[i].Rotation = snakeBody[i - 1].Rotation;
		}
		if (snakeBody.Count > 0)
		{
			snakeBody[0].Position = previousPosition;
			snakeBody[0].Rotation = snakeHead.Rotation;
		}
	}

	private float DirectionToRotation(Vector2 direction)
	{
		if (direction == Vector2.Right) return 0;
		if (direction == Vector2.Left) return Mathf.Pi;
		if (direction == Vector2.Up) return -Mathf.Pi / 2;
		if (direction == Vector2.Down) return Mathf.Pi / 2;

		return 0;
	}

	private void CheckCollisions()
	{
		var viewportRect = GetViewport().GetVisibleRect().Size;

		if (snakeHead.Position.X < 0 || snakeHead.Position.X >= viewportRect.X ||
		snakeHead.Position.Y < 0 || snakeHead.Position.Y >= viewportRect.Y)
		{
			gameOver = true;
			GD.Print("Game Over. Your Score: ", score, " - Out of bounds!");
			gameOverMenu.UpdateScore(score);
			gameOverMenu.Visible = true;
			return;
		}

		if (snakeHead.Position.DistanceTo(berry.Position) < 32)
		{
			score++;
			RespawnBerry();
			ExtendSnake();
		}

		foreach (Node2D part in snakeBody)
		{
			if (snakeHead.Position == part.Position)
			{
				gameOver = true;
				gameOverMenu.UpdateScore(score);
				gameOverMenu.Visible = true;
				GD.Print("Game Over. Your Score: ", score, " - Collision with body!");
				break;
			}
		}
	}

	private void RespawnBerry()
	{
		berry.Position = new Vector2(randomNumber.Next(1, (int)GetViewport().GetVisibleRect().Size.X / 32) * 32 - 16,
									 randomNumber.Next(1, (int)GetViewport().GetVisibleRect().Size.Y / 32) * 32);
	}

	private void ExtendSnake()
	{
		var newPart = snakePartScene.Instantiate<Node2D>();
		newPart.Position = snakeBody[snakeBody.Count - 1].Position;
		newPart.Rotation = snakeBody[snakeBody.Count - 1].Rotation;
		AddChild(newPart);
		snakeBody.Add(newPart);
	}
	
	private void InitializeBackground()
	{
		int gridWidth = (int)GetViewport().GetVisibleRect().Size.X / 32;
		int gridHeight = (int)GetViewport().GetVisibleRect().Size.Y / 32;

		for (int x = 0; x < gridWidth; x++)
		{
			for (int y = 0; y < gridHeight; y++)
			{
				backgroundTileMap.SetCell(0, new Vector2I(x, y), 0);
			}
		}
	}
}

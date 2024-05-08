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
	private float speed = 30;
	private float moveTimer = 0;
	private bool gameOver = false;

	private PackedScene snakePartScene = (PackedScene)ResourceLoader.Load("res://SnakePart.tscn");

	public override void _Ready()
	{
		snakeHead = GetNode<Node2D>("SnakeHead");
		berry = GetNode<Node2D>("Berry");
		RespawnBerry();
		InitializeSnakeBody();
	}

	private void InitializeSnakeBody()
	{
		for (int i = 1; i <= 5; i++)
		{
			Node2D snakePart = snakePartScene.Instantiate<Node2D>();
			snakePart.Position = new Vector2(560-i, 315);
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
		snakeHead.Position += direction * 16;

		for (int i = snakeBody.Count - 1; i > 0; i--)
		{
			snakeBody[i].Position = snakeBody[i - 1].Position;
		}
		if (snakeBody.Count > 0)
		{
			snakeBody[0].Position = previousPosition;
		}
	}

	private void CheckCollisions()
	{
		var viewportRect = GetViewport().GetVisibleRect().Size;

		if (snakeHead.Position.X < 0 || snakeHead.Position.X >= viewportRect.X ||
		snakeHead.Position.Y < 0 || snakeHead.Position.Y >= viewportRect.Y)
		{
			gameOver = true;
			GD.Print("Game Over. Your Score: ", score, " - Out of bounds!");
			return;
		}

		if (snakeHead.Position.DistanceTo(berry.Position) < 16)
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
				GD.Print("Game Over. Your Score: ", score, " - Collision with body!");
				break;
			}
		}
	}

	private void RespawnBerry()
	{
		berry.Position = new Vector2(randomNumber.Next(1, (int)GetViewport().GetVisibleRect().Size.X / 16) * 16,
									 randomNumber.Next(1, (int)GetViewport().GetVisibleRect().Size.Y / 16) * 16);
	}

	private void ExtendSnake()
	{
		var newPart = snakePartScene.Instantiate<Node2D>();
		newPart.Position = snakeBody[snakeBody.Count - 1].Position;
		AddChild(newPart);
		snakeBody.Add(newPart);
	}
}

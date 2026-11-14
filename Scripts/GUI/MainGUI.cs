using Godot;
using System;

public abstract partial class MainGUI : Control
{
    protected GameManager gameManager;



    public override void _Ready()
    {
        gameManager = GetNode<GameManager>(GetTree().CurrentScene.GetPath());

        gameManager.OnSecondPassed += _OnSecondPassed;
        gameManager.OnDayPassed += _OnDayPassed;
    }


    protected abstract void _OnSecondPassed();
    protected virtual void _OnDayPassed() { }
}

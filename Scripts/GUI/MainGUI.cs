using Godot;
using System;

public abstract partial class MainGUI : Control
{
    protected GameManager gameManager;



    public override void _Ready()
    {
        gameManager = GetNode<GameManager>(GetTree().CurrentScene.GetPath());

        gameManager.OnMinutePassed += _OnMinutePassed;
        gameManager.OnDayPassed += _OnDayPassed;
    }


    protected abstract void _OnMinutePassed();
    protected virtual void _OnDayPassed() { }
}

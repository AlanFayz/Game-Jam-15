using Godot;
using System;

public partial class TestFSM : Node
{
	FSMTestMan Parent;
	States CurrentState;
	States PreviousState;
	enum States
	{
		 Enter,
		 Space
	}


	public override void _Ready()
	{
		Parent = GetParent<FSMTestMan>();
		CurrentState = States.Enter;
	}
	public override void _Process(double delta)
	{
		States newState = GetTransition();
		if (newState != CurrentState)
		{
			ChangeState(newState);
		}
		StateLogic(delta);
	}
	private void ChangeState(States NewState)
	{
		PreviousState = CurrentState;
		ExitState(CurrentState);
		EnterState(NewState);
		CurrentState = NewState;
	}

	public void StateLogic(double delta)
	{
	  switch (CurrentState)
	  {
		case States.Enter:
			Parent.Enter();
			break;
		case States.Space:
			Parent.Space();
			break;
	  }  
	}
	private States GetTransition() 
	{
		switch (CurrentState)
		{
			case States.Enter:
				if (Parent.CheckSpace())
				{
					return States.Space;
				}
				break;
			case States.Space:
				if (Parent.CheckEnter())
				{
					return States.Enter;
				}
				break;
		}
		return CurrentState;
	}
	private void EnterState(States State) 
	{
		switch (CurrentState)
	  {
		case States.Enter:
			Parent.StartEnter();
			break;
		case States.Space:
			Parent.StartSpace();
			break;
	  }  
	}
	private void ExitState(States State) {}



}

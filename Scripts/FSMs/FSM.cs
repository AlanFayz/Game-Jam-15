using Godot;
using System;

public partial class FSM : Node
{
	States CurrentState;
	States PreviousState;
	enum States {}

	public override void _Process(double delta)
    {
        States newState = GetTransition();
		if (newState != CurrentState)
		{
			ChangeState(newState);
		}
		StateLogic(delta);
    }

	void ChangeState(States NewState)
	{
		PreviousState = CurrentState;
		ExitState(CurrentState);
		EnterState(NewState);
		CurrentState = NewState;
	}

	States GetTransition() {return CurrentState;}
	void StateLogic(double delta) {}
    void EnterState(States State) {}
	void ExitState(States State) {}

}

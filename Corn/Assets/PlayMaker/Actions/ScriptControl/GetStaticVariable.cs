using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Get static variable from script")]
	public class GetStaticVariable : FsmStateAction
	{

		// Code that runs on entering the state.
		public override void OnEnter()
		{
			Finish();
		}


	}

}

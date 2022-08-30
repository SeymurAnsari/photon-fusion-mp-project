using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
	[SerializeField] private Ball ballPrefab;
	[SerializeField] private PhysxBall physxBallPrefab;
	[Networked] private TickTimer delay { get; set; }

	private Vector3 forwardVector;


	private NetworkCharacterControllerPrototype networkCharacterController;

	private void Awake ()
	{
		networkCharacterController = GetComponent<NetworkCharacterControllerPrototype> ();
		forwardVector = transform.forward;
	}

	public override void FixedUpdateNetwork ()
	{
		if (GetInput (out NetworkInputData data))
		{
			data.direction.Normalize ();
			networkCharacterController.Move (5 * data.direction * Runner.DeltaTime);

			if (data.direction.sqrMagnitude > 0)
			{
				forwardVector = data.direction;
			}

			if (delay.ExpiredOrNotRunning (Runner))
			{
				if ((data.buttons & NetworkInputData.MOUSEBUTTON1) != 0)
				{
					delay = TickTimer.CreateFromSeconds (Runner, 0.5f);
					Runner.Spawn (ballPrefab, transform.position + forwardVector, Quaternion.LookRotation (forwardVector), Object.InputAuthority, (runner, obj) =>
					{
						// Initialize the Ball before synchronizing it
						obj.GetComponent<Ball> ().Init ();
					});
				}
				else if ((data.buttons & NetworkInputData.MOUSEBUTTON2) != 0)
				{
					delay = TickTimer.CreateFromSeconds (Runner, 0.5f);
					Runner.Spawn (physxBallPrefab,
						transform.position + forwardVector,
						Quaternion.LookRotation (forwardVector),
						Object.InputAuthority,
						(runner, obj) => { obj.GetComponent<PhysxBall> ().Init (10 * forwardVector); });
				}
			}
		}
	}
}
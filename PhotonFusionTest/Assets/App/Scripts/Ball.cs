using Fusion;

public class Ball : NetworkBehaviour
{
	public override void FixedUpdateNetwork ()
	{
		transform.position += 5 * transform.forward * Runner.DeltaTime;
	}
}
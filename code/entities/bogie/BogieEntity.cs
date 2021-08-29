using Sandbox;

[Library( "ent_bogie", Title = "Bogie", Spawnable = true )]
public partial class BogieEntity : Prop
{
	private PhysicsBody wheel_f_PhysicsBody;
	private PhysicsBody wheel_r_PhysicsBody;

	public BogieEntity()
	{

	}

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/railway/1520/bogies/1000.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );

		

		wheel_f_PhysicsBody = GetBonePhysicsBody( GetBoneIndex( "wheel_f" ) );
		wheel_r_PhysicsBody = GetBonePhysicsBody( GetBoneIndex( "wheel_r" ) );
	}

	

	[Event.Tick.Server]
	protected void Tick()
	{
		wheel_f_PhysicsBody.AngularVelocity += wheel_f_PhysicsBody.Transform.NormalToWorld(Vector3.Left);
		wheel_r_PhysicsBody.AngularVelocity += wheel_r_PhysicsBody.Transform.NormalToWorld(Vector3.Left);

		wheel_f_PhysicsBody.Wake();
		wheel_r_PhysicsBody.Wake();

		wheel_f_PhysicsBody.PhysicsGroup.Wake();
		wheel_r_PhysicsBody.PhysicsGroup.Wake();
	}
}

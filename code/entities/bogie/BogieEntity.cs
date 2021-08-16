using Sandbox;

[Library( "ent_bogie", Title = "Bogie", Spawnable = true )]
public partial class BogieEntity : Prop
{
	private Prop frontWheel;
	private Prop rearWheel;

	public BogieEntity()
	{

	}

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/train/1520/bogies/simple.vmdl" );
	}

}

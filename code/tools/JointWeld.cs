namespace Sandbox.Tools
{
	[Library( "tool_jointweld", Title = "Joint Weld", Description = "Fixed weld" )]
	public partial class JointWeldTool : BaseTool
	{
		PhysicsBody physicsBody;

		public override void Simulate()
		{
			if ( !Host.IsServer )
				return;

			using ( Prediction.Off() )
			{
				if ( !Input.Pressed( InputButton.Attack1 ) )
					return;

				var startPos = Owner.EyePos;
				var dir = Owner.EyeRot.Forward;

				var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
					.Ignore( Owner )
					.Run();

				if ( !tr.Hit )
					return;

				if ( !tr.Entity.IsValid() )
					return;

				if ( !(tr.Body.IsValid() && (tr.Body.PhysicsGroup != null) && tr.Body.Entity.IsValid()) ) return;

				if ( !physicsBody.IsValid() )
				{
					physicsBody = tr.Body;
				}
				else
				{
					PhysicsJoint.Weld
						.From( physicsBody )
						.To( tr.Body )
						.WithPivot( tr.EndPos )
						.WithBasis( Rotation.LookAt( tr.Normal ) * Rotation.From( new Angles( 90, 0, 0 ) ) )
						.Create();

					physicsBody.PhysicsGroup?.Wake();
					tr.Body.PhysicsGroup?.Wake();

					physicsBody = null;
				}
				CreateHitEffects( tr.EndPos );
			}
		}
	}
}

namespace Sandbox.Tools
{
	[Library( "tool_slider", Title = "Slider", Description = "Physical Slider" )]
	public partial class SliderTool : BaseTool
	{
		private PhysicsBody body1, body2;

		private Vector3 LPos1;

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

				if ( !body1.IsValid() )
				{
					body1 = tr.Body;

					LPos1 = body1.Transform.PointToLocal( tr.EndPos );
				}
				else
				{
					body2 = tr.Body;

					if ( body1 == body2 )
					{
						body1 = null;
						return;
					}

					var WPos1 = body1.Transform.PointToWorld( LPos1 );
					var WPos2 = tr.EndPos;

					PhysicsJoint.Prismatic
						.From( body1 )
						.To( body2 )
						.WithPivot( WPos2 )
						.WithBasis( Rotation.LookAt( (WPos2 - WPos1).Normal ) * Rotation.From( new Angles( 90, 0, 0 ) ) )
						.WithCollisionsEnabled()
						.Create();

					body1.PhysicsGroup?.Wake();
					body2.PhysicsGroup?.Wake();

					body1 = null;
					body2 = null;
				}
				CreateHitEffects( tr.EndPos );
			}
		}
	}
}

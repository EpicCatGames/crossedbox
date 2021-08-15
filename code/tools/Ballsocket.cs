namespace Sandbox.Tools
{
	[Library( "tool_ballsocket", Title = "Ballsocket", Description = "Physical ballsocket" )]
	public partial class BallsocketTool : BaseTool
	{
		private PhysicsBody body1, body2;

		private Vector3 LNorm1;
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
					if ( tr.Entity.IsWorld || tr.Entity is WorldEntity ) return;

					body1 = tr.Body;

					LNorm1 = body1.Transform.NormalToLocal( tr.Normal );

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

					body1.Rotation = Rotation.LookAt( tr.Normal ) * Rotation.From( new Angles( 0, -180, 0 ) );
					body1.Rotation = Rotation.Difference( Rotation.LookAt( LNorm1 ), body1.Rotation );

					body1.Position += tr.EndPos - body1.Transform.PointToWorld( LPos1 );

					PhysicsJoint.Spherical
						.From( body1 )
						.To( body2 )
						.WithPivot( tr.EndPos )
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

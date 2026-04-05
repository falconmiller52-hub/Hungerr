namespace Runtime.Features.Health
{
	/// <summary>
	/// Реализаторы получают возможность получать урон
	/// </summary>
	public interface IDamageable
	{
		void ApplyDamage(int value);
	}
}

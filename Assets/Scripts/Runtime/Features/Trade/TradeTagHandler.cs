namespace Runtime.Features.Trade
{
	public class TradeTagHandler
	{
		private ITrade _trade;

		public TradeTagHandler(ITrade trade)
		{
			_trade = trade;
		}

		public void GetTagValue(string valueTag)
		{
			switch (valueTag)
			{
				case "OpenTrade":
				{
					_trade.StartTrade();
					break;
				}
			}
		}
	}
}
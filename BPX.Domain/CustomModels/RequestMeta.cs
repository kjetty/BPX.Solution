namespace BPX.Domain.CustomModels
{
	public class RequestMeta
	{
		public string host { get; set; }
		public string area { get; set; }
		public string controller { get; set; }
		public string action { get; set; }
		public string id { get; set; }

		public RequestMeta()
		{
			host = string.Empty;
			area = string.Empty;
			controller = string.Empty;
			action = string.Empty;
			id = string.Empty;
		}
	}
}

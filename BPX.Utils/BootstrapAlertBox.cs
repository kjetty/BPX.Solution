namespace BPX.Utils
{
	public class AlertBox
    {
		// DO NOT provide the default constructor
		//public AlertBox()
		//{
		//}

		//public AlertBox()
		//{
        //      throw new NotImplementedException();
		//}

        public AlertBox(AlertType alertType, string alertMessage)
        {
            this.alertType = (int)alertType;
            this.alertMessage = alertMessage;
        }

        public int alertType { get; set; }
        public string alertMessage { get; set; }
    }

    public enum AlertType
    {
        Success, Info, Warning, Error
    }
}
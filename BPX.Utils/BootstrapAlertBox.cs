namespace BPX.Utils
{
	public class BootstrapAlertBox
    {
		// DO NOT provide the default constructor
		//public BootstrapAlertBox()
		//{
		//}

		//public BootstrapAlertBox()
		//{
        //      throw new NotImplementedException();
		//}

        public BootstrapAlertBox(AlertType alertType, string alertMessage)
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
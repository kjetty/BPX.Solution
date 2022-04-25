namespace BPX.Domain.ViewModels
{
    public class ErrorPageViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}

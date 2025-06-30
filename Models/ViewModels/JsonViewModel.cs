using Newtonsoft.Json;

namespace ProjectManagement.Models
{
    [Serializable]

    public class JsonViewModel
    {
        public int ResponseCode { get; set; }

        public string ResponseMessage { get; set; } = string.Empty;
    }
}
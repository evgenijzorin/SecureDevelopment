using Microsoft.AspNetCore.Mvc;

namespace CardStorageService.Models.Requests
{
    public class AuthenticationResponse : IOperationResult
    {
        public AuthenticationStatus Status { get; set; }
        public SessionInfo SessionInfo { get; set; }

        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}

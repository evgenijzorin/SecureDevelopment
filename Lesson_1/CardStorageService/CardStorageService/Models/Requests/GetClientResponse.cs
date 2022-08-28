using CardStorageService.Data;
using System.Collections.Generic;

namespace CardStorageService.Models.Requests
{
    public class GetClientResponse : IOperationResult
    {
        public ClientDto? ClientDto { get; set; }
        public IList<ClientDto>? ClientsDto { get; set; }

        public int ErrorCode { get; set; }

        public string? ErrorMessage { get; set; }

    }
}

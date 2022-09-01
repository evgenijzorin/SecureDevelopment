using CardStorageService.Data;
using System.Collections.Generic;

namespace CardStorageService.Models.Requests
{
    public class GetCardResponse : IOperationResult
    {
        public CardDto? cardDto { get; set; }
        public IList<CardDto>? CardsDto { get; set; }
        public int ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

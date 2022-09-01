namespace CardStorageService.Models.Requests
{
    /// <summary>
    /// Описывает результат выполнения операции возвращая код выполнения
    /// </summary>
    public interface IOperationResult
    {
        int ErrorCode { get; }
        string? ErrorMessage { get; }

    }
}

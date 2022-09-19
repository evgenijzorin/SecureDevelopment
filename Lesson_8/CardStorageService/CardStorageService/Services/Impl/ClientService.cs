using AutoMapper;
using CardStorageService.Controllers;
using CardStorageService.Data;
using ClientServiceProtos;
using FluentValidation;
using FluentValidation.Results;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using static ClientServiceProtos.ClientService;

namespace CardStorageService.Services.Impl
{
    public class ClientService : ClientServiceBase
    {
        #region Services
        private readonly IClientRepositoryService _clientRepositoryService; // Ссылка на контекст
        private readonly ILogger<ClientService> _logger; // Ведение журнала        
        private readonly IMapper _mapper;
        #endregion

        public ClientService(ILogger<ClientService> logger,
            IClientRepositoryService clientRepositoryService,            
            IMapper mapper
            )
        {
            _logger = logger;
            _clientRepositoryService = clientRepositoryService;            
            _mapper = mapper;
        }

        public override Task<CreateClientResponse> Create(CreateClientRequest request, ServerCallContext context)
        {
            try
            {
                // Валидация запроса
                // ValidationResult validationResult = _createClieentReuqestValidator.Validate(request);
                //if (!validationResult.IsValid)
                //    return;
                // Создание клиента в базе данных
                var clientId = _clientRepositoryService.Create(_mapper.Map<Client>(request));

                var response = new CreateClientResponse
                {
                    ClientId = clientId,
                    ErrorCode = 0,
                    ErrorMessage = String.Empty
                };
                return Task.FromResult(response);

            }
            catch (Exception e)
            {
                // логируем ошибку
                _logger.LogError(e, "Create client error.");

                var response = new CreateClientResponse
                {
                    ClientId = -1,
                    ErrorCode = 912,
                    ErrorMessage = "Create client error."
                };
                return Task.FromResult(response);
            }
        }
    }
}

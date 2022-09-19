using CardStorageService.Data;
using CardStorageService.Models;
using CardStorageService.Models.Requests;
using CardStorageService.Services;
using CardStorageService.Services.Impl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;
using AutoMapper;

namespace CardStorageService.Controllers
{
    [Authorize]
    [Route("api/client")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        #region Services
        private readonly IClientRepositoryService _clientRepositoryService; // Ссылка на контекст
        private readonly ILogger<ClientController> _logger; // Ведение журнала
        private readonly IValidator<CreateClientRequest> _createClieentReuqestValidator;
        private readonly IValidator<DeleteClientRequest> _deleteClientRequestValidator;
        private readonly IMapper _mapper;

        #endregion

        #region Constructors
        // В конструкторе класса инициализируем сервисы (DI) (связать контроллеры с репозиториями
        public ClientController(
            ILogger<ClientController> logger,
            IClientRepositoryService clientRepositoryService,
            IValidator<CreateClientRequest> createClieentReuqestValidator,
            IValidator<DeleteClientRequest> deleteClientRequestValidator,
            IMapper mapper)
        {
            _logger = logger;
            _clientRepositoryService = clientRepositoryService;
            _createClieentReuqestValidator = createClieentReuqestValidator;
            _deleteClientRequestValidator = deleteClientRequestValidator;
            _mapper = mapper;
        }
        #endregion

        #region Create Client
        [HttpPost("create")]
        [ProducesResponseType(typeof(CreateClientResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
        public IActionResult Create([FromBody] CreateClientRequest request)
        {
            try
            {
                // Валидация запроса
                ValidationResult validationResult = _createClieentReuqestValidator.Validate(request);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.ToDictionary());
                // Создание клиента в базе данных
                var clientId = _clientRepositoryService.Create(_mapper.Map<Client>(request)); 
                return Ok(new CreateClientResponse
                {
                    ClientId = clientId
                });
            }
            catch (Exception e)
            {
                // логируем ошибку
                _logger.LogError(e, "Create client error.");
                return Ok(new CreateClientResponse
                {
                    ErrorCode = 912,
                    ErrorMessage = "Create clinet error."
                });
            }
        }
        #endregion

        #region get client by card id
        [HttpGet("get-by-card-id")]
        [ProducesResponseType(typeof(GetClientResponse), StatusCodes.Status200OK)]
        // [FromQuery]: возвращает значения из строки запроса.
        public IActionResult GetByCardId([FromQuery] string cardId)
        {
            try
            {
                Client client = _clientRepositoryService.GetByCardId(cardId);
                GetClientResponse getClientResponse = new GetClientResponse {ClientDto = _mapper.Map<ClientDto>(client) }; 
                return Ok(getClientResponse);  
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Get by card id error");
                return Ok(new GetClientResponse
                {
                    ErrorCode = 914,
                    ErrorMessage = "Get by card id error",
                });
            }
        }
        #endregion

        #region Get all clients
        [HttpGet("get-all")]
        [ProducesResponseType(typeof(GetClientResponse), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {            
            try
            {
                IList<Client> clients = _clientRepositoryService.GetAll();
                IList<ClientDto> clientsDto = new List<ClientDto>();
                for(int i = 0; i<clients.Count; i++)
                {                   
                    clientsDto.Add(_mapper.Map<ClientDto>(clients[i]));
                }
                return Ok(new GetClientResponse
                { ClientsDto = clientsDto});
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Get all clients error");
                return Ok(new GetCardResponse
                {
                    ErrorCode = 915,
                    ErrorMessage = "Get all clients error",
                });
            }
        }
        #endregion
         
        #region Delete client
        [HttpPost("delete")]
        [ProducesResponseType(typeof(IDictionary<string,string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DeleteClientResponse), StatusCodes.Status200OK)]
        public IActionResult DeletebyId(DeleteClientRequest deleteclientRequest)
        {
            try
            {
                ValidationResult validationResult= _deleteClientRequestValidator.Validate(deleteclientRequest);
                if(!validationResult.IsValid)
                    return BadRequest(validationResult.ToDictionary());

                _clientRepositoryService.Delete(deleteclientRequest.ClientId);
                return Ok(new DeleteClientResponse
                {
                    ClientId = deleteclientRequest.ClientId
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Delete client error");
                return Ok(new GetCardResponse
                {
                    ErrorCode = 916,
                    ErrorMessage = "Delete client error",
                });
            }
        }
        #endregion

    }
}

using CardStorageService.Data;
using CardStorageService.Models;
using CardStorageService.Models.Requests;
using CardStorageService.Services;
using CardStorageService.Services.Impl;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace CardStorageService.Controllers
{
    [Route("api/client")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        #region Services
        private readonly IClientRepositoryService _clientRepositoryService; // Ссылка на контекст
        private readonly ILogger<ClientController> _logger; // Ведение журнала
        

        // В конструкторе класса инициализируем сервисы (DI) (связать контроллеры с репозиториями
        public ClientController(
            ILogger<ClientController> logger,
            IClientRepositoryService clientRepositoryService)
        {
            _logger = logger;
            _clientRepositoryService = clientRepositoryService;
        }
        #endregion

        #region Public methods
        [HttpPost("create")]
        [ProducesResponseType(typeof(CreateClientResponse), StatusCodes.Status200OK)]
        public IActionResult Create([FromBody] CreateClientRequest request)
        {
            try
            {
                var clientId = _clientRepositoryService.Create(new Client
                {
                    FirstName = request.FirstName,
                    Surname = request.Surname,
                    Patronymic = request.Patronymic
                });
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

        [HttpGet("get-by-card-id")]
        [ProducesResponseType(typeof(GetClientResponse), StatusCodes.Status200OK)]
        // [FromQuery]: возвращает значения из строки запроса.
        public IActionResult GetByCardId([FromQuery] string cardId)
        {
            try
            {
                Client client = _clientRepositoryService.GetByCardId(cardId);
                return Ok(new GetClientResponse
                {
                    // конструкция возратит коллекцию Cards объектов CardDto, полученными путем копирования свойств объектов коллекции cards
                    ClientDto = new Models.ClientDto 
                    { ClientId = client.ClientId,
                        // Cards = client.Cards, // отключено, так как вызовет ошибку циклической ссылки                    
                        FirstName = client.FirstName,                   
                        Patronymic = client.Patronymic,                   
                        Surname = client.Surname
                    }
                });  
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
                    Client client = clients[i];
                    clientsDto.Add(new ClientDto()
                    {
                        ClientId = client.ClientId,
                        FirstName = client.FirstName,
                        Patronymic = client.Patronymic,
                        Surname = client.Surname
                    });
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

        [HttpPost("delete")]
        [ProducesResponseType(typeof(GetCardResponse), StatusCodes.Status200OK)]
        public IActionResult DeletebyId(int id)
        {
            try
            {
                _clientRepositoryService.Delete(id);
                return Ok(new DeleteClientResponse
                {
                    ClientId = id
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

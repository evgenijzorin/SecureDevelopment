using CardStorageService.Data;
using CardStorageService.Models;
using CardStorageService.Models.Requests;
using CardStorageService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CardStorageService.Controllers
{
    [Authorize]
    [Route("api/cards")]
    [ApiController]
    public class CardController : ControllerBase
    {
        #region Services

        private readonly ICardRepositoryService _cardRepositoryService;
        private readonly ILogger<CardController> _logger;

        #endregion

        #region Constructors

        public CardController(
            ILogger<CardController> logger,
            ICardRepositoryService cardRepositoryService)
        {
            _logger = logger;
            _cardRepositoryService = cardRepositoryService;
        }

        #endregion

        #region Pulbic Methods
        [HttpPost("create")]
        // фильтр типов значений возвращенных действием и статус код возвращаемый действием
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        // [FromBody]: возвращает значения из текста запроса.
        public IActionResult Create([FromBody] CreateCardRequest request)
        {
            try
            {
                // создать новую карту и получить её Id
                var cardId = _cardRepositoryService.Create(new Data.Card
                {
                    ClientId = request.ClientId,                    
                    CardNo = request.CardNo,
                    Name = request.Name,
                    ExDate =request.ExpirationDate,
                    CVV2 = request.CVV2
                });
                return Ok(new CreateCardResponse
                {
                    CardId = cardId.ToString()
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Create card error");
                return Ok(new CreateCardResponse
                {
                    ErrorCode = 1012,
                    ErrorMessage = "Create card error"
                });
            }
        }

        [HttpGet("get-by-client-id")]
        [ProducesResponseType(typeof(GetCardResponse), StatusCodes.Status200OK)]
        // [FromQuery]: возвращает значения из строки запроса.
        public IActionResult GetByClientId ([FromQuery] int clientId)
        {
            try
            {
                var cards = _cardRepositoryService.GetByClientId(clientId);
                return Ok(new GetCardResponse
                {
                    // конструкция возратит коллекцию Cards объектов CardDto, полученными путем копирования свойств объектов коллекции cards
                    CardsDto = cards.Select(card => new CardDto
                    {
                        CardId = card.CardId, 
                        CardNo = card.CardNo,
                        CVV2 = card.CVV2,
                        Name = card.Name,
                        ExpDate = card.ExDate.ToString("MM/yy"), // формат месяц/год
                        // Client = card.Client // отключено, так как вызовет ошибку циклической ссылки
                    }).ToList()
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Get cards error");
                return Ok(new GetCardResponse
                {
                    ErrorCode = 1013,
                    ErrorMessage = "Get cards error",
                });
            }
        }

        [HttpGet("get-all")]
        [ProducesResponseType(typeof(GetCardResponse), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            try
            {
                IList<Card> cards = _cardRepositoryService.GetAll();
                IList<CardDto> cardsDto = new List<CardDto>();
                for (int i = 0; i < cards.Count; i++)
                {
                    Card card = cards[i];
                    cardsDto.Add(new CardDto()
                    {
                        CardId=card.CardId,
                        CardNo=card.CardNo,
                        CVV2=card.CVV2,
                        ExpDate = card.ExDate.ToString(),
                        Name=card.Name,
                        ClientId=card.ClientId                        
                    });
                }
                return Ok(new GetCardResponse
                { CardsDto = cardsDto });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Get all cards error");
                return Ok(new GetCardResponse
                {
                    ErrorCode = 1014,
                    ErrorMessage = "Get all cards error",
                });
            }

        }

        [HttpPost("delete")]
        [ProducesResponseType(typeof(DeleteCardResponse), StatusCodes.Status200OK)]
        public IActionResult DeletebyId(string id)
        {
            try
            {
                _cardRepositoryService.Delete(id);
                return Ok(new DeleteCardResponse
                {
                    CardId = id
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Delete card error");
                return Ok(new GetCardResponse
                {
                    ErrorCode = 1015,
                    ErrorMessage = "Delete card error",
                });
            }
        }
        #endregion        
    }
}

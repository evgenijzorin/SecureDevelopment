using AutoMapper;
using CardStorageService.Data;
using CardStorageService.Models;
using CardStorageService.Models.Requests;
using CardStorageService.Models.Validstors;
using CardStorageService.Services;
using FluentValidation;
using FluentValidation.Results;
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
        private readonly IMapper _mapper;
        private readonly IValidator<CreateCardRequest> _createCardRequestValidator;
        private readonly IValidator<DeleteCardRequest> _deleteCardRequestValidator;        
        #endregion

        #region Constructors
        public CardController(
            ILogger<CardController> logger,
            ICardRepositoryService cardRepositoryService,
            IMapper mapper,
            IValidator<CreateCardRequest> createCardRequestValidator,
            IValidator<DeleteCardRequest> deleteCardRequestValidator)
        {
            _logger = logger;
            _cardRepositoryService = cardRepositoryService;
            _mapper = mapper;
            _createCardRequestValidator = createCardRequestValidator;
            _deleteCardRequestValidator = deleteCardRequestValidator;
        }
        #endregion

        #region create Card
        [HttpPost("create")]
        // фильтр типов значений возвращенных действием и статус код возвращаемый действием
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        // Тип возвращаемого значения при невалидности запроса 
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
        // [FromBody]: возвращает значения из текста запроса.
        public IActionResult Create([FromBody] CreateCardRequest request)
        {
            try
            {
                // Валидировать запрос
                ValidationResult validationResult = _createCardRequestValidator.Validate(request);
                if (!validationResult.IsValid)
                    // если провалидировать параметр не удалось возвращается объект хранящий данные валидации
                    return BadRequest(validationResult.ToDictionary());

                #region Маппинг вручную (заменен на автомаппинг)
                // создать новую карту и получить её Id (Маппинг)
                //var cardId = _cardRepositoryService.Create(new Data.Card
                //{
                //    ClientId = request.ClientId,                    
                //    CardNo = request.CardNo,
                //    Name = request.Name,
                //    ExDate =request.ExpirationDate,
                //    CVV2 = request.CVV2
                //});
                // синтаксис был заменен на конструкцию автомаппера
                #endregion

                // конструкция автомаппера: Следующий синтаксис создает объект Card из объекта CreateCardRequest request
                var cardId = _cardRepositoryService.Create(_mapper.Map<Card>(request));
                   
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
        #endregion

        #region Action: get card by client id
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
                    #region Маппинг вручную (заменен на автомаппинг)
                    //CardsDto = cards.Select(card => new CardDto
                    //{
                    //    CardId = card.CardId, 
                    //    CardNo = card.CardNo,
                    //    CVV2 = card.CVV2,
                    //    Name = card.Name,
                    //    ExpDate = card.ExDate.ToString("MM/yy"), // формат месяц/год                        
                    //}).ToList()
                    // заменяем синтаксис на конструкцию автомаппера
                    #endregion

                    // конструкция автомаппера: Следующий синтаксис создает List<CardDto> из IList cards
                    CardsDto = _mapper.Map<List<CardDto>>(cards)
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
        #endregion

        #region Action: get all card
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
                    cardsDto.Add(_mapper.Map<CardDto>(cards[i]));
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
        #endregion

        #region Action: Delete card by id
        [HttpPost("delete")]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DeleteCardResponse), StatusCodes.Status200OK)]
        public IActionResult DeletebyId(DeleteCardRequest deleteCardRequest)
        {
            try
            {
                // Валидировать запрос
                ValidationResult validationResult = _deleteCardRequestValidator.Validate(deleteCardRequest);
                if (!validationResult.IsValid)
                    // если провалидировать параметр не удалось возвращается объект хранящий данные валидации
                    return BadRequest(validationResult.ToDictionary());

                _cardRepositoryService.Delete(deleteCardRequest.CardId);
                return Ok(new DeleteCardResponse
                {
                    CardId = deleteCardRequest.CardId
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

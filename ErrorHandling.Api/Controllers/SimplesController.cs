using ErrorHandling.Api.Util;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ErrorHandling.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimplesController : ControllerBase
    {
        private readonly IValidator<ProdutoDTO> _validator;
        private readonly ILogger<ProdutoDTO> _logger;

        public SimplesController(IValidator<ProdutoDTO> validator, ILogger<ProdutoDTO> logger)
        {
            _validator = validator;
            _logger = logger;
   
        }

        [HttpGet("GeraExceptionDeTeste")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GeraExceptionDeTeste()
        {
            throw new Exception("Gerando uma Exception no controller");
        }

        [HttpPost("CriarProdutoFluentValidation")]
        public async Task<ActionResult<Resultado<Guid>>> CriarProdutoFluentValidation([FromBody] ProdutoDTO request, CancellationToken cancellationToken)
        {
            // Valida o request
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            // Retorna erro se a validação falhar
            if (!validationResult.IsValid)
            {
                return Resultado<Guid>.ComFalha(Erro.RequisicaoInvalidaProduto);
            }

            // Cria um novo produto se a validação for bem-sucedida
            var product = new ProdutoDTO(
                Guid.NewGuid(),
                DateTime.UtcNow,
                request.Nome,
                request.Descricao,
                request.Preco
            );


            // Retorna o sucesso com o Id do produto criado
            return Resultado<Guid>.ComSucesso(product.Id);
        }


        [HttpPost("CriarProdutoFluentValidation2")]
        public async Task<IActionResult> CriarProdutoFluentValidation2(ProdutoDTO produtoDTO)
        {
            var validationResult = await _validator.ValidateAsync(produtoDTO);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Erro de validação: {0}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                throw new ValidationException(validationResult.Errors); // Lança a exceção de validação
            }

 
            return Ok(new { Message = "Produto criado com sucesso" });
        }

    

    }
}

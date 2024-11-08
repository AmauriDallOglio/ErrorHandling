using FluentValidation;

namespace ErrorHandling.Api.Util
{
    public class ProdutoDTOValidator : AbstractValidator<ProdutoDTO>
    {
        public ProdutoDTOValidator()
        {
            RuleFor(x => x.Nome).NotEmpty().WithMessage("O nome do produto é obrigatório.");
            RuleFor(x => x.Preco).GreaterThan(0).WithMessage("O preço do produto deve ser maior que zero.");
        }
    }
}

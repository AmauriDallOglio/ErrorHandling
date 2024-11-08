namespace ErrorHandling.Api.Util
{
    public class ProdutoDTO
    {
        public Guid Id { get; } // Identificador único do produto
        public DateTime DataCriacao { get; } // Data de criação do produto
        public string Nome { get; } // Nome do produto
        public string Descricao { get; } // Descrição do produto
        public decimal Preco { get; } // Preço do produto

        public ProdutoDTO(Guid id, DateTime dataCriacao, string nome, string descricao, decimal preco)
        {
            Id = id;
            DataCriacao = dataCriacao;
            Nome = nome;
            Descricao = descricao;
            Preco = preco;
        }
    }
}

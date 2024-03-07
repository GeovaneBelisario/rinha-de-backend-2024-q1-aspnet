namespace Rinha.Backend.DTOs
{
    public record TransacaoRequest(int Valor, string Tipo, string Descricao)
    {
        public bool Valida() =>
            Tipo is "c" or "d"
            && !string.IsNullOrWhiteSpace(Descricao)
            && Descricao.Length <= 10
            && Valor > 0;
    }
}

using static Rinha.Backend.DTOs.ExtratoResponse;

namespace Rinha.Backend.DTOs
{
    public record ExtratoResponse(SaldoResponse? Saldo, List<TransacaoSaldoResponse>? Ultimas_Transacoes)
    {
        public record SaldoResponse(int Total, DateTime Data_Extrato, int Limite);

        public record TransacaoSaldoResponse(int Valor, string Tipo, string? Descricao, DateTime Realizada_Em);
    }
}

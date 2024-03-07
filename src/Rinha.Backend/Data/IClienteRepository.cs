using Rinha.Backend.DTOs;

namespace Rinha.Backend.Data
{
    public interface IClienteRepository
    {
        Task<ExtratoResponse> GetExtratoAsync(int clienteId);

        Task<TransacaoResponse> InserirTransacaoAsync(int clienteId, TransacaoRequest transacao);
    }
}

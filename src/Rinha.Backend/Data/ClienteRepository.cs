using Npgsql;
using Rinha.Backend.DTOs;
using static Rinha.Backend.DTOs.ExtratoResponse;

namespace Rinha.Backend.Data
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly NpgsqlConnection connection;

        public ClienteRepository(NpgsqlConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public async Task<ExtratoResponse> GetExtratoAsync(int clienteId)
        {
            await connection.OpenAsync();

            SaldoResponse saldo = new(0, DateTime.UtcNow, 0);
            using (var command = new NpgsqlCommand("SELECT clientes.limite, clientes.saldo FROM clientes WHERE clientes.id = @Id", connection))
            {
                command.Parameters.AddWithValue("@Id", clienteId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        saldo = new(reader.GetInt32(1), DateTime.UtcNow, reader.GetInt32(0));
                    }
                }
            }

            var transacoes = new List<TransacaoSaldoResponse>();
            var query = @"SELECT transacoes.valor, transacoes.tipo, transacoes.descricao, transacoes.realizada_em 
                   FROM transacoes 
                   WHERE cliente_id = @Id
                   ORDER BY transacoes.realizada_em DESC
                   LIMIT 10";
            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", clienteId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                        transacoes.Add(new(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3)));
                }
            }

            return new ExtratoResponse(saldo, transacoes);
        }

        public async Task<TransacaoResponse> InserirTransacaoAsync(int clienteId, TransacaoRequest transacao)
        {
            await connection.OpenAsync();

            using (var command = new NpgsqlCommand("SELECT * FROM InserirTransacao(@clienteId, @transacaoValor, @transacaoTipo, @transacaoDescricao)", connection))
            {
                command.Parameters.AddWithValue("@clienteId", clienteId);
                command.Parameters.AddWithValue("@transacaoValor", transacao.Valor);
                command.Parameters.AddWithValue("@transacaoTipo", transacao.Tipo);
                command.Parameters.AddWithValue("@transacaoDescricao", transacao.Descricao);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new(reader.GetInt32(0), reader.GetInt32(1));
                    }
                }
            }

            return null;
        }
    }
}

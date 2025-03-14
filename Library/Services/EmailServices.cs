using FluentEmail.Core;

namespace Library.Services
{
    public class EmailServices
    {
        private readonly IFluentEmail _fluentEmail;
        public EmailServices(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }

        public async Task<bool> SendConfirm(string name, string email)
        {

                var result = await _fluentEmail.To(email).Subject("Conferma prenotazione").Body($"Caro {name} hai effettuato un ordine presso il nostro sito!").SendAsync();
                return result.Successful;

        }

    }
}

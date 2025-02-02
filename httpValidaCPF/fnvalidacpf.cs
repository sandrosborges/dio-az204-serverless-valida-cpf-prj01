using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;

namespace httpValidaCPF
{
    public static class Fnvalidacpf
    {
        [FunctionName("fnvalidacpf")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Iniciando a validacao de CPF");

    

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            if(data==null)
                return new BadRequestObjectResult("Nenhum dado informado.");
    

            string cpf = data?.cpf;

            if(cpf == null)
                return new BadRequestObjectResult("Informe o CPF");
            

            if(!ValidaCPF(cpf))
                return new BadRequestObjectResult("O CPF informado e invalido.");

            var msgResponse = "CPF valido.";


            return new OkObjectResult(msgResponse);
        }

            
        public static bool ValidaCPF(string cpf)
        {
            // Remove caracteres não numéricos
            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            // Verifica se o CPF tem 11 dígitos
            if (cpf.Length != 11)
                return false;

            // Verifica se todos os dígitos são iguais (caso inválido)
            if (cpf.Distinct().Count() == 1)
                return false;

            // Calcula o primeiro dígito verificador
            int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma1 = 0;
            for (int i = 0; i < 9; i++)
                soma1 += int.Parse(cpf[i].ToString()) * multiplicador1[i];
            int resto1 = soma1 % 11;
            int digito1 = resto1 < 2 ? 0 : 11 - resto1;

            // Calcula o segundo dígito verificador
            int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma2 = 0;
            for (int i = 0; i < 10; i++)
                soma2 += int.Parse(cpf[i].ToString()) * multiplicador2[i];
            int resto2 = soma2 % 11;
            int digito2 = resto2 < 2 ? 0 : 11 - resto2;

            // Verifica se os dígitos verificadores calculados são iguais aos do CPF fornecido
            return cpf.EndsWith(digito1.ToString() + digito2.ToString());

        }

    }

}

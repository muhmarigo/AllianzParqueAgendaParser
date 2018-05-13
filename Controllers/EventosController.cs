using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllianzCalendar.ViewModels;
using AngleSharp;
using Microsoft.AspNetCore.Mvc;

namespace AllianzCalendar.Controllers
{
    [Route("/")]
    [ApiController]
    public class EventosController : ControllerBase
    {
        Dictionary<string, byte> arrayMeses = new Dictionary<string, byte> {
             {"JAN", 1} ,
             {"FEV", 2},
             {"MAR", 3},
             {"ABR", 4},
             {"MAI", 5},
             {"JUN", 6},
             {"JUL", 7},
             {"AGO", 8},
             {"SET", 9},
             {"OUT", 10},
             {"NOV", 11},
             {"DEZ", 12}
            };

        [HttpGet]
        public async Task<IEnumerable<Evento>> Get()
        {
            var config = Configuration.Default.WithDefaultLoader();
            var address = "http://www.allianzparque.com.br/agenda/calendario-geral";
            var document = await BrowsingContext.New(config).OpenAsync(address);
            var cells = document.QuerySelectorAll(".event-details");
            return cells.Select(x => new Evento
            {
                Titulo = x?.QuerySelector(".title a")?.TextContent,
                Tipo = x.QuerySelector(".date span")?.TextContent,
                Data = ExtractDate(x?.QuerySelector(".date")?.TextContent)
            });
        }

        private DateTime ExtractDate(string date)
        {
            var textoData = date.Split("|")[1].Trim();
            var partesData = textoData.Split(" ");
            byte dia = byte.Parse(partesData[0]);
            byte mes = arrayMeses[partesData[1]];
            var mesAtual = DateTime.UtcNow.Month;
            var ano = DateTime.UtcNow.Year;
            if (mes < mesAtual)
            {
                ano += 1;
            }
            return new DateTime(ano, mes, dia);
        }
    }
}
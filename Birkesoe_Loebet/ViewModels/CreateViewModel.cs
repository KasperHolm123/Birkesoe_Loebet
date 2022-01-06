using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Birkesoe_Loebet.Models;

namespace Birkesoe_Loebet.ViewModels
{
    public class CreateViewModel
    {
        //Properties bindet til tekst-felter i UI
        Runner model = new Runner();
        public RelayCommand CreateUser { get; set; }
        public CreateViewModel()
        {
            CreateUser = new RelayCommand(p => CreateCmd());

        }
        private void CreateCmd()
        {
            //INSERT INTO query 
        }
        private void BuildModel()
        {
            //Sæt properties for Runner modellen
        }
    }
}

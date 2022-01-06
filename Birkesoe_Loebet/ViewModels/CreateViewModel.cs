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
        //Properties bindet til tekst-felter i UI, her kan vi også evt enforce attributes' domæner. Den vil kun registrere brugeren med et gyldigt tlf-nr f.eks
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        private bool route1Enabled;
        private bool route2Enabled;
        private bool route3Enabled;
        private List<RunningCourse> courses;
        Runner model = new Runner();
        public RelayCommand CreateUser { get; set; }
        public CreateViewModel()
        {
            courses = new List<RunningCourse>();
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
        private bool ValidateInput() //Validerer at formattet af input stemmer
        {
            if (PhoneNumber.Length == 8 && Name.Length < 40)
            {
                return true;
            }
            else return false;
        }
        public bool Route1Enabled
        {
            get
            {
                return route1Enabled;
            }
            set
            {
                route1Enabled = value;
                if (route1Enabled)
                {
                    courses[0] = new RunningCourse(10, 10);
                }
                else
                {
                    courses[0] = null;
                }
            }
        }
        public bool Route2Enabled
        {
            get
            {
                return route2Enabled;
            }
            set
            {
                route2Enabled = value;
                if (route2Enabled)
                {
                    courses[1] = new RunningCourse(10, 10);
                }
                else
                {
                    courses[1] = null;
                }
            }
        }
        public bool Route3Enabled
        {
            get
            {
                return route3Enabled;
            }
            set
            {
                route3Enabled = value;
                if (route3Enabled)
                {
                    courses[2] = new RunningCourse(10, 10);
                }
                else
                {
                    courses[2] = null;
                }
            }
        }
    }
}
